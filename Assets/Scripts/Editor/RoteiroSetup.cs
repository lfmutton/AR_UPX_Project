using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Facens.WL01.EditorTools
{
    /// <summary>
    /// Menu "B'Energy > Fase 2 - Criar etapas e UI do roteiro": cria os 4 assets de
    /// etapa (conteúdo [preencher]) e monta o Canvas com o painel da etapa na cena aberta.
    /// </summary>
    public static class RoteiroSetup
    {
        private const string StepsFolder = "Assets/Etapas";

        private static Font font;
        private static Sprite roundedSprite;

        [MenuItem("B'Energy/Fase 2 - Criar etapas e UI do roteiro")]
        private static void Run()
        {
            var recognition = Object.FindFirstObjectByType<ImageRecognitionController>();
            if (recognition == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Abra a cena WL01RecognitionAR antes de rodar este menu.", "OK");
                return;
            }

            if (Object.FindFirstObjectByType<InspectionFlow>() != null)
            {
                EditorUtility.DisplayDialog("B'Energy", "O roteiro já existe nesta cena. Nada foi alterado.", "OK");
                return;
            }

            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            roundedSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

            InspectionStep[] steps = CreateSteps();

            var flowGo = new GameObject("Roteiro");
            Undo.RegisterCreatedObjectUndo(flowGo, "Criar roteiro");
            var flow = flowGo.AddComponent<InspectionFlow>();
            var flowSo = new SerializedObject(flow);
            flowSo.FindProperty("recognition").objectReferenceValue = recognition;
            SerializedProperty list = flowSo.FindProperty("steps");
            list.arraySize = steps.Length;
            for (int i = 0; i < steps.Length; i++)
                list.GetArrayElementAtIndex(i).objectReferenceValue = steps[i];
            flowSo.ApplyModifiedPropertiesWithoutUndo();

            EnsureEventSystem();
            GameObject canvas = BuildCanvas(flow);

            EditorSceneManager.MarkSceneDirty(flowGo.scene);
            Selection.activeGameObject = canvas;
            EditorUtility.DisplayDialog("B'Energy", "Roteiro e UI criados.\n\nSalve a cena com Ctrl+S.", "OK");
        }

        // ---------- Etapas ----------

        private static InspectionStep[] CreateSteps()
        {
            if (!AssetDatabase.IsValidFolder(StepsFolder))
                AssetDatabase.CreateFolder("Assets", "Etapas");

            var steps = new[]
            {
                CreateStep("Etapa1_Soldagem", "Guia de soldagem", "RF01",
                    "Compare as soldas destacadas na placa com as referências abaixo. [preencher: componentes a inspecionar]",
                    ("Boa solda", "Brilhante, lisa, em forma de cone côncavo."),
                    ("Solda fria", "Opaca, granulada, em forma de bolinha.")),
                CreateStep("Etapa2_Continuidade", "Continuidade e curto-circuito", "RF02",
                    "Coloque o multímetro em modo continuidade e meça entre os test points indicados na placa.",
                    ("Pontos", "[preencher: test points a medir]"),
                    ("Esperado", "Sem bipe entre alimentação e GND (sem curto). [preencher]")),
                CreateStep("Etapa3_Energizacao", "Energização / Bring UP", "RF03",
                    "Energize a placa com fonte de bancada com limite de corrente e observe os LEDs de diagnóstico.",
                    ("LEDs", "[preencher: LEDs e comportamento esperado]"),
                    ("Tensões", "[preencher: trilhos e faixas esperadas]")),
                CreateStep("Etapa4_Integracao", "Integração no veículo", "RF04",
                    "Conecte a placa ao veículo e acione conectores e chaves na ordem indicada.",
                    ("Conectores", "[preencher: conectores e chicotes]"),
                    ("Sequência", "[preencher: ordem de acionamento]"))
            };

            AssetDatabase.SaveAssets();
            return steps;
        }

        private static InspectionStep CreateStep(string fileName, string title, string requirementId,
            string instruction, params (string title, string text)[] cards)
        {
            string path = $"{StepsFolder}/{fileName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<InspectionStep>(path);
            if (existing != null)
                return existing;

            var step = ScriptableObject.CreateInstance<InspectionStep>();
            AssetDatabase.CreateAsset(step, path);

            var so = new SerializedObject(step);
            so.FindProperty("title").stringValue = title;
            so.FindProperty("requirementId").stringValue = requirementId;
            so.FindProperty("instruction").stringValue = instruction;
            SerializedProperty list = so.FindProperty("cards");
            list.arraySize = cards.Length;
            for (int i = 0; i < cards.Length; i++)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("title").stringValue = cards[i].title;
                element.FindPropertyRelative("text").stringValue = cards[i].text;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(step);
            return step;
        }

        // ---------- UI ----------

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
                return;

            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(go, "Criar EventSystem");
        }

        private static GameObject BuildCanvas(InspectionFlow flow)
        {
            var canvasGo = new GameObject("Canvas Roteiro", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGo, "Criar Canvas");
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0f;

            // Aviso de busca (topo)
            Image hint = NewImage("Aviso Busca", canvasGo.transform, Hex("#000000", 0.75f));
            RectTransform hintRt = hint.rectTransform;
            hintRt.anchorMin = hintRt.anchorMax = new Vector2(0.5f, 1f);
            hintRt.pivot = new Vector2(0.5f, 1f);
            hintRt.anchoredPosition = new Vector2(0f, -140f);
            var hintLayout = hint.gameObject.AddComponent<HorizontalLayoutGroup>();
            hintLayout.padding = new RectOffset(44, 44, 26, 26);
            hintLayout.childControlWidth = hintLayout.childControlHeight = true;
            var hintFitter = hint.gameObject.AddComponent<ContentSizeFitter>();
            hintFitter.horizontalFit = hintFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text hintText = NewText("Texto", hint.transform, "Aponte a câmera para o marcador da placa", 38, FontStyle.Normal, Color.white);
            hintText.horizontalOverflow = HorizontalWrapMode.Overflow;

            // Painel da etapa (rodapé)
            Image panel = NewImage("Painel Etapa", canvasGo.transform, Hex("#15181D"));
            RectTransform panelRt = panel.rectTransform;
            panelRt.anchorMin = new Vector2(0f, 0f);
            panelRt.anchorMax = new Vector2(1f, 0f);
            panelRt.pivot = new Vector2(0.5f, 0f);
            panelRt.anchoredPosition = new Vector2(0f, 48f);
            panelRt.sizeDelta = new Vector2(-64f, 0f);
            var panelLayout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(48, 48, 48, 48);
            panelLayout.spacing = 28f;
            panelLayout.childControlWidth = panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;
            panel.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Text stepLabel = NewText("Rotulo Etapa", panel.transform, "Etapa 1 de 4 · RF01", 34, FontStyle.Normal, Hex("#A3A9B1"));
            Text title = NewText("Titulo", panel.transform, "Guia de soldagem", 54, FontStyle.Bold, Color.white);
            Text instruction = NewText("Instrucao", panel.transform, "Instrução da etapa", 40, FontStyle.Normal, Hex("#D4D8DD"));

            // Cards
            RectTransform cards = NewRect("Cards", panel.transform);
            var cardsLayout = cards.gameObject.AddComponent<HorizontalLayoutGroup>();
            cardsLayout.spacing = 24f;
            cardsLayout.childControlWidth = cardsLayout.childControlHeight = true;
            cardsLayout.childForceExpandWidth = cardsLayout.childForceExpandHeight = true;

            Image card = NewImage("Card (modelo)", cards, Hex("#1B2A4A"));
            var cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.padding = new RectOffset(32, 32, 28, 28);
            cardLayout.spacing = 10f;
            cardLayout.childControlWidth = cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = true;
            cardLayout.childForceExpandHeight = false;
            card.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            Text cardTitle = NewText("Titulo", card.transform, "Título", 36, FontStyle.Bold, Hex("#93C5FD"));
            Text cardBody = NewText("Texto", card.transform, "Texto", 34, FontStyle.Normal, Hex("#D4D8DD"));
            var cardView = card.gameObject.AddComponent<InfoCardView>();
            SetRefs(cardView, ("titleText", cardTitle), ("bodyText", cardBody));

            // Botões
            RectTransform buttons = NewRect("Botoes", panel.transform);
            var buttonsLayout = buttons.gameObject.AddComponent<HorizontalLayoutGroup>();
            buttonsLayout.spacing = 24f;
            buttonsLayout.childControlWidth = buttonsLayout.childControlHeight = true;
            buttonsLayout.childForceExpandWidth = true;
            buttonsLayout.childForceExpandHeight = false;
            Button back = NewButton("Botao Voltar", buttons, "Voltar", Hex("#1B2A4A"), out _);
            Button next = NewButton("Botao Proximo", buttons, "Próximo", Hex("#2563EB"), out Text nextLabel);

            var stepPanel = canvasGo.AddComponent<StepPanel>();
            SetRefs(stepPanel,
                ("flow", flow),
                ("scanHint", hint.gameObject),
                ("scanHintText", hintText),
                ("panelRoot", panel.gameObject),
                ("stepLabel", stepLabel),
                ("titleText", title),
                ("instructionText", instruction),
                ("cardsContainer", cards),
                ("cardTemplate", cardView),
                ("backButton", back),
                ("nextButton", next),
                ("nextLabel", nextLabel));

            panel.gameObject.SetActive(false);
            return canvasGo;
        }

        // ---------- Helpers ----------

        private static void SetRefs(Object target, params (string field, Object value)[] refs)
        {
            var so = new SerializedObject(target);
            foreach (var (field, value) in refs)
                so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        private static Image NewImage(string name, Transform parent, Color color)
        {
            Image image = NewRect(name, parent).gameObject.AddComponent<Image>();
            image.sprite = roundedSprite;
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
            var element = image.gameObject.AddComponent<LayoutElement>();
            element.preferredHeight = 140f;
            element.flexibleWidth = 1f;

            labelText = NewText("Texto", image.transform, label, 42, FontStyle.Bold, Color.white);
            labelText.alignment = TextAnchor.MiddleCenter;
            RectTransform rt = labelText.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return button;
        }

        private static Color Hex(string hex, float alpha = 1f)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            color.a = alpha;
            return color;
        }
    }
}
