using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01.EditorTools
{
    /// <summary>
    /// Menu "B'Energy > Fase 9 - Tela de registro e remover quiz":
    /// - monta a "Tela Registro" (tela cheia com checklist e medições em rolagem) e liga o RecordScreen;
    /// - troca a aba Registro do painel da etapa pelo botão "Registrar", que abre essa tela;
    /// - apaga o registro antigo do painel e tudo que sobrou do quiz na cena.
    /// </summary>
    public static class RecordScreenSetup
    {
        private const string UndoName = "Fase 9 - Tela de registro";

        private static Font regular;
        private static Font bold;
        private static Sprite rounded;
        private static Sprite circle;

        [MenuItem("B'Energy/Fase 9 - Tela de registro e remover quiz")]
        private static void Run()
        {
            var stepPanel = Object.FindFirstObjectByType<StepPanel>(FindObjectsInactive.Include);
            var flow = Object.FindFirstObjectByType<InspectionFlow>(FindObjectsInactive.Include);
            if (stepPanel == null || flow == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Abra a cena WL01RecognitionAR e rode as fases anteriores antes.", "OK");
                return;
            }

            if (Object.FindFirstObjectByType<RecordScreen>(FindObjectsInactive.Include) != null)
            {
                EditorUtility.DisplayDialog("B'Energy", "A tela de registro já existe nesta cena. Nada foi alterado.", "OK");
                return;
            }

            LoadAssets();
            GameObject canvas = stepPanel.gameObject;
            Undo.SetCurrentGroupName(UndoName);
            int undoGroup = Undo.GetCurrentGroup();

            // a) Tela Registro + b) componente RecordScreen
            RecordScreen screen = BuildRecordScreen(canvas, flow);

            // c) Painel da etapa: botão Registrar no lugar da aba, sem o registro antigo
            string panelReport = SetupStepPanel(stepPanel, screen);

            // d) Quiz fora da cena
            string quizReport = RemoveQuiz(canvas);

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(canvas.scene);
            EditorUtility.DisplayDialog("B'Energy",
                "Tela de registro criada e ligada ao botão Registrar do painel." +
                panelReport + quizReport +
                "\n\nSalve a cena com Ctrl+S.",
                "OK");
        }

        // ---------- a/b) Tela Registro ----------

        private static RecordScreen BuildRecordScreen(GameObject canvas, InspectionFlow flow)
        {
            Image background = NewRect("Tela Registro", canvas.transform).gameObject.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.94f);
            Stretch(background.rectTransform, 0f, 0f, 0f, 0f);
            GameObject root = background.gameObject;
            PlaceScreen(root.transform, canvas.transform);

            RectTransform content = NewVertical("Conteudo", root.transform, 32f);
            Stretch(content, 64f, 64f, 160f, 80f);

            // Topo: voltar + etapa + título
            RectTransform top = NewRect("Topo", content);
            var topLayout = top.gameObject.AddComponent<HorizontalLayoutGroup>();
            topLayout.spacing = 32f;
            topLayout.childAlignment = TextAnchor.MiddleLeft;
            topLayout.childControlWidth = topLayout.childControlHeight = true;
            topLayout.childForceExpandWidth = topLayout.childForceExpandHeight = false;

            Image closeImage = NewRect("Botao Voltar", top).gameObject.AddComponent<Image>();
            closeImage.sprite = circle;
            closeImage.color = Hex("#15181D");
            var closeElement = closeImage.gameObject.AddComponent<LayoutElement>();
            closeElement.minWidth = closeElement.preferredWidth = 96f;
            closeElement.minHeight = closeElement.preferredHeight = 96f;
            var closeButton = closeImage.gameObject.AddComponent<Button>();
            closeButton.targetGraphic = closeImage;
            Text closeGlyph = NewText("Texto", closeImage.transform, "‹", 64, FontStyle.Bold, Color.white);
            closeGlyph.alignment = TextAnchor.MiddleCenter;
            Stretch(closeGlyph.rectTransform, 0f, 0f, 0f, 8f);

            RectTransform heading = NewVertical("Textos", top, 4f);
            heading.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            Text stepLabel = NewText("Etapa", heading, "Etapa 1 de 4 · RF01", 34, FontStyle.Normal, Hex("#A3A9B1"));
            Text titleText = NewText("Titulo", heading, "Registro da etapa", 52, FontStyle.Bold, Color.white);

            // Área com rolagem: checklist e medições
            RectTransform scroll = NewRect("Rolagem", content);
            var scrollElement = scroll.gameObject.AddComponent<LayoutElement>();
            scrollElement.flexibleHeight = 1f;
            scrollElement.minHeight = 200f;

            RectTransform viewport = NewRect("Viewport", scroll);
            Stretch(viewport, 0f, 0f, 0f, 0f);
            viewport.gameObject.AddComponent<RectMask2D>();
            Image hitArea = viewport.gameObject.AddComponent<Image>();
            hitArea.color = new Color(0f, 0f, 0f, 0f);
            hitArea.GetComponent<CanvasRenderer>().cullTransparentMesh = false; // arrastar em qualquer ponto

            RectTransform list = NewVertical("Lista", viewport, 48f);
            list.anchorMin = new Vector2(0f, 1f);
            list.anchorMax = new Vector2(1f, 1f);
            list.pivot = new Vector2(0.5f, 1f);
            list.anchoredPosition = Vector2.zero;
            list.sizeDelta = Vector2.zero;
            list.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform checklist = NewVertical("Checklist", list, 12f);
            ChecklistItemView checklistTemplate = BuildChecklistTemplate(checklist);
            RectTransform measurements = NewVertical("Medicoes", list, 24f);
            MeasurementFieldView measurementTemplate = BuildMeasurementTemplate(measurements);

            var scrollRect = scroll.gameObject.AddComponent<ScrollRect>();
            scrollRect.viewport = viewport;
            scrollRect.content = list;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 30f;

            // Rodapé: pendências + concluir
            Text pendingText = NewText("Pendencias", content, "Marque todos os itens e preencha as medições.", 36, FontStyle.Normal, Hex("#FBBF24"));
            pendingText.alignment = TextAnchor.UpperCenter;

            Image doneImage = NewImage("Botao Concluir", content, Hex("#2563EB"));
            var doneButton = doneImage.gameObject.AddComponent<Button>();
            doneButton.targetGraphic = doneImage;
            doneImage.gameObject.AddComponent<LayoutElement>().preferredHeight = 140f;
            Text doneLabel = NewText("Texto", doneImage.transform, "Concluir registro", 42, FontStyle.Bold, Color.white);
            doneLabel.alignment = TextAnchor.MiddleCenter;
            Stretch(doneLabel.rectTransform, 0f, 0f, 0f, 0f);

            root.SetActive(false);
            Undo.RegisterCreatedObjectUndo(root, UndoName);

            var screen = Undo.AddComponent<RecordScreen>(canvas);
            SetRefs(screen, false,
                ("flow", flow),
                ("root", root),
                ("stepLabel", stepLabel),
                ("titleText", titleText),
                ("checklistContainer", checklist),
                ("checklistTemplate", checklistTemplate),
                ("measurementsContainer", measurements),
                ("measurementTemplate", measurementTemplate),
                ("pendingText", pendingText),
                ("doneButton", doneButton),
                ("doneLabel", doneLabel),
                ("closeButton", closeButton));
            return screen;
        }

        /// <summary>Último filho do canvas, mas abaixo da Tela Resultado e da Tela Inicial.</summary>
        private static void PlaceScreen(Transform screen, Transform canvas)
        {
            int index = -1;
            foreach (string above in new[] { "Tela Resultado", "Tela Inicial" })
            {
                Transform other = canvas.Find(above);
                if (other != null && other != screen && (index < 0 || other.GetSiblingIndex() < index))
                    index = other.GetSiblingIndex();
            }

            if (index < 0)
                screen.SetAsLastSibling();
            else
                screen.SetSiblingIndex(index);
        }

        // ---------- Modelos (iguais aos da Fase 4) ----------

        private static ChecklistItemView BuildChecklistTemplate(Transform parent)
        {
            Image row = NewRect("Item (modelo)", parent).gameObject.AddComponent<Image>();
            row.color = new Color(0f, 0f, 0f, 0f);
            var rowLayout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 24f;
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childControlWidth = rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = rowLayout.childForceExpandHeight = false;
            row.gameObject.AddComponent<LayoutElement>().minHeight = 96f;

            Image box = NewImage("Caixa", row.transform, Hex("#1B2A4A"));
            box.pixelsPerUnitMultiplier = 2.2f;
            var boxElement = box.gameObject.AddComponent<LayoutElement>();
            boxElement.minWidth = boxElement.preferredWidth = 72f;
            boxElement.minHeight = boxElement.preferredHeight = 72f;

            Image check = NewRect("Marca", box.transform).gameObject.AddComponent<Image>();
            check.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
            check.color = Hex("#60A5FA");
            check.raycastTarget = false;
            Stretch(check.rectTransform, 8f, 8f, 8f, 8f);

            Text label = NewText("Texto", row.transform, "Item do checklist", 38, FontStyle.Normal, Hex("#D4D8DD"));
            label.alignment = TextAnchor.MiddleLeft;
            label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var toggle = row.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = box;
            toggle.graphic = check;
            toggle.isOn = false;

            var view = row.gameObject.AddComponent<ChecklistItemView>();
            SetRefs(view, false, ("toggle", toggle), ("label", label));
            return view;
        }

        private static MeasurementFieldView BuildMeasurementTemplate(Transform parent)
        {
            RectTransform root = NewVertical("Medicao (modelo)", parent, 10f);
            Text label = NewText("Rotulo", root, "Medição", 34, FontStyle.Normal, Hex("#A3A9B1"));

            RectTransform line = NewRect("Linha", root);
            var lineLayout = line.gameObject.AddComponent<HorizontalLayoutGroup>();
            lineLayout.spacing = 20f;
            lineLayout.childAlignment = TextAnchor.MiddleLeft;
            lineLayout.childControlWidth = lineLayout.childControlHeight = true;
            lineLayout.childForceExpandWidth = lineLayout.childForceExpandHeight = false;

            Image fieldBg = NewImage("Campo", line, Hex("#1B2A4A"));
            var fieldElement = fieldBg.gameObject.AddComponent<LayoutElement>();
            fieldElement.preferredHeight = 110f;
            fieldElement.flexibleWidth = 1f;

            Text fieldText = NewText("Texto", fieldBg.transform, string.Empty, 40, FontStyle.Normal, Color.white);
            fieldText.alignment = TextAnchor.MiddleLeft;
            fieldText.supportRichText = false;
            Stretch(fieldText.rectTransform, 28f, 28f, 28f, 28f);

            Text placeholder = NewText("Placeholder", fieldBg.transform, "0,00", 40, FontStyle.Italic, Hex("#6B7280"));
            placeholder.alignment = TextAnchor.MiddleLeft;
            Stretch(placeholder.rectTransform, 28f, 28f, 28f, 28f);

            var input = fieldBg.gameObject.AddComponent<InputField>();
            input.targetGraphic = fieldBg;
            input.textComponent = fieldText;
            input.placeholder = placeholder;
            input.contentType = InputField.ContentType.Custom;
            input.lineType = InputField.LineType.SingleLine;
            input.keyboardType = TouchScreenKeyboardType.DecimalPad;

            Text unit = NewText("Unidade", line, "V", 40, FontStyle.Normal, Color.white);
            unit.alignment = TextAnchor.MiddleLeft;
            unit.gameObject.AddComponent<LayoutElement>().preferredWidth = 90f;

            Text status = NewText("Status", root, "Dentro do esperado", 32, FontStyle.Normal, Hex("#86EFAC"));

            var view = root.gameObject.AddComponent<MeasurementFieldView>();
            SetRefs(view, false, ("label", label), ("input", input), ("unit", unit), ("status", status));
            return view;
        }

        // ---------- c) Painel da etapa ----------

        private static string SetupStepPanel(StepPanel stepPanel, RecordScreen screen)
        {
            var panelSo = new SerializedObject(stepPanel);
            var panelRootProp = panelSo.FindProperty("panelRoot");
            var tabsRootProp = panelSo.FindProperty("tabsRoot");
            Transform panel = panelRootProp != null && panelRootProp.objectReferenceValue is GameObject panelGo
                ? panelGo.transform
                : stepPanel.transform;
            Transform tabs = tabsRootProp != null && tabsRootProp.objectReferenceValue is GameObject tabsGo
                ? tabsGo.transform
                : FindDeep(panel, "Abas");

            string report = string.Empty;
            Transform recordTab = (tabs != null ? FindDeep(tabs, "Aba Registro") : null) ?? FindDeep(panel, "Aba Registro");
            Button recordButton = null;
            Text recordLabel = null;
            if (recordTab != null)
            {
                Undo.RecordObject(recordTab.gameObject, UndoName);
                recordTab.gameObject.name = "Botao Registrar";
                recordButton = recordTab.GetComponent<Button>();
                recordLabel = recordTab.GetComponentInChildren<Text>(true);
                if (recordLabel != null)
                {
                    Undo.RecordObject(recordLabel, UndoName);
                    recordLabel.text = "Registrar";
                }
            }
            else
            {
                report += "\n\nAtenção: não achei \"Aba Registro\" no painel; ligue o recordButton à mão.";
            }

            SetRefs(stepPanel, true,
                ("recordScreen", screen),
                ("recordButton", recordButton),
                ("recordButtonLabel", recordLabel));

            // Registro antigo dentro do painel
            int removed = 0;
            foreach (string name in new[] { "Rolagem Registro", "Grupo Registro" })
            {
                Transform old = FindDeep(panel, name);
                if (old == null)
                    continue;
                Undo.DestroyObjectImmediate(old.gameObject);
                removed++;
            }
            if (removed > 0)
                report += "\nO registro antigo do painel foi apagado.";

            return report;
        }

        // ---------- d) Quiz ----------

        private static string RemoveQuiz(GameObject canvas)
        {
            string report = string.Empty;

            Transform quizScreen = FindDeep(canvas.transform, "Tela Quiz");
            if (quizScreen != null)
            {
                Undo.DestroyObjectImmediate(quizScreen.gameObject);
                report += "\nTela Quiz apagada.";
            }

            Transform result = FindDeep(canvas.transform, "Tela Resultado");
            Transform stats = result != null ? FindDeep(result, "Indicadores") : null;
            Transform quizStat = stats != null ? stats.Find("Quiz") : null;
            if (quizStat != null)
            {
                Undo.DestroyObjectImmediate(quizStat.gameObject);
                report += "\nIndicador Quiz do resultado apagado.";
            }

            int missing = 0;
            foreach (Transform t in canvas.GetComponentsInChildren<Transform>(true))
            {
                GameObject go = t.gameObject;
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) == 0)
                    continue;
                Undo.RegisterCompleteObjectUndo(go, UndoName);
                missing += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            }
            if (missing > 0)
                report += $"\n{missing} componente(s) sem script removido(s).";

            return report;
        }

        // ---------- Helpers ----------

        private static void LoadAssets()
        {
            regular = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/IBMPlexSans-Regular.ttf");
            bold = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/IBMPlexSans-Bold.ttf");
            if (regular == null || bold == null)
                regular = bold = null;

            rounded = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Sprites/arredondado.png")
                      ?? AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            circle = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Sprites/circulo.png")
                     ?? AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        }

        private static Transform FindDeep(Transform parent, string name)
        {
            foreach (Transform t in parent.GetComponentsInChildren<Transform>(true))
                if (t != parent && t.name == name)
                    return t;
            return null;
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
            image.pixelsPerUnitMultiplier = 1.1f;
            image.color = color;
            return image;
        }

        private static Text NewText(string name, Transform parent, string content, int size, FontStyle style, Color color)
        {
            Text text = NewRect(name, parent).gameObject.AddComponent<Text>();
            if (regular != null)
            {
                // Com a IBM Plex Sans o negrito vem do arquivo Bold (igual à Fase 8 - Aplicar fonte)
                text.font = style == FontStyle.Bold ? bold : regular;
                text.fontStyle = style == FontStyle.Italic ? FontStyle.Italic : FontStyle.Normal;
            }
            else
            {
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.fontStyle = style;
            }
            text.text = content;
            text.fontSize = size;
            text.color = color;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private static void Stretch(RectTransform rt, float left, float right, float top, float bottom)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        /// <summary>Liga campos serializados; avisa no Console se algum nome não existir.
        /// Objetos recém-criados não precisam de Undo próprio (o Undo da criação já os desfaz).</summary>
        private static void SetRefs(Object target, bool withUndo, params (string field, Object value)[] refs)
        {
            var so = new SerializedObject(target);
            foreach (var (field, value) in refs)
            {
                SerializedProperty prop = so.FindProperty(field);
                if (prop == null)
                {
                    Debug.LogWarning($"[Fase 9] {target.GetType().Name} não tem o campo '{field}'.", target);
                    continue;
                }
                prop.objectReferenceValue = value;
            }
            if (withUndo)
                so.ApplyModifiedProperties();
            else
                so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }
    }
}
