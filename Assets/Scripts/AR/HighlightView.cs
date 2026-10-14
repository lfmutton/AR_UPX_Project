using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>Uma marcação sobre a placa: ponto colorido + etiqueta.</summary>
    public class HighlightView : MonoBehaviour
    {
        private const float UnitsPerCm = 100f;

        [SerializeField] private Image dot;
        [SerializeField] private Image labelBackground;
        [SerializeField] private Text label;

        public void Show(Highlight highlight)
        {
            ((RectTransform)transform).anchoredPosition = highlight.positionCm * UnitsPerCm;
            label.text = highlight.label;

            switch (highlight.kind)
            {
                case HighlightKind.Ok:
                    Paint(new Color32(0x25, 0x63, 0xEB, 0xFF), new Color32(0x25, 0x63, 0xEB, 0xFF), Color.white);
                    break;
                case HighlightKind.Atencao:
                    Paint(new Color32(0xFB, 0xBF, 0x24, 0xFF), new Color32(0xFB, 0xBF, 0x24, 0xFF), new Color32(0x11, 0x11, 0x11, 0xFF));
                    break;
                default:
                    Paint(new Color32(0x60, 0xA5, 0xFA, 0xFF), new Color32(0x15, 0x18, 0x1D, 0xF0), Color.white);
                    break;
            }
        }

        private void Paint(Color dotColor, Color backgroundColor, Color textColor)
        {
            dotColor.a = 0.55f;
            dot.color = dotColor;
            labelBackground.color = backgroundColor;
            label.color = textColor;
        }
    }
}
