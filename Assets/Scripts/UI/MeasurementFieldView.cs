using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>
    /// Campo de medição da etapa (RF07): rótulo, valor digitado, unidade e o
    /// resultado da comparação com a faixa esperada (calculado pelo MeasurementSpec).
    /// </summary>
    public class MeasurementFieldView : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private InputField input;
        [SerializeField] private Text unit;
        [SerializeField] private Text status;

        private static readonly Color Ok = new Color32(0x86, 0xEF, 0xAC, 0xFF);
        private static readonly Color Warning = new Color32(0xFC, 0xD3, 0x4D, 0xFF);
        private static readonly Color Neutral = new Color32(0xA3, 0xA9, 0xB1, 0xFF);

        private MeasurementSpec spec;

        public void Bind(MeasurementSpec measurement, string value, Action<string> onChanged)
        {
            spec = measurement;
            label.text = spec.hasRange
                ? $"{spec.label}  (esperado {Format(spec.min)} a {Format(spec.max)} {spec.unit})"
                : spec.label;
            unit.text = spec.unit;

            input.onValueChanged.RemoveAllListeners();
            input.SetTextWithoutNotify(value ?? string.Empty);
            input.onValueChanged.AddListener(text =>
            {
                onChanged(text);
                UpdateStatus(text);
            });
            UpdateStatus(input.text);
        }

        private void UpdateStatus(string text)
        {
            switch (spec.Evaluate(text))
            {
                case MeasurementResult.Dentro:
                    SetStatus("Dentro do esperado", Ok);
                    break;
                case MeasurementResult.Fora:
                    SetStatus("Fora da faixa esperada", Warning);
                    break;
                case MeasurementResult.Invalido:
                    SetStatus("Valor inválido", Warning);
                    break;
                case MeasurementResult.SemFaixa:
                    SetStatus("Faixa esperada a definir [preencher]", Neutral);
                    break;
                default:
                    SetStatus(string.Empty, Neutral);
                    break;
            }
        }

        private void SetStatus(string text, Color color)
        {
            status.text = text;
            status.color = color;
            status.gameObject.SetActive(text.Length > 0);
        }

        private static string Format(float value) => value.ToString("0.##", CultureInfo.InvariantCulture).Replace('.', ',');
    }
}
