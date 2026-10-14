using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    public enum QuizOptionState
    {
        Normal,
        Selecionada,
        Correta,
        Errada
    }

    /// <summary>Uma alternativa do quiz.</summary>
    public class QuizOptionView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private Text label;

        public Button Button => button;

        public void Bind(string text)
        {
            label.text = text;
            SetState(QuizOptionState.Normal);
        }

        public void SetState(QuizOptionState state)
        {
            switch (state)
            {
                case QuizOptionState.Selecionada:
                    Paint(new Color32(0x1B, 0x2A, 0x4A, 0xFF), Color.white);
                    break;
                case QuizOptionState.Correta:
                    Paint(new Color32(0x14, 0x53, 0x2D, 0xFF), new Color32(0x86, 0xEF, 0xAC, 0xFF));
                    break;
                case QuizOptionState.Errada:
                    Paint(new Color32(0x3A, 0x2E, 0x0E, 0xFF), new Color32(0xFC, 0xD3, 0x4D, 0xFF));
                    break;
                default:
                    Paint(new Color32(0x15, 0x18, 0x1D, 0xFF), new Color32(0xD4, 0xD8, 0xDD, 0xFF));
                    break;
            }
        }

        private void Paint(Color backgroundColor, Color textColor)
        {
            background.color = backgroundColor;
            label.color = textColor;
        }
    }
}
