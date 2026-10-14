using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Facens.WL01
{
    /// <summary>
    /// Controla a sequência de etapas do roteiro (RF05): começa quando a placa é
    /// reconhecida e avança/volta entre as etapas. A UI só escuta os eventos.
    /// </summary>
    public class InspectionFlow : MonoBehaviour
    {
        [SerializeField] private ImageRecognitionController recognition;
        [SerializeField] private List<InspectionStep> steps = new();

        public event Action<InspectionStep, int, int> StepChanged;
        public event Action Finished;

        public int CurrentIndex { get; private set; } = -1;
        public int StepCount => steps.Count;
        public bool IsRunning => CurrentIndex >= 0;
        public bool IsFinished { get; private set; }
        public InspectionStep CurrentStep => IsRunning ? steps[CurrentIndex] : null;

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
            if (!IsRunning && !IsFinished)
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
            GoTo(0);
        }

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
            Finished?.Invoke();
        }

        public void Previous()
        {
            if (IsRunning && CurrentIndex > 0)
                GoTo(CurrentIndex - 1);
        }

        private void GoTo(int index)
        {
            CurrentIndex = index;
            StepChanged?.Invoke(steps[index], index, steps.Count);
        }
    }
}
