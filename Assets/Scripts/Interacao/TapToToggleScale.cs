using UnityEngine;
using UnityEngine.InputSystem;

namespace Facens.WL01
{
    /// <summary>
    /// Interacao minima do white label: toque no objeto para alternar a escala.
    /// Usa o Input System novo (toque no celular, clique do mouse no Editor).
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
            if (!TryGetTapPosition(out Vector2 screenPosition))
                return;

            Camera cameraToUse = Camera.main;
            if (cameraToUse == null)
                return;

            Ray ray = cameraToUse.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                expanded = !expanded;
                transform.localScale = expanded ? initialScale * scaleMultiplier : initialScale;
            }
        }

        private static bool TryGetTapPosition(out Vector2 position)
        {
            Touchscreen touchscreen = Touchscreen.current;
            if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
            {
                position = touchscreen.primaryTouch.position.ReadValue();
                return true;
            }

            Mouse mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                position = mouse.position.ReadValue();
                return true;
            }

            position = default;
            return false;
        }
    }
}
