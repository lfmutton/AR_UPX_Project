using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01
{
    /// <summary>
    /// Tela de resultado: tempo, erros, acertos do quiz e situação de cada etapa (RNF09).
    /// </summary>
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] private InspectionFlow flow;
        [SerializeField] private GameObject root;
        [SerializeField] private Text timeText;
        [SerializeField] private Text errorsText;
        [SerializeField] private Text quizText;
        [SerializeField] private Transform stepsContainer;
        [SerializeField] private StepResultRowView rowTemplate;
        [SerializeField] private Text savedText;
        [SerializeField] private Button restartButton;

        private readonly List<GameObject> rows = new();

        private void Awake()
        {
            rowTemplate.gameObject.SetActive(false);
            restartButton.onClick.AddListener(HandleRestart);
            root.SetActive(false);
        }

        private void OnEnable()
        {
            flow.Completed += Show;
        }

        private void OnDisable()
        {
            flow.Completed -= Show;
        }

        private void Show(SessionReport report)
        {
            root.SetActive(true);

            int seconds = Mathf.RoundToInt(report.duracaoSegundos);
            timeText.text = $"{seconds / 60:00}:{seconds % 60:00}";
            errorsText.text = report.erros.ToString();
            quizText.text = report.quizTotal > 0 ? $"{report.quizAcertos}/{report.quizTotal}" : "—";

            foreach (GameObject row in rows)
                Destroy(row);
            rows.Clear();

            foreach (StepReport step in report.etapas)
            {
                StepResultRowView view = Instantiate(rowTemplate, stepsContainer);
                view.gameObject.SetActive(true);
                view.Show($"{step.requisito} · {step.etapa}", step.aprovada);
                rows.Add(view.gameObject);
            }

            savedText.text = flow.LastReportPath != null
                ? "Resultado salvo no celular para o acompanhamento do treinamento."
                : "Não foi possível salvar o resultado.";
        }

        private void HandleRestart()
        {
            root.SetActive(false);
            flow.Restart();
        }
    }
}
