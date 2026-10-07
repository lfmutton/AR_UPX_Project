using UnityEngine;

namespace Facens.WL01
{
    /// <summary>
    /// Interacao minima do white label: toque no objeto para alternar a escala.
    /// </summary>
    public class TapToToggleScale : MonoBehaviour
    {
        [SerializeField] private float scaleMultiplier = 1.5f;

        private Vector3 initialScale;
        private bool expanded;

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        private void Update()
        {
            if (Input.touchCount != 1)
                return;

            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;

            Camera cameraToUse = Camera.main;
            if (cameraToUse == null)
                return;

            Ray ray = cameraToUse.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                expanded = !expanded;
                transform.localScale = expanded ? initialScale * scaleMultiplier : initialScale;
            }
        }
    }
}
