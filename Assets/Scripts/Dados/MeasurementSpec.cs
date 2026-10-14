using System;
using System.Globalization;
using UnityEngine;

namespace Facens.WL01
{
    public enum MeasurementResult
    {
        Vazio,
        Invalido,
        SemFaixa,
        Dentro,
        Fora
    }

    /// <summary>
    /// Medição pedida em uma etapa (RF07), com a faixa esperada opcional.
    /// </summary>
    [Serializable]
    public class MeasurementSpec
    {
        public string label;
        public string unit;
        [Tooltip("Desmarque enquanto a faixa da placa real não estiver definida")]
        public bool hasRange;
        public float min;
        public float max;

        /// <summary>Aceita vírgula ou ponto como separador decimal.</summary>
        public static bool TryParse(string raw, out float value)
        {
            value = 0f;
            if (string.IsNullOrWhiteSpace(raw))
                return false;
            return float.TryParse(raw.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        public MeasurementResult Evaluate(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return MeasurementResult.Vazio;
            if (!TryParse(raw, out float value))
                return MeasurementResult.Invalido;
            if (!hasRange)
                return MeasurementResult.SemFaixa;
            return value >= min && value <= max ? MeasurementResult.Dentro : MeasurementResult.Fora;
        }
    }
}
