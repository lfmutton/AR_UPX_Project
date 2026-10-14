using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01.EditorTools
{
    /// <summary>
    /// Menu "B'Energy > Fase 5 - Criar quiz e resultado": monta as telas de Quiz e Resultado
    /// no Canvas do roteiro e adiciona perguntas de exemplo às etapas que ainda não têm.
    /// </summary>
    public static class QuizSetup
    {
        private static Font font;
        private static Sprite rounded;

        [MenuItem("B'Energy/Fase 5 - Criar quiz e resultado")]
        private static void Run()
        {
            var stepPanel = Object.FindFirstObjectByType<StepPanel>(FindObjectsInactive.Include);
            var flow = Object.FindFirstObjectByType<InspectionFlow>();
            if (stepPanel == null || flow == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Abra a cena WL01RecognitionAR e rode as fases 2 a 4 antes.", "OK");
                return;
            }

            if (Object.FindFirstObjectByType<QuizPanel>(FindObjectsInactive.Include) != null)
            {
                EditorUtility.DisplayDialog("B'Energy", "O quiz e o resultado já existem nesta cena. Nada foi alterado.", "OK");
                return;
            }

            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

            GameObject canvas = stepPanel.gameObject;
            BuildQuiz(canvas, flow);
            BuildResult(canvas, flow);
            FillExampleQuiz(flow);

            EditorSceneManager.MarkSceneDirty(canvas.scene);
            EditorUtility.DisplayDialog("B'Energy",
                "Telas de Quiz e Resultado criadas.\n\nSalve a cena com Ctrl+S.\n\nAs perguntas ficam em cada etapa (Assets/Etapas), campo Quiz.",
                "OK");
        }

        // ---------- Quiz ----------

        private static void BuildQuiz(GameObject canvas, InspectionFlow flow)
        {
            RectTransform content = NewScreen("Tela Quiz", canvas.transform, out GameObject root);

            NewText("Titulo", content, "Quiz de fixação", 56, FontStyle.Bold, Color.white);
            Text progress = NewText("Progresso", content, "Pergunta 1 de 4 · Etapa", 34, FontStyle.Normal, Hex("#A3A9B1"));
            Text question = NewText("Pergunta", content, "Pergunta", 48, FontStyle.Bold, Color.white);

            RectTransform options = NewVertical("Alternativas", content, 20f);
            Image option = NewImage("Alternativa (modelo)", options, Hex("#15181D"));
            var optionLayout = option.gameObject.AddComponent<VerticalLayoutGroup>();
            optionLayout.padding = new RectOffset(40, 40, 34, 34);
            optionLayout.childControlWidth = optionLayout.childControlHeight = true;
            optionLayout.childForceExpandWidth = true;
            optionLayout.childForceExpandHeight = false;
            option.gameObject.AddComponent<LayoutElement>().minHeight = 130f;
            var optionButton = option.gameObject.AddComponent<Button>();
            optionButton.targetGraphic = option;
            optionButton.transition = Selectable.Transition.None;
            Text optionLabel = NewText("Texto", option.transform, "Alternativa", 40, FontStyle.Normal, Hex("#D4D8DD"));
            var optionView = option.gameObject.AddComponent<QuizOptionView>();
            SetRefs(optionView, ("button", optionButton), ("background", option), ("label", optionLabel));

            Text feedback = NewText("Feedback", content, string.Empty, 38, FontStyle.Bold, Hex("#86EFAC"));
            AddSpacer(content);
            Button confirm = NewButton("Botao Confirmar", content, "Confirmar resposta", Hex("#2563EB"), out Text confirmLabel);

            var panel = Undo.AddComponent<QuizPanel>(canvas);
            SetRefs(panel,
                ("flow", flow),
                ("root", root),
                ("progressText", progress),
                ("questionText", question),
                ("optionsContainer", options),
                ("optionTemplate", optionView),
                ("feedbackText", feedback),
                ("confirmButton", confirm),
                ("confirmLabel", confirmLabel));
        }

        // ---------- Resultado ----------

        private static void BuildResult(GameObject canvas, InspectionFlow flow)
        {
            RectTransform content = NewScreen("Tela Resultado", canvas.transform, out GameObject root);

            NewText("Titulo", content, "Roteiro concluído", 60, FontStyle.Bold, Color.white);
            NewText("Subtitulo", content, "Confira o seu desempenho no treinamento.", 36, FontStyle.Normal, Hex("#A3A9B1"));

            RectTransform stats = NewRect("Indicadores", content);
            var statsLayout = stats.gameObject.AddComponent<HorizontalLayoutGroup>();
            statsLayout.spacing = 24f;
            statsLayout.childControlWidth = statsLayout.childControlHeight = true;
            statsLayout.childForceExpandWidth = true;
            statsLayout.childForceExpandHeight = true;
            Text time = NewStat(stats, "Tempo", "00:00");
            Text errors = NewStat(stats, "Erros", "0");
            Text quiz = NewStat(stats, "Quiz", "0/0");

            Image list = NewImage("Etapas", content, Hex("#15181D"));
            var listLayout = list.gameObject.AddComponent<VerticalLayoutGroup>();
            listLayout.padding = new RectOffset(40, 40, 16, 16);
            listLayout.childControlWidth = listLayout.childControlHeight = true;
            listLayout.childForceExpandWidth = true;
            listLayout.childForceExpandHeight = false;

            RectTransform row = NewRect("Linha (modelo)", list.transform);
            var rowLayout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 20f;
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childControlWidth = rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = rowLayout.childForceExpandHeight = false;
            row.gameObject.AddComponent<LayoutElement>().minHeight = 110f;
            Text rowName = NewText("Etapa", row, "RF01 · Etapa", 36, FontStyle.Normal, Color.white);
            rowName.alignment = TextAnchor.MiddleLeft;
            rowName.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            Image badge = NewImage("Selo", row, Hex("#10261C"));
            var badgeLayout = badge.gameObject.AddComponent<HorizontalLayoutGroup>();
            badgeLayout.padding = new RectOffset(26, 26, 12, 12);
            badgeLayout.childControlWidth = badgeLayout.childControlHeight = true;
            Text badgeText = NewText("Texto", badge.transform, "Aprovada", 32, FontStyle.Bold, Hex("#86EFAC"));
            badgeText.horizontalOverflow = HorizontalWrapMode.Overflow;
            var rowView = row.gameObject.AddComponent<StepResultRowView>();
            SetRefs(rowView, ("nameText", rowName), ("badge", badge), ("badgeText", badgeText));

            Text saved = NewText("Salvo", content, string.Empty, 30, FontStyle.Normal, Hex("#A3A9B1"));
            saved.alignment = TextAnchor.UpperCenter;
            AddSpacer(content);
            Button restart = NewButton("Botao Refazer", content, "Refazer roteiro", Hex("#2563EB"), out _);

            var panel = Undo.AddComponent<ResultPanel>(canvas);
            SetRefs(panel,
                ("flow", flow),
                ("root", root),
                ("timeText", time),
                ("errorsText", errors),
                ("quizText", quiz),
                ("stepsContainer", list.transform),
                ("rowTemplate", rowView),
                ("savedText", saved),
                ("restartButton", restart));
        }

        private static Text NewStat(Transform parent, string label, string value)
        {
            Image tile = NewImage(label, parent, Hex("#15181D"));
            var layout = tile.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 32, 32);
            layout.spacing = 6f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            tile.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            Text valueText = NewText("Valor", tile.transform, value, 60, FontStyle.Bold, Color.white);
            valueText.alignment = TextAnchor.MiddleCenter;
            Text labelText = NewText("Rotulo", tile.transform, label, 32, FontStyle.Normal, Hex("#A3A9B1"));
            labelText.alignment = TextAnchor.MiddleCenter;
            return valueText;
        }

        // ---------- Perguntas de exemplo ----------

        private static void FillExampleQuiz(InspectionFlow flow)
        {
            SerializedProperty steps = new SerializedObject(flow).FindProperty("steps");
            for (int i = 0; i < steps.arraySize; i++)
            {
                var step = steps.GetArrayElementAtIndex(i).objectReferenceValue as InspectionStep;
                if (step == null || step.Quiz.Count > 0)
                    continue;

                var so = new SerializedObject(step);
                switch (step.RequirementId)
                {
                    case "RF01":
                        AddQuestion(so, "Qual característica indica uma solda fria?", 1,
                            "Superfície brilhante e lisa", "Aspecto opaco e granulado", "Formato de cone côncavo");
                        break;
                    case "RF02":
                        AddQuestion(so, "No teste de continuidade entre alimentação e GND, o que indica um curto-circuito?", 0,
                            "O multímetro apita", "O multímetro não apita", "O visor mostra OL");
                        break;
                    case "RF03":
                        AddQuestion(so, "Por que energizar a placa com uma fonte com limite de corrente?", 1,
                            "Para a placa ligar mais rápido", "Para limitar danos caso exista um curto", "Para aumentar a tensão de saída");
                        break;
                    case "RF04":
                        AddQuestion(so, "O que conferir antes de acionar a chave de teste no veículo?", 0,
                            "Se os conectores estão bem encaixados", "A cor da placa", "O tamanho do marcador");
                        break;
                }
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(step);
            }

            AssetDatabase.SaveAssets();
        }

        private static void AddQuestion(SerializedObject so, string question, int correctIndex, params string[] options)
        {
            SerializedProperty list = so.FindProperty("quiz");
            list.arraySize++;
            SerializedProperty element = list.GetArrayElementAtIndex(list.arraySize - 1);
            element.FindPropertyRelative("question").stringValue = question;
            element.FindPropertyRelative("correctIndex").intValue = correctIndex;
            SerializedProperty optionList = element.FindPropertyRelative("options");
            optionList.arraySize = options.Length;
            for (int i = 0; i < options.Length; i++)
                optionList.GetArrayElementAtIndex(i).stringValue = options[i];
        }

        // ---------- Helpers ----------

        /// <summary>Tela cheia escura sobre a câmera, com uma coluna de conteúdo.</summary>
        private static RectTransform NewScreen(string name, Transform canvas, out GameObject root)
        {
            Image background = NewRect(name, canvas).gameObject.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.94f);
            Stretch(background.rectTransform, 0f, 0f, 0f, 0f);
            root = background.gameObject;
            Undo.RegisterCreatedObjectUndo(root, $"Criar {name}");

            RectTransform content = NewVertical("Conteudo", root.transform, 32f);
            Stretch(content, 64f, 64f, 160f, 80f);
            root.SetActive(false);
            return content;
        }

        private static void AddSpacer(Transform parent)
        {
            NewRect("Espaco", parent).gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
        }

        private static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        private static RectTransform NewVertical(string name, Transform parent, float spacing)
        {
            RectTransform rt = NewRect(name, parent);
            var layout = rt.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return rt;
        }

        private static Image NewImage(string name, Transform parent, Color color)
        {
            Image image = NewRect(name, parent).gameObject.AddComponent<Image>();
            image.sprite = rounded;
            image.type = Image.Type.Sliced;
            image.color = color;
            return image;
        }

        private static Text NewText(string name, Transform parent, string content, int size, FontStyle style, Color color)
        {
            Text text = NewRect(name, parent).gameObject.AddComponent<Text>();
            text.font = font;
            text.text = content;
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private static Button NewButton(string name, Transform parent, string label, Color color, out Text labelText)
        {
            Image image = NewImage(name, parent, color);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            image.gameObject.AddComponent<LayoutElement>().preferredHeight = 140f;

            labelText = NewText("Texto", image.transform, label, 42, FontStyle.Bold, Color.white);
            labelText.alignment = TextAnchor.MiddleCenter;
            Stretch(labelText.rectTransform, 0f, 0f, 0f, 0f);
            return button;
        }

        private static void Stretch(RectTransform rt, float left, float right, float top, float bottom)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        private static void SetRefs(Object target, params (string field, Object value)[] refs)
        {
            var so = new SerializedObject(target);
            foreach (var (field, value) in refs)
                so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }
    }
}
