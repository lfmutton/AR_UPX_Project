using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facens.WL01
{
    /// <summary>
    /// Bloco de informação exibido no painel da etapa (ex.: "Boa solda" x "Solda fria").
    /// </summary>
    [Serializable]
    public class InfoCard
    {
        public string title;
        [TextArea(2, 4)] public string text;
    }

    public enum HighlightKind
    {
        Info,
        Ok,
        Atencao
    }

    /// <summary>
    /// Marcação exibida sobre a placa: um ponto com rótulo, posicionado em
    /// centímetros a partir do centro do marcador (x = direita, y = para cima da imagem).
    /// </summary>
    [Serializable]
    public class Highlight
    {
        public string label;
        [Tooltip("Posição em cm a partir do centro do marcador")]
        public Vector2 positionCm;
        public HighlightKind kind;
    }

    /// <summary>
    /// Dados de uma etapa do roteiro de inspeção da PCB (RF01–RF04).
    /// Editável no Inspector, sem programar: cada etapa é um asset em Assets/Etapas.
    /// </summary>
    [CreateAssetMenu(fileName = "NovaEtapa", menuName = "B'Energy/Etapa de Inspeção")]
    public class InspectionStep : ScriptableObject
    {
        [SerializeField] private string title;
        [Tooltip("Requisito atendido pela etapa, ex.: RF01")]
        [SerializeField] private string requirementId;
        [TextArea(3, 6)]
        [SerializeField] private string instruction;
        [SerializeField] private List<InfoCard> cards = new();
        [Tooltip("Marcações exibidas sobre a placa nesta etapa")]
        [SerializeField] private List<Highlight> highlights = new();
        [Tooltip("Critérios de aprovação da etapa (RF06)")]
        [SerializeField] private List<string> checklist = new();
        [Tooltip("Medições que o trainee registra nesta etapa (RF07)")]
        [SerializeField] private List<MeasurementSpec> measurements = new();
        [Tooltip("Perguntas de fixação desta etapa, aplicadas no quiz ao final do roteiro")]
        [SerializeField] private List<QuizQuestion> quiz = new();

        public string Title => title;
        public string RequirementId => requirementId;
        public string Instruction => instruction;
        public IReadOnlyList<InfoCard> Cards => cards;
        public IReadOnlyList<Highlight> Highlights => highlights;
        public IReadOnlyList<string> Checklist => checklist;
        public IReadOnlyList<MeasurementSpec> Measurements => measurements;
        public IReadOnlyList<QuizQuestion> Quiz => quiz;
        public bool HasRecord => checklist.Count > 0 || measurements.Count > 0;
    }
}
