using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>Linha do resultado: nome da etapa + selo Aprovada/Revisar.</summary>
    public class StepResultRowView : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Image badge;
        [SerializeField] private Text badgeText;

        public void Show(string stepName, bool approved)
        {
            nameText.text = stepName;
            badgeText.text = approved ? "Aprovada" : "Revisar";
            badge.color = approved ? new Color32(0x10, 0x26, 0x1C, 0xFF) : new Color32(0x3A, 0x2E, 0x0E, 0xFF);
            badgeText.color = approved ? new Color32(0x86, 0xEF, 0xAC, 0xFF) : new Color32(0xFC, 0xD3, 0x4D, 0xFF);
        }
    }
}
