using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>
    /// Tela do quiz de fixação: mostra a pergunta atual, deixa escolher uma alternativa,
    /// confirma e mostra a correção. As regras (respostas, pontuação) ficam no QuizRun.
    /// </summary>
    public class QuizPanel : MonoBehaviour
    {
        [SerializeField] private InspectionFlow flow;
        [SerializeField] private GameObject root;
        [SerializeField] private Text progressText;
        [SerializeField] private Text questionText;
        [SerializeField] private Transform optionsContainer;
        [SerializeField] private QuizOptionView optionTemplate;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Text confirmLabel;

        private static readonly Color Ok = new Color32(0x86, 0xEF, 0xAC, 0xFF);
        private static readonly Color Warning = new Color32(0xFC, 0xD3, 0x4D, 0xFF);

        private readonly List<QuizOptionView> options = new();
        private QuizRun quiz;
        private int selected = -1;
        private bool answered;

        private void Awake()
        {
            optionTemplate.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(HandleConfirm);
            root.SetActive(false);
        }

        private void OnEnable()
        {
            flow.QuizStarted += Show;
            flow.Completed += HandleCompleted;
        }

        private void OnDisable()
        {
            flow.QuizStarted -= Show;
            flow.Completed -= HandleCompleted;
        }

        private void Show(QuizRun run)
        {
            quiz = run;
            root.SetActive(true);
            RenderQuestion();
        }

        private void RenderQuestion()
        {
            QuizRun.Item item = quiz.Current;
            progressText.text = $"Pergunta {quiz.CurrentIndex + 1} de {quiz.Count} · {item.step.Title}";
            questionText.text = item.question.question;
            feedbackText.text = string.Empty;
            selected = -1;
            answered = false;

            foreach (QuizOptionView view in options)
                Destroy(view.gameObject);
            options.Clear();

            for (int i = 0; i < item.question.options.Count; i++)
            {
                int optionIndex = i;
                QuizOptionView view = Instantiate(optionTemplate, optionsContainer);
                view.gameObject.SetActive(true);
                view.Bind(item.question.options[i]);
                view.Button.onClick.AddListener(() => Select(optionIndex));
                options.Add(view);
            }

            confirmLabel.text = "Confirmar resposta";
            confirmButton.interactable = false;
        }

        private void Select(int index)
        {
            if (answered)
                return;

            selected = index;
            for (int i = 0; i < options.Count; i++)
                options[i].SetState(i == selected ? QuizOptionState.Selecionada : QuizOptionState.Normal);
            confirmButton.interactable = true;
        }

        private void HandleConfirm()
        {
            if (!answered)
            {
                answered = true;
                bool correct = quiz.Answer(selected);
                int correctIndex = quiz.Current.question.correctIndex;

                options[selected].SetState(correct ? QuizOptionState.Correta : QuizOptionState.Errada);
                if (correctIndex >= 0 && correctIndex < options.Count)
                    options[correctIndex].SetState(QuizOptionState.Correta);

                feedbackText.text = correct ? "Resposta correta!" : "Resposta incorreta. A correta está em verde.";
                feedbackText.color = correct ? Ok : Warning;
                confirmLabel.text = quiz.IsLast ? "Ver resultado" : "Próxima pergunta";
                return;
            }

            if (flow.AdvanceQuiz())
                RenderQuestion();
        }

        private void HandleCompleted(SessionReport report)
        {
            root.SetActive(false);
        }
    }
}
