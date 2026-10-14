using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>Tela inicial: apresenta o roteiro e libera o início pelo botão "Iniciar RA".</summary>
    public class HomeScreen : MonoBehaviour
    {
        [SerializeField] private InspectionFlow flow;
        [SerializeField] private GameObject root;
        [SerializeField] private Button startButton;

        private void Awake()
        {
            root.SetActive(true);
            startButton.onClick.AddListener(HandleStart);
        }

        private void HandleStart()
        {
            root.SetActive(false);
            flow.RequestStart();
        }
    }
}
