using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Facens.WL01
{
    // Formato do arquivo salvo (RNF09). Nomes em português para facilitar a análise no artigo.

    [Serializable]
    public class MeasurementReport
    {
        public string medicao;
        public string valor;
        public string unidade;
        public string resultado;
    }

    [Serializable]
    public class StepReport
    {
        public string etapa;
        public string requisito;
        public float duracaoSegundos;
        public int checklistMarcados;
        public int checklistTotal;
        public List<MeasurementReport> medicoes = new();
        public bool aprovada;
    }

    [Serializable]
    public class SessionReport
    {
        public string inicio;
        public string fim;
        public float duracaoSegundos;
        public int erros;
        public List<StepReport> etapas = new();
    }

    /// <summary>
    /// Monta o relatório da sessão e salva em JSON em
    /// Application.persistentDataPath/sessoes (no Android: Android/data/&lt;pacote&gt;/files/sessoes).
    /// Erros = itens do checklist não marcados + medições fora da faixa/inválidas.
    /// </summary>
    public static class MetricsRecorder
    {
        public static SessionReport Build(IReadOnlyList<InspectionStep> steps, InspectionSession session)
        {
            var report = new SessionReport
            {
                inicio = session.StartedAt.ToString("s"),
                fim = session.EndedAt.ToString("s"),
                duracaoSegundos = session.TotalSeconds
            };

            foreach (InspectionStep step in steps)
            {
                InspectionSession.StepRecord record = session.GetRecord(step);
                var stepReport = new StepReport
                {
                    etapa = step.Title,
                    requisito = step.RequirementId,
                    duracaoSegundos = record.seconds,
                    checklistMarcados = session.CheckedCount(step),
                    checklistTotal = step.Checklist.Count,
                    aprovada = session.IsApproved(step)
                };
                report.erros += stepReport.checklistTotal - stepReport.checklistMarcados;

                for (int i = 0; i < step.Measurements.Count; i++)
                {
                    MeasurementSpec spec = step.Measurements[i];
                    MeasurementResult result = spec.Evaluate(record.values[i]);
                    if (result == MeasurementResult.Fora || result == MeasurementResult.Invalido)
                        report.erros++;

                    stepReport.medicoes.Add(new MeasurementReport
                    {
                        medicao = spec.label,
                        valor = record.values[i] ?? string.Empty,
                        unidade = spec.unit,
                        resultado = result.ToString()
                    });
                }

                report.etapas.Add(stepReport);
            }

            return report;
        }

        /// <summary>Salva o relatório e devolve o caminho do arquivo (ou null se falhar).</summary>
        public static string Save(SessionReport report)
        {
            try
            {
                string folder = Path.Combine(Application.persistentDataPath, "sessoes");
                Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, $"sessao_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                File.WriteAllText(path, JsonUtility.ToJson(report, true));
                Debug.Log($"MetricsRecorder: sessão salva em {path}");
                return path;
            }
            catch (Exception e)
            {
                Debug.LogError($"MetricsRecorder: não foi possível salvar a sessão. {e.Message}");
                return null;
            }
        }
    }
}
