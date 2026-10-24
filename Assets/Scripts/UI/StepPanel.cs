using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>
    /// Painel inferior da etapa atual. Só mostra o estado do InspectionFlow e
    /// repassa os cliques — não decide nada. Abre minimizado; o botão Guia mostra
    /// ou esconde a instrução e os cards, e o botão Registrar abre a RecordScreen
    /// (checklist RF06 e medições RF07) numa tela à parte.
    /// </summary>
    public class StepPanel : MonoBehaviour
    {
        [SerializeField] private InspectionFlow flow;

        [Header("Aviso de busca")]
        [SerializeField] private GameObject scanHint;
        [SerializeField] private Text scanHintText;

        [Header("Painel da etapa")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text stepLabel;
        [SerializeField] private Text titleText;
        [SerializeField] private Text instructionText;
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private InfoCardView cardTemplate;
        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Text nextLabel;

        [Header("Abas")]
        [Tooltip("Linha das abas: Guia e Registrar")]
        [SerializeField] private GameObject tabsRoot;
        [SerializeField] private Button guideTab;
        [Tooltip("Instrução e cards; o botão Guia mostra e esconde")]
        [SerializeField] private GameObject guideGroup;

        [Header("Registro")]
        [Tooltip("Tela à parte com checklist e medições, obrigatória para avançar")]
        [SerializeField] private RecordScreen recordScreen;
        [SerializeField] private Button recordButton;
        [SerializeField] private Text recordButtonLabel;

        [Header("Visual (fase 8)")]
        [Tooltip("Segmentos da barra de progresso: um por etapa, criados a partir do modelo")]
        [SerializeField] private Transform progressContainer;
        [SerializeField] private Image progressTemplate;
        [Tooltip("Bolinha do selo de status no topo")]
        [SerializeField] private Image scanHintDot;

        private const string ScanningMessage = "Aponte a câmera para o marcador da placa";

        private static readonly Color ActiveTab = new Color32(0x25, 0x63, 0xEB, 0xFF);
        private static readonly Color InactiveTab = new Color32(0x1B, 0x2A, 0x4A, 0xFF);
        private static readonly Color ProgressDone = new Color32(0x3B, 0x82, 0xF6, 0xFF);
        private static readonly Color ProgressTodo = new Color(1f, 1f, 1f, 0.25f);
        private static readonly Color SearchingDot = new Color32(0xFB, 0xBF, 0x24, 0xFF);

        private readonly List<GameObject> spawned = new();
        private readonly List<Image> progressSegments = new();
        private InspectionStep currentStep;
        private bool guideOpen;

        private void Awake()
        {
            cardTemplate.gameObject.SetActive(false);
            backButton.onClick.AddListener(flow.Previous);
            // O fluxo bloqueia se o registro estiver incompleto e a RecordScreen abre sozinha.
            nextButton.onClick.AddListener(flow.Next);

            if (progressTemplate != null)
                progressTemplate.gameObject.SetActive(false);

            if (guideTab != null)
                guideTab.onClick.AddListener(ToggleGuide);

            if (recordButton != null)
                recordButton.onClick.AddListener(OpenRecord);
        }

        private void OnEnable()
        {
            flow.StepChanged += Render;
            flow.Finished += HandleFinished;
            if (recordScreen != null)
                recordScreen.Closed += UpdateRecordButton;
            ShowHint(ScanningMessage);
        }

        private void OnDisable()
        {
            flow.StepChanged -= Render;
            flow.Finished -= HandleFinished;
            if (recordScreen != null)
                recordScreen.Closed -= UpdateRecordButton;
        }

        private void Render(InspectionStep step, int index, int count)
        {
            currentStep = step;
            scanHint.SetActive(false);
            panelRoot.SetActive(true);

            stepLabel.text = $"Etapa {index + 1} de {count} · {step.RequirementId}";
            titleText.text = step.Title;
            instructionText.text = step.Instruction;
            backButton.interactable = index > 0;
            nextLabel.text = index == count - 1 ? "Concluir" : "Próximo";

            RenderProgress(index, count);
            ClearSpawned();
            RenderCards(step.Cards);

            // As abas ficam sempre visíveis: a Guia é o único jeito de abrir a instrução.
            if (tabsRoot != null)
                tabsRoot.SetActive(true);

            UpdateRecordButton();
            SetGuideOpen(false);
        }

        private void RenderProgress(int index, int count)
        {
            if (progressTemplate == null)
                return;

            while (progressSegments.Count < count)
            {
                Image segment = Instantiate(progressTemplate, progressContainer);
                segment.gameObject.SetActive(true);
                progressSegments.Add(segment);
            }

            for (int i = 0; i < progressSegments.Count; i++)
            {
                progressSegments[i].gameObject.SetActive(i < count);
                progressSegments[i].color = i <= index ? ProgressDone : ProgressTodo;
            }
        }

        private void RenderCards(IReadOnlyList<InfoCard> cards)
        {
            foreach (InfoCard card in cards)
            {
                InfoCardView view = Spawn(cardTemplate, cardsContainer);
                view.Show(card);
            }
            cardsContainer.gameObject.SetActive(cards.Count > 0);
        }

        /// <summary>Tocar em Guia abre a instrução e os cards; tocar de novo recolhe.</summary>
        private void ToggleGuide()
        {
            SetGuideOpen(!guideOpen);
        }

        /// <summary>
        /// Minimizado: só etapa, título, abas e botões (a placa fica à vista).
        /// Guia aberta: instrução e cards.
        /// </summary>
        private void SetGuideOpen(bool open)
        {
            guideOpen = open;

            // Cena antiga sem Grupo Guia: a instrução fica sempre visível.
            if (guideGroup != null)
                guideGroup.SetActive(open);

            if (guideTab != null && guideTab.targetGraphic is Image tabImage)
                tabImage.color = open ? ActiveTab : InactiveTab;
        }

        private void OpenRecord()
        {
            if (recordScreen != null && currentStep != null)
                recordScreen.Open(currentStep);
        }

        /// <summary>
        /// Registrar só aparece em etapas com checklist ou medições; o texto mostra
        /// quantos itens faltam. Atualiza ao renderizar e quando a RecordScreen fecha.
        /// </summary>
        private void UpdateRecordButton()
        {
            if (recordButton == null || currentStep == null)
                return;

            bool hasRecord = currentStep.HasRecord && recordScreen != null;
            recordButton.gameObject.SetActive(hasRecord);
            if (!hasRecord || recordButtonLabel == null)
                return;

            int pending = flow.Session.PendingCount(currentStep);
            recordButtonLabel.text = pending > 0
                ? $"Registrar ({pending} pendentes)"
                : "Registro completo";
        }

        private T Spawn<T>(T template, Transform parent) where T : Component
        {
            T view = Instantiate(template, parent);
            view.gameObject.SetActive(true);
            spawned.Add(view.gameObject);
            return view;
        }

        private void ClearSpawned()
        {
            foreach (GameObject go in spawned)
                Destroy(go);
            spawned.Clear();
        }

        private void HandleFinished()
        {
            // Depois das etapas vem o resultado (tela própria).
            panelRoot.SetActive(false);
            scanHint.SetActive(false);
        }

        private void ShowHint(string message)
        {
            panelRoot.SetActive(false);
            scanHintText.text = message;
            if (scanHintDot != null)
                scanHintDot.color = SearchingDot;
            scanHint.SetActive(true);
        }
    }
}
