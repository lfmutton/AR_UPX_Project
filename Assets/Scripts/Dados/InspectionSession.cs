using System.Collections.Generic;

namespace Facens.WL01
{
    /// <summary>
    /// Registro de uma execução do roteiro: checklist marcado e medições digitadas
    /// por etapa (RF06, RF07). Base para as métricas da fase 5.
    /// </summary>
    public class InspectionSession
    {
        public class StepRecord
        {
            public bool[] checks;
            public string[] values;
        }

        private readonly Dictionary<InspectionStep, StepRecord> records = new();

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
    }
}
