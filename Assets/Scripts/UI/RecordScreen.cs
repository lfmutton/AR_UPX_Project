using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>
    /// Tela cheia de registro da etapa: checklist (RF06) e medições (RF07).
    /// Abre quando o InspectionFlow avisa que o registro está incompleto
    /// (RecordIncomplete). Só mostra o estado da InspectionSession e repassa
    /// as mudanças — quem decide se dá para avançar é o InspectionFlow.
    /// </summary>
    public class RecordScreen : MonoBehaviour
    {
        [SerializeField] InspectionFlow flow;
        [SerializeField] GameObject root;
        [SerializeField] Text stepLabel;
        [SerializeField] Text titleText;
        [SerializeField] Transform checklistContainer;
        [SerializeField] ChecklistItemView checklistTemplate;
        [SerializeField] Transform measurementsContainer;
        [SerializeField] MeasurementFieldView measurementTemplate;
        [SerializeField] Text pendingText;
        [SerializeField] Button doneButton;
        [SerializeField] Text doneLabel;
        [SerializeField] Button closeButton;

        private static readonly Color PendingColor = new Color32(0xFC, 0xD3, 0x4D, 0xFF);
        private static readonly Color CompleteColor = new Color32(0x86, 0xEF, 0xAC, 0xFF);

        private readonly List<GameObject> spawned = new();
        private InspectionStep currentStep;

        /// <summary>Disparado sempre que a tela é fechada (concluída ou não).</summary>
        public event Action Closed;

        public bool IsOpen => root.activeSelf;

        private void Awake()
        {
            checklistTemplate.gameObject.SetActive(false);
            measurementTemplate.gameObject.SetActive(false);
            root.SetActive(false);

            doneButton.onClick.AddListener(HandleDone);
            closeButton.onClick.AddListener(Close);
        }

        private void OnEnable()
        {
            flow.RecordIncomplete += Open;
            flow.StepChanged += HandleStepChanged;
            flow.Finished += Close;
        }

        private void OnDisable()
        {
            flow.RecordIncomplete -= Open;
            flow.StepChanged -= HandleStepChanged;
            flow.Finished -= Close;
        }

        /// <summary>Abre a tela de registro da etapa, lendo o que já foi preenchido na sessão.</summary>
        public void Open(InspectionStep step)
        {
            if (step == null)
                return;

            currentStep = step;
            root.SetActive(true);

            stepLabel.text = $"Etapa {flow.CurrentIndex + 1} de {flow.StepCount} · {step.RequirementId}";
            titleText.text = $"Registro — {step.Title}";

            ClearSpawned();
            RenderRecord(step);
            UpdatePending();
        }

        /// <summary>Fecha a tela sem avançar. Só avisa (Closed) se ela estava aberta.</summary>
        public void Close()
        {
            if (!root.activeSelf)
                return;

            root.SetActive(false);
            Closed?.Invoke();
        }

        private void HandleStepChanged(InspectionStep step, int index, int count) => Close();

        private void RenderRecord(InspectionStep step)
        {
            InspectionSession.StepRecord record = flow.Session.GetRecord(step);

            for (int i = 0; i < step.Checklist.Count; i++)
            {
                int itemIndex = i;
                ChecklistItemView view = Spawn(checklistTemplate, checklistContainer);
                view.Bind(step.Checklist[i], record.checks[i], isChecked =>
                {
                    flow.Session.SetChecked(step, itemIndex, isChecked);
                    UpdatePending();
                });
            }
            checklistContainer.gameObject.SetActive(step.Checklist.Count > 0);

            for (int i = 0; i < step.Measurements.Count; i++)
            {
                int fieldIndex = i;
                MeasurementFieldView view = Spawn(measurementTemplate, measurementsContainer);
                view.Bind(step.Measurements[i], record.values[i], value =>
                {
                    flow.Session.SetValue(step, fieldIndex, value);
                    UpdatePending();
                });
            }
            measurementsContainer.gameObject.SetActive(step.Measurements.Count > 0);
        }

        /// <summary>Atualiza o aviso de pendências e o texto do botão principal.</summary>
        private void UpdatePending()
        {
            if (currentStep == null)
                return;

            bool complete = flow.Session.IsRecordComplete(currentStep);
            if (complete)
            {
                pendingText.text = "Registro completo";
                pendingText.color = CompleteColor;
                doneLabel.text = "Salvar e continuar";
            }
            else
            {
                int pending = flow.Session.PendingCount(currentStep);
                pendingText.text = pending == 1
                    ? "Falta 1 item para concluir"
                    : $"Faltam {pending} itens para concluir";
                pendingText.color = PendingColor;
                doneLabel.text = "Salvar e voltar";
            }
        }

        /// <summary>Completo: fecha e avança direto. Incompleto: só fecha e volta ao painel.</summary>
        private void HandleDone()
        {
            bool complete = currentStep != null && flow.Session.IsRecordComplete(currentStep);
            Close();
            if (complete)
                flow.Next();
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
    }
}
