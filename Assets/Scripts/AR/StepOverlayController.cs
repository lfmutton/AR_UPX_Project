using System.Collections.Generic;
using UnityEngine;

namespace Facens.WL01
{
    /// <summary>
    /// Liga o roteiro ao AR: quando o conteúdo é criado sobre o marcador e quando a
    /// etapa muda, atualiza as marcações exibidas sobre a placa.
    /// </summary>
    public class StepOverlayController : MonoBehaviour
    {
        [SerializeField] private ImageRecognitionController recognition;
        [SerializeField] private InspectionFlow flow;

        private readonly List<StepOverlay> overlays = new();

        private void OnEnable()
        {
            recognition.ContentCreated += HandleContentCreated;
            flow.StepChanged += HandleStepChanged;
            flow.Finished += HandleFinished;
        }

        private void OnDisable()
        {
            recognition.ContentCreated -= HandleContentCreated;
            flow.StepChanged -= HandleStepChanged;
            flow.Finished -= HandleFinished;
        }

        private void HandleContentCreated(GameObject content)
        {
            if (!content.TryGetComponent(out StepOverlay overlay))
                return;

            overlays.Add(overlay);
            if (flow.CurrentStep != null)
                overlay.Show(flow.CurrentStep.Highlights);
        }

        private void HandleStepChanged(InspectionStep step, int index, int count)
        {
            overlays.RemoveAll(o => o == null);
            foreach (StepOverlay overlay in overlays)
                overlay.Show(step.Highlights);
        }

        private void HandleFinished()
        {
            overlays.RemoveAll(o => o == null);
            foreach (StepOverlay overlay in overlays)
                overlay.Clear();
        }
    }
}
