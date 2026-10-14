using System.Collections.Generic;

namespace Facens.WL01
{
    /// <summary>
    /// Uma aplicação do quiz: junta as perguntas de todas as etapas e guarda as respostas.
    /// </summary>
    public class QuizRun
    {
        public readonly struct Item
        {
            public readonly InspectionStep step;
            public readonly QuizQuestion question;

            public Item(InspectionStep step, QuizQuestion question)
            {
                this.step = step;
                this.question = question;
            }
        }

        private readonly List<Item> items = new();
        private readonly List<int> answers = new();

        public int Count => items.Count;
        public int CurrentIndex { get; private set; }
        public Item Current => items[CurrentIndex];
        public bool IsLast => CurrentIndex == items.Count - 1;
        public IReadOnlyList<Item> Items => items;

        public static QuizRun FromSteps(IEnumerable<InspectionStep> steps)
        {
            var run = new QuizRun();
            foreach (InspectionStep step in steps)
            {
                foreach (QuizQuestion question in step.Quiz)
                {
                    run.items.Add(new Item(step, question));
                    run.answers.Add(-1);
                }
            }
            return run;
        }

        /// <summary>Registra a resposta da pergunta atual e diz se está correta.</summary>
        public bool Answer(int option)
        {
            answers[CurrentIndex] = option;
            return option == Current.question.correctIndex;
        }

        public bool MoveNext()
        {
            if (CurrentIndex >= items.Count - 1)
                return false;
            CurrentIndex++;
            return true;
        }

        public int AnswerAt(int index) => answers[index];

        public int CorrectCount
        {
            get
            {
                int correct = 0;
                for (int i = 0; i < items.Count; i++)
                    if (answers[i] == items[i].question.correctIndex)
                        correct++;
                return correct;
            }
        }
    }
}
