using System;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>Um item do checklist da etapa (RF06): caixa de seleção + texto. Fica verde quando marcado.</summary>
    public class ChecklistItemView : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Text label;

        private static readonly Color BoxOff = new Color32(0x1B, 0x2A, 0x4A, 0xFF);
        private static readonly Color BoxOn = new Color32(0x16, 0xA3, 0x4A, 0xFF);
        private static readonly Color LabelOff = new Color32(0xD4, 0xD8, 0xDD, 0xFF);
        private static readonly Color LabelOn = new Color32(0x86, 0xEF, 0xAC, 0xFF);

        private void Awake()
        {
            // O fundo da linha é transparente; sem isto o Unity o ignora no toque
            // e só a caixinha responde. Assim a linha inteira (texto incluso) é clicável.
            if (TryGetComponent(out CanvasRenderer rowRenderer))
                rowRenderer.cullTransparentMesh = false;
        }

        public void Bind(string text, bool isChecked, Action<bool> onChanged)
        {
            label.text = text;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.SetIsOnWithoutNotify(isChecked);
            Paint(isChecked);
            toggle.onValueChanged.AddListener(value =>
            {
                Paint(value);
                onChanged(value);
            });
        }

        private void Paint(bool isChecked)
        {
            toggle.targetGraphic.color = isChecked ? BoxOn : BoxOff;
            toggle.graphic.color = Color.white;
            label.color = isChecked ? LabelOn : LabelOff;
        }
    }
}
