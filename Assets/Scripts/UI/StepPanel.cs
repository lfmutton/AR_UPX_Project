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

        private const string ScanningMessage = "Aponte a câmera para o marcador da placa";

        private static readonly Color ActiveTab = new Color32(0x25, 0x63, 0xEB, 0xFF);
        private static readonly Color InactiveTab = new Color32(0x1B, 0x2A, 0x4A, 0xFF);

        private readonly List<GameObject> spawned = new();
        private InspectionStep currentStep;

        private bool HasRecordUi => recordGroup != null;

        private void Awake()
        {
            cardTemplate.gameObject.SetActive(false);
            backButton.onClick.AddListener(flow.Previous);
            nextButton.onClick.AddListener(flow.Next);

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

            ClearSpawned();
            RenderCards(step.Cards);

            if (HasRecordUi)
            {
                tabsRoot.SetActive(step.HasRecord);
                RenderRecord(step);
                ShowTab(false);
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
            recordGroup.SetActive(record);
            ((Image)guideTab.targetGraphic).color = record ? InactiveTab : ActiveTab;
            ((Image)recordTab.targetGraphic).color = record ? ActiveTab : InactiveTab;
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
            scanHint.SetActive(true);
        }
    }
}
