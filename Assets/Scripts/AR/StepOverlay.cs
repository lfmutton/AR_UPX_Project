using System.Collections.Generic;
using UnityEngine;

namespace Facens.WL01
{
    /// <summary>
    /// Conteúdo instanciado sobre o marcador: um Canvas deitado no plano da imagem
    /// que desenha as marcações da etapa atual.
    /// </summary>
    public class StepOverlay : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private HighlightView template;

        private readonly List<HighlightView> spawned = new();

        private void Awake()
        {
            template.gameObject.SetActive(false);
        }

        public void Show(IReadOnlyList<Highlight> highlights)
        {
            Clear();
            foreach (Highlight highlight in highlights)
            {
                HighlightView view = Instantiate(template, container);
                view.gameObject.SetActive(true);
                view.Show(highlight);
                spawned.Add(view);
            }
        }

        public void Clear()
        {
            foreach (HighlightView view in spawned)
                Destroy(view.gameObject);
            spawned.Clear();
        }
    }
}
