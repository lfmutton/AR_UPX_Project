using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facens.WL01
{
    /// <summary>Pergunta de fixação ligada a uma etapa do roteiro.</summary>
    [Serializable]
    public class QuizQuestion
    {
        [TextArea(2, 4)] public string question;
        public List<string> options = new();
        [Tooltip("Índice da alternativa correta (0 = primeira)")]
        public int correctIndex;
    }
}
