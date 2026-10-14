using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>Exibe um InfoCard (título + texto) dentro do painel da etapa.</summary>
    public class InfoCardView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text bodyText;

        public void Show(InfoCard card)
        {
            titleText.text = card.title;
            bodyText.text = card.text;
        }
    }
}
