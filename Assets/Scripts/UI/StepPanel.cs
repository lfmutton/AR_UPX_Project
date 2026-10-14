using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>
    /// Painel inferior da etapa atual. Só mostra o estado do InspectionFlow e
    /// repassa os cliques — não decide nada. Duas abas: Guia (instrução e cards)
    /// e Registro (checklist RF06 e medições RF07).
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

        [Header("Abas (fase 4)")]
        [SerializeField] private GameObject tabsRoot;
        [SerializeField] private Button guideTab;
        [SerializeField] private Button recordTab;
        [SerializeField] private Text recordTabLabel;
        [SerializeField] private GameObject guideGroup;
        [SerializeField] private GameObject recordGroup;

        [Header("Registro (fase 4)")]
        [SerializeField] private Transform checklistContainer;
        [SerializeField] private ChecklistItemView checklistTemplate;
        [SerializeField] private Transform measurementsContainer;
        [SerializeField] private MeasurementFieldView measurementTemplate;

        [Header("Visual (fase 8)")]
        [Tooltip("Segmentos da barra de progresso: um por etapa, criados a partir do modelo")]
        [SerializeField] private Transform progressContainer;
        [SerializeField] private Image progressTemplate;
        [Tooltip("Bolinha do selo de status no topo")]
        [SerializeField] private Image scanHintDot;

        [Header("Registro em tela cheia (fase 8)")]
        [Tooltip("Área com rolagem que contém o Grupo Registro. Se definida, é ela que aparece/some na aba Registro")]
        [SerializeField] private GameObject recordRoot;
        [Tooltip("Até que altura da tela (0–1) o painel sobe na aba Registro; acima disso fica a faixa da câmera")]
        [Range(0.5f, 1f)]
        [SerializeField] private float recordPanelTop = 0.8f;

        private const string ScanningMessage = "Aponte a câmera para o marcador da placa";

        private static readonly Color ActiveTab = new Color32(0x25, 0x63, 0xEB, 0xFF);
        private static readonly Color InactiveTab = new Color32(0x1B, 0x2A, 0x4A, 0xFF);
        private static readonly Color ProgressDone = new Color32(0x3B, 0x82, 0xF6, 0xFF);
        private static readonly Color ProgressTodo = new Color(1f, 1f, 1f, 0.25f);
        private static readonly Color SearchingDot = new Color32(0xFB, 0xBF, 0x24, 0xFF);

        private readonly List<GameObject> spawned = new();
        private readonly List<Image> progressSegments = new();
        private InspectionStep currentStep;

        private bool HasRecordUi => recordGroup != null;

        private RectTransform panelRect;
        private ContentSizeFitter panelFitter;
        private Vector2 compactAnchorMin, compactAnchorMax, compactPivot, compactPosition, compactSize;

        private void Awake()
        {
            panelRect = (RectTransform)panelRoot.transform;
            panelFitter = panelRoot.GetComponent<ContentSizeFitter>();
            compactAnchorMin = panelRect.anchorMin;
            compactAnchorMax = panelRect.anchorMax;
            compactPivot = panelRect.pivot;
            compactPosition = panelRect.anchoredPosition;
            compactSize = panelRect.sizeDelta;

            if (recordRoot != null)
                recordGroup.SetActive(true);

            cardTemplate.gameObject.SetActive(false);
            backButton.onClick.AddListener(flow.Previous);
            nextButton.onClick.AddListener(flow.Next);

            if (progressTemplate != null)
                progressTemplate.gameObject.SetActive(false);

            if (HasRecordUi)
            {
                checklistTemplate.gameObject.SetActive(false);
                measurementTemplate.gameObject.SetActive(false);
                guideTab.onClick.AddListener(() => ShowTab(false));
                recordTab.onClick.AddListener(() => ShowTab(true));
            }
        }

        private void OnEnable()
        {
            flow.StepChanged += Render;
            flow.Finished += HandleFinished;
            ShowHint(ScanningMessage);
        }

        private void OnDisable()
        {
            flow.StepChanged -= Render;
            flow.Finished -= HandleFinished;
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

            if (HasRecordUi)
            {
                tabsRoot.SetActive(step.HasRecord);
                RenderRecord(step);
                ShowTab(false);
            }
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

        private void RenderRecord(InspectionStep step)
        {
            InspectionSession session = flow.Session;
            InspectionSession.StepRecord record = session.GetRecord(step);

            for (int i = 0; i < step.Checklist.Count; i++)
            {
                int itemIndex = i;
                ChecklistItemView view = Spawn(checklistTemplate, checklistContainer);
                view.Bind(step.Checklist[i], record.checks[i], isChecked =>
                {
                    session.SetChecked(step, itemIndex, isChecked);
                    UpdateRecordTabLabel();
                });
            }
            checklistContainer.gameObject.SetActive(step.Checklist.Count > 0);

            for (int i = 0; i < step.Measurements.Count; i++)
            {
                int fieldIndex = i;
                MeasurementFieldView view = Spawn(measurementTemplate, measurementsContainer);
                view.Bind(step.Measurements[i], record.values[i],
                    value => session.SetValue(step, fieldIndex, value));
            }
            measurementsContainer.gameObject.SetActive(step.Measurements.Count > 0);

            UpdateRecordTabLabel();
        }

        private void UpdateRecordTabLabel()
        {
            int total = currentStep.Checklist.Count;
            recordTabLabel.text = total > 0
                ? $"Registro ({flow.Session.CheckedCount(currentStep)}/{total})"
                : "Registro";
        }

        private void ShowTab(bool record)
        {
            guideGroup.SetActive(!record);
            (recordRoot != null ? recordRoot : recordGroup).SetActive(record);
            ((Image)guideTab.targetGraphic).color = record ? InactiveTab : ActiveTab;
            ((Image)recordTab.targetGraphic).color = record ? ActiveTab : InactiveTab;
            SetExpanded(record && recordRoot != null);
        }

        /// <summary>
        /// Aba Registro: o painel sobe até recordPanelTop e a câmera fica numa faixa no topo.
        /// Aba Guia: volta ao painel compacto no rodapé, do tamanho do conteúdo.
        /// </summary>
        private void SetExpanded(bool expanded)
        {
            if (panelFitter != null)
                panelFitter.enabled = !expanded;

            if (expanded)
            {
                panelRect.anchorMin = new Vector2(0f, 0f);
                panelRect.anchorMax = new Vector2(1f, recordPanelTop);
                panelRect.pivot = new Vector2(0.5f, 0f);
                panelRect.offsetMin = new Vector2(-compactSize.x / 2f, compactPosition.y);
                panelRect.offsetMax = new Vector2(compactSize.x / 2f, 0f);
            }
            else
            {
                panelRect.anchorMin = compactAnchorMin;
                panelRect.anchorMax = compactAnchorMax;
                panelRect.pivot = compactPivot;
                panelRect.anchoredPosition = compactPosition;
                panelRect.sizeDelta = compactSize;
            }
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
            // Depois das etapas vêm o quiz e o resultado (telas próprias).
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
