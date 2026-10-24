using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Facens.WL01
{
    /// <summary>
    /// Controla o roteiro: etapas com registro obrigatório (RF05–RF07) → resultado com métricas (RNF09).
    /// Só avança de etapa com o checklist e as medições completos; voltar é sempre livre.
    /// Começa quando a placa é reconhecida. A UI só escuta os eventos e chama os métodos públicos.
    /// </summary>
    public class InspectionFlow : MonoBehaviour
    {
        [SerializeField] private ImageRecognitionController recognition;
        [SerializeField] private List<InspectionStep> steps = new();
        [Tooltip("Se marcado, o roteiro só começa depois de RequestStart() (botão Iniciar RA da tela inicial)")]
        [SerializeField] private bool waitForStart;

        private bool startRequested;

        public event Action<InspectionStep, int, int> StepChanged;
        /// <summary>Disparado ao tentar avançar com o registro da etapa incompleto.</summary>
        public event Action<InspectionStep> RecordIncomplete;
        /// <summary>Disparado ao concluir a última etapa (antes do resultado).</summary>
        public event Action Finished;
        /// <summary>Disparado ao fim de tudo, com o relatório já salvo.</summary>
        public event Action<SessionReport> Completed;

        public int CurrentIndex { get; private set; } = -1;
        public int StepCount => steps.Count;
        public bool IsRunning => CurrentIndex >= 0;
        public bool IsFinished { get; private set; }
        public InspectionStep CurrentStep => IsRunning ? steps[CurrentIndex] : null;
        public InspectionSession Session { get; private set; } = new();
        public string LastReportPath { get; private set; }
        /// <summary>True quando o registro da etapa atual está completo e o botão Próximo pode avançar.</summary>
        public bool CanAdvance => IsRunning && Session.IsRecordComplete(CurrentStep);

        private static float Now => Time.realtimeSinceStartup;

        private void OnEnable()
        {
            if (recognition != null)
                recognition.MarkerDetected += HandleMarkerDetected;
        }

        private void OnDisable()
        {
            if (recognition != null)
                recognition.MarkerDetected -= HandleMarkerDetected;
        }

        private void HandleMarkerDetected(ARTrackedImage trackedImage)
        {
            if (waitForStart && !startRequested)
                return;
            if (!IsRunning && !IsFinished)
                Begin();
        }

        /// <summary>Chamado pelo botão "Iniciar RA": começa já se a placa estiver na tela, senão espera reconhecê-la.</summary>
        public void RequestStart()
        {
            startRequested = true;
            if (recognition != null && recognition.HasVisibleMarker && !IsRunning)
                Begin();
        }

        public void Begin()
        {
            if (steps.Count == 0)
            {
                Debug.LogWarning("InspectionFlow: nenhuma etapa configurada.");
                return;
            }

            IsFinished = false;
            Session = new InspectionSession(Now);
            GoTo(0);
        }

        /// <summary>Recomeça o roteiro do zero (botão "Refazer roteiro").</summary>
        public void Restart() => Begin();

        /// <summary>Avança se o registro da etapa estiver completo; na última etapa, vai direto para o resultado.</summary>
        public void Next()
        {
            if (!IsRunning)
                return;

            if (!Session.IsRecordComplete(CurrentStep))
            {
                RecordIncomplete?.Invoke(CurrentStep);
                return;
            }

            if (CurrentIndex < steps.Count - 1)
            {
                GoTo(CurrentIndex + 1);
                return;
            }

            CurrentIndex = -1;
            IsFinished = true;
            Session.EndSteps(Now);
            Finished?.Invoke();
            Complete();
        }

        public void Previous()
        {
            if (IsRunning && CurrentIndex > 0)
                GoTo(CurrentIndex - 1);
        }

        private void Complete()
        {
            Session.Finish(Now);
            SessionReport report = MetricsRecorder.Build(steps, Session);
            LastReportPath = MetricsRecorder.Save(report);
            Completed?.Invoke(report);
        }

        private void GoTo(int index)
        {
            CurrentIndex = index;
            Session.EnterStep(steps[index], Now);
            StepChanged?.Invoke(steps[index], index, steps.Count);
        }
    }
}
