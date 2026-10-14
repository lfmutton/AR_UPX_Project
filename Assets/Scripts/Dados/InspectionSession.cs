using System;
using System.Collections.Generic;

namespace Facens.WL01
{
    /// <summary>
    /// Registro de uma execução do roteiro: checklist marcado e medições digitadas
    /// por etapa (RF06, RF07) e o tempo gasto em cada uma (RNF09).
    /// Os tempos vêm de fora (Time.realtimeSinceStartup) para manter a classe sem Unity.
    /// </summary>
    public class InspectionSession
    {
        public class StepRecord
        {
            public bool[] checks;
            public string[] values;
            public float seconds;
        }

        private readonly Dictionary<InspectionStep, StepRecord> records = new();
        private readonly float startedRealtime;
        private InspectionStep activeStep;
        private float activeSince;

        public DateTime StartedAt { get; } = DateTime.Now;
        public DateTime EndedAt { get; private set; }
        public float TotalSeconds { get; private set; }

        public InspectionSession(float now = 0f)
        {
            startedRealtime = now;
        }

        public StepRecord GetRecord(InspectionStep step)
        {
            if (!records.TryGetValue(step, out StepRecord record))
            {
                record = new StepRecord
                {
                    checks = new bool[step.Checklist.Count],
                    values = new string[step.Measurements.Count]
                };
                records.Add(step, record);
            }
            return record;
        }

        public void SetChecked(InspectionStep step, int index, bool isChecked)
        {
            GetRecord(step).checks[index] = isChecked;
        }

        public void SetValue(InspectionStep step, int index, string value)
        {
            GetRecord(step).values[index] = value;
        }

        public int CheckedCount(InspectionStep step)
        {
            int count = 0;
            foreach (bool check in GetRecord(step).checks)
                if (check)
                    count++;
            return count;
        }

        /// <summary>Começa a contar o tempo de uma etapa (fecha a anterior).</summary>
        public void EnterStep(InspectionStep step, float now)
        {
            CloseActiveStep(now);
            activeStep = step;
            activeSince = now;
        }

        public void EndSteps(float now)
        {
            CloseActiveStep(now);
            activeStep = null;
        }

        public void Finish(float now)
        {
            EndSteps(now);
            EndedAt = DateTime.Now;
            TotalSeconds = now - startedRealtime;
        }

        /// <summary>Aprovada = todo o checklist marcado e nenhuma medição fora da faixa ou inválida.</summary>
        public bool IsApproved(InspectionStep step)
        {
            StepRecord record = GetRecord(step);
            foreach (bool check in record.checks)
                if (!check)
                    return false;

            for (int i = 0; i < step.Measurements.Count; i++)
            {
                MeasurementResult result = step.Measurements[i].Evaluate(record.values[i]);
                if (result == MeasurementResult.Fora || result == MeasurementResult.Invalido)
                    return false;
            }
            return true;
        }

        private void CloseActiveStep(float now)
        {
            if (activeStep != null)
                GetRecord(activeStep).seconds += now - activeSince;
        }
    }
}
