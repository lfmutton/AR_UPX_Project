using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>
    /// Painel inferior da etapa atual. Só mostra o estado do InspectionFlow e
    /// repassa os cliques de Voltar/Próximo — não decide nada.
    /// </summary>
    public class StepPanel : MonoBehaviour
    {
        [SerializeField] private InspectionFlow flow;

        [Header("Aviso de busca")]
        [SerializeField] private GameObject scanHint;
        [SerializeField] private Text scanHintText;

        [Header("Painel da etapa")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text stepLabel;
        [SerializeField] private Text titleText;
        [SerializeField] private Text instructionText;
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private InfoCardView cardTemplate;
        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Text nextLabel;

        private const string ScanningMessage = "Aponte a câmera para o marcador da placa";
        private const string FinishedMessage = "Roteiro concluído";

        private readonly List<InfoCardView> spawnedCards = new();

        private void Awake()
        {
            cardTemplate.gameObject.SetActive(false);
            backButton.onClick.AddListener(flow.Previous);
            nextButton.onClick.AddListener(flow.Next);
        }

        private void OnEnable()
        {
            flow.StepChanged += Render;
            flow.Finished += HandleFinished;
            ShowHint(ScanningMessage);
        }

        private void OnDisable()
        {
            flow.StepChanged -= Render;
            flow.Finished -= HandleFinished;
        }

        private void Render(InspectionStep step, int index, int count)
        {
            scanHint.SetActive(false);
            panelRoot.SetActive(true);

            stepLabel.text = $"Etapa {index + 1} de {count} · {step.RequirementId}";
            titleText.text = step.Title;
            instructionText.text = step.Instruction;
            backButton.interactable = index > 0;
            nextLabel.text = index == count - 1 ? "Concluir" : "Próximo";

            RenderCards(step.Cards);
        }

        private void RenderCards(IReadOnlyList<InfoCard> cards)
        {
            foreach (InfoCardView view in spawnedCards)
                Destroy(view.gameObject);
            spawnedCards.Clear();

            foreach (InfoCard card in cards)
            {
                InfoCardView view = Instantiate(cardTemplate, cardsContainer);
                view.gameObject.SetActive(true);
                view.Show(card);
                spawnedCards.Add(view);
            }

            cardsContainer.gameObject.SetActive(cards.Count > 0);
        }

        private void HandleFinished()
        {
            ShowHint(FinishedMessage);
        }

        private void ShowHint(string message)
        {
            panelRoot.SetActive(false);
            scanHintText.text = message;
            scanHint.SetActive(true);
        }
    }
}
