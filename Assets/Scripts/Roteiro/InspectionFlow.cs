using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Facens.WL01
{
    /// <summary>
    /// Controla o roteiro: etapas (RF05) → quiz de fixação → resultado com métricas (RNF09).
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
        /// <summary>Disparado ao concluir a última etapa (antes do quiz).</summary>
        public event Action Finished;
        public event Action<QuizRun> QuizStarted;
        /// <summary>Disparado ao fim de tudo, com o relatório já salvo.</summary>
        public event Action<SessionReport> Completed;

        public int CurrentIndex { get; private set; } = -1;
        public int StepCount => steps.Count;
        public bool IsRunning => CurrentIndex >= 0;
        public bool IsFinished { get; private set; }
        public InspectionStep CurrentStep => IsRunning ? steps[CurrentIndex] : null;
        public InspectionSession Session { get; private set; } = new();
        public QuizRun Quiz { get; private set; }
        public string LastReportPath { get; private set; }

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
            Quiz = null;
            Session = new InspectionSession(Now);
            GoTo(0);
        }

        /// <summary>Recomeça o roteiro do zero (botão "Refazer roteiro").</summary>
        public void Restart() => Begin();

        public void Next()
        {
            if (!IsRunning)
                return;

            if (CurrentIndex < steps.Count - 1)
            {
                GoTo(CurrentIndex + 1);
                return;
            }

            CurrentIndex = -1;
            IsFinished = true;
            Session.EndSteps(Now);
            Finished?.Invoke();
            StartQuiz();
        }

        public void Previous()
        {
            if (IsRunning && CurrentIndex > 0)
                GoTo(CurrentIndex - 1);
        }

        /// <summary>Vai para a próxima pergunta; ao fim do quiz, gera o resultado. Retorna true se ainda há perguntas.</summary>
        public bool AdvanceQuiz()
        {
            if (Quiz != null && Quiz.MoveNext())
                return true;

            Complete();
            return false;
        }

        private void StartQuiz()
        {
            Quiz = QuizRun.FromSteps(steps);
            if (Quiz.Count == 0)
            {
                Complete();
                return;
            }
            QuizStarted?.Invoke(Quiz);
        }

        private void Complete()
        {
            Session.Finish(Now);
            SessionReport report = MetricsRecorder.Build(steps, Session, Quiz);
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
