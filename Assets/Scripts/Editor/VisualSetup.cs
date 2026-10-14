using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01.EditorTools
{
    /// <summary>
    /// Menus da fase 8 (visual do Figma):
    /// - "Fase 8 - Aplicar visual do Figma": cantos arredondados, selo de status, barra de
    ///   progresso e tela inicial com "Iniciar RA".
    /// - "Fase 8 - Aplicar fonte IBM Plex Sans": troca a fonte de todos os textos, se os
    ///   arquivos IBMPlexSans-Regular.ttf e IBMPlexSans-Bold.ttf estiverem no projeto.
    /// </summary>
    public static class VisualSetup
    {
        private const string SpritesFolder = "Assets/UI/Sprites";

        private static Font font;
        private static Sprite rounded;
        private static Sprite circle;
        private static Sprite bolt;

        [MenuItem("B'Energy/Fase 8 - Aplicar visual do Figma")]
        private static void Run()
        {
            var stepPanel = Object.FindFirstObjectByType<StepPanel>(FindObjectsInactive.Include);
            var flow = Object.FindFirstObjectByType<InspectionFlow>();
            if (stepPanel == null || flow == null || Object.FindFirstObjectByType<ResultPanel>(FindObjectsInactive.Include) == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Abra a cena WL01RecognitionAR e rode as fases 2 a 5 antes.", "OK");
                return;
            }

            if (Object.FindFirstObjectByType<HomeScreen>(FindObjectsInactive.Include) != null)
            {
                EditorUtility.DisplayDialog("B'Energy", "O visual da fase 8 já foi aplicado nesta cena. Nada foi alterado.", "OK");
                return;
            }

            ConfigureSprites();
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            GameObject canvas = stepPanel.gameObject;
            Undo.RegisterFullObjectHierarchyUndo(canvas, "Aplicar visual do Figma");

            RoundCorners(canvas);
            var panelSo = new SerializedObject(stepPanel);
            StyleScanHint(panelSo);
            AddProgressBar(panelSo);
            panelSo.ApplyModifiedProperties();
            BuildHomeScreen(canvas, flow);

            var flowSo = new SerializedObject(flow);
            flowSo.FindProperty("waitForStart").boolValue = true;
            flowSo.ApplyModifiedProperties();

            EditorSceneManager.MarkSceneDirty(canvas.scene);
            EditorUtility.DisplayDialog("B'Energy",
                "Visual do Figma aplicado: cantos arredondados, selo de status, barra de progresso e tela inicial.\n\nSalve a cena com Ctrl+S.",
                "OK");
        }

        [MenuItem("B'Energy/Fase 8 - Aplicar fonte IBM Plex Sans")]
        private static void ApplyFont()
        {
            Font regular = FindFont("IBMPlexSans-Regular");
            Font bold = FindFont("IBMPlexSans-Bold");
            if (regular == null || bold == null)
            {
                EditorUtility.DisplayDialog("B'Energy",
                    "Não encontrei IBMPlexSans-Regular.ttf e IBMPlexSans-Bold.ttf no projeto.\n\nColoque os dois arquivos em Assets/Fonts e rode este menu de novo.",
                    "OK");
                return;
            }

            int changed = 0;
            foreach (Text text in Object.FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Undo.RecordObject(text, "Aplicar fonte");
                ApplyFontTo(text, regular, bold);
                changed++;
            }

            const string overlayPath = "Assets/Prefabs/Overlays/StepOverlay.prefab";
            GameObject overlay = PrefabUtility.LoadPrefabContents(overlayPath);
            if (overlay != null)
            {
                foreach (Text text in overlay.GetComponentsInChildren<Text>(true))
                    ApplyFontTo(text, regular, bold);
                PrefabUtility.SaveAsPrefabAsset(overlay, overlayPath);
                PrefabUtility.UnloadPrefabContents(overlay);
            }

            EditorSceneManager.MarkAllScenesDirty();
            EditorUtility.DisplayDialog("B'Energy", $"Fonte IBM Plex Sans aplicada em {changed} textos da cena e nas marcações.\n\nSalve a cena com Ctrl+S.", "OK");
        }

        [MenuItem("B'Energy/Fase 8 - Registro em tela cheia")]
        private static void RecordFullScreen()
        {
            var stepPanel = Object.FindFirstObjectByType<StepPanel>(FindObjectsInactive.Include);
            if (stepPanel == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Abra a cena WL01RecognitionAR antes.", "OK");
                return;
            }

            var panelSo = new SerializedObject(stepPanel);
            var recordGroup = (GameObject)panelSo.FindProperty("recordGroup").objectReferenceValue;
            if (recordGroup == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Rode a Fase 4 antes.", "OK");
                return;
            }
            if (panelSo.FindProperty("recordRoot").objectReferenceValue != null)
            {
                EditorUtility.DisplayDialog("B'Energy", "O registro em tela cheia já está configurado. Nada foi alterado.", "OK");
                return;
            }

            Transform panel = recordGroup.transform.parent;
            Undo.RegisterFullObjectHierarchyUndo(panel.gameObject, "Registro em tela cheia");

            // Área com rolagem que ocupa o espaço livre do painel expandido
            RectTransform scroll = NewRect("Rolagem Registro", panel);
            scroll.SetSiblingIndex(recordGroup.transform.GetSiblingIndex());
            var scrollElement = scroll.gameObject.AddComponent<LayoutElement>();
            scrollElement.flexibleHeight = 1f;
            scrollElement.minHeight = 200f;

            RectTransform viewport = NewRect("Viewport", scroll);
            Stretch(viewport, 0f, 0f, 0f, 0f);
            viewport.gameObject.AddComponent<RectMask2D>();
            Image hitArea = viewport.gameObject.AddComponent<Image>();
            hitArea.color = new Color(0f, 0f, 0f, 0f);
            hitArea.GetComponent<CanvasRenderer>().cullTransparentMesh = false; // arrastar em qualquer ponto

            // O Grupo Registro vira o conteúdo rolável
            Undo.SetTransformParent(recordGroup.transform, viewport, "Mover registro");
            var content = (RectTransform)recordGroup.transform;
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;
            if (!recordGroup.TryGetComponent(out ContentSizeFitter fitter))
                fitter = recordGroup.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            recordGroup.SetActive(true);

            var scrollRect = scroll.gameObject.AddComponent<ScrollRect>();
            scrollRect.viewport = viewport;
            scrollRect.content = content;
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 30f;
            scroll.gameObject.SetActive(false);

            panelSo.FindProperty("recordRoot").objectReferenceValue = scroll.gameObject;
            panelSo.ApplyModifiedProperties();

            EditorSceneManager.MarkSceneDirty(stepPanel.gameObject.scene);
            EditorUtility.DisplayDialog("B'Energy",
                "Na aba Registro o painel agora ocupa a tela, a câmera fica numa faixa no topo e o conteúdo rola.\n\nSalve a cena com Ctrl+S.",
                "OK");
        }

        private static void ApplyFontTo(Text text, Font regular, Font bold)
        {
            bool isBold = text.fontStyle == FontStyle.Bold || text.font == bold;
            text.font = isBold ? bold : regular;
            text.fontStyle = text.fontStyle == FontStyle.Italic ? FontStyle.Italic : FontStyle.Normal;
            EditorUtility.SetDirty(text);
        }

        private static Font FindFont(string name)
        {
            foreach (string guid in AssetDatabase.FindAssets($"{name} t:Font"))
            {
                var found = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(guid));
                if (found != null && found.name == name)
                    return found;
            }
            return null;
        }

        // ---------- Sprites ----------

        private static void ConfigureSprites()
        {
            rounded = ImportSprite($"{SpritesFolder}/arredondado.png", new Vector4(40, 40, 40, 40));
            circle = ImportSprite($"{SpritesFolder}/circulo.png", Vector4.zero);
            bolt = ImportSprite($"{SpritesFolder}/raio.png", Vector4.zero);
        }

        private static Sprite ImportSprite(string path, Vector4 border)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spriteBorder = border;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // ---------- Restyle ----------

        private static void RoundCorners(GameObject canvas)
        {
            Sprite builtin = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            foreach (Image image in canvas.GetComponentsInChildren<Image>(true))
            {
                if (image.sprite != builtin)
                    continue;

                image.sprite = rounded;
                image.type = Image.Type.Sliced;
                image.pixelsPerUnitMultiplier = image.name switch
                {
                    "Painel Etapa" => 0.8f,    // ~50 px de raio
                    "Caixa" => 2.2f,           // caixinha do checklist
                    "Selo" => 1.6f,            // selo Aprovada/Revisar
                    "Aviso Busca" => 0.9f,     // selo de status (pílula)
                    _ => 1.1f
                };
            }
        }

        private static void StyleScanHint(SerializedObject panelSo)
        {
            var hint = (GameObject)panelSo.FindProperty("scanHint").objectReferenceValue;
            var layout = hint.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 20f;
            layout.childAlignment = TextAnchor.MiddleLeft;

            Image dot = NewRect("Ponto", hint.transform).gameObject.AddComponent<Image>();
            dot.transform.SetAsFirstSibling();
            dot.sprite = circle;
            dot.color = Hex("#FBBF24");
            dot.raycastTarget = false;
            var element = dot.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = element.minWidth = 24f;
            element.preferredHeight = element.minHeight = 24f;
            layout.childForceExpandHeight = false;

            panelSo.FindProperty("scanHintDot").objectReferenceValue = dot;
        }

        private static void AddProgressBar(SerializedObject panelSo)
        {
            var label = (Text)panelSo.FindProperty("stepLabel").objectReferenceValue;
            Transform panel = label.transform.parent;

            RectTransform bar = NewRect("Progresso", panel);
            bar.SetSiblingIndex(label.transform.GetSiblingIndex() + 1);
            var layout = bar.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12f;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            bar.gameObject.AddComponent<LayoutElement>().preferredHeight = 10f;

            Image segment = NewRect("Segmento (modelo)", bar).gameObject.AddComponent<Image>();
            segment.sprite = rounded;
            segment.type = Image.Type.Sliced;
            segment.pixelsPerUnitMultiplier = 8f;
            segment.raycastTarget = false;
            var element = segment.gameObject.AddComponent<LayoutElement>();
            element.preferredHeight = 10f;
            element.flexibleWidth = 1f;

            panelSo.FindProperty("progressContainer").objectReferenceValue = bar;
            panelSo.FindProperty("progressTemplate").objectReferenceValue = segment;
        }

        // ---------- Tela inicial ----------

        private static void BuildHomeScreen(GameObject canvas, InspectionFlow flow)
        {
            Image background = NewRect("Tela Inicial", canvas.transform).gameObject.AddComponent<Image>();
            background.color = Color.black;
            Stretch(background.rectTransform, 0f, 0f, 0f, 0f);
            GameObject root = background.gameObject;

            RectTransform content = NewVertical("Conteudo", root.transform, 40f);
            Stretch(content, 64f, 64f, 200f, 96f);

            // Logo
            RectTransform logoRow = NewRect("Logo", content);
            var logoLayout = logoRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            logoLayout.childControlWidth = logoLayout.childControlHeight = false;
            logoLayout.childForceExpandWidth = logoLayout.childForceExpandHeight = false;
            logoRow.gameObject.AddComponent<LayoutElement>().preferredHeight = 150f;
            Image logo = NewImage("Quadrado", logoRow, Hex("#2563EB"), 1.2f);
            logo.rectTransform.sizeDelta = new Vector2(150f, 150f);
            Image boltImage = NewRect("Raio", logo.transform).gameObject.AddComponent<Image>();
            boltImage.sprite = bolt;
            boltImage.raycastTarget = false;
            Stretch(boltImage.rectTransform, 32f, 32f, 32f, 32f);

            NewText("Titulo", content, "B'Energy AR Academy", 76, FontStyle.Bold, Color.white);
            NewText("Subtitulo", content, "Inspeção de PCB com Realidade Aumentada", 40, FontStyle.Normal, Hex("#A3A9B1"));
            NewText("Secao", content, "ROTEIRO DE INSPEÇÃO", 32, FontStyle.Bold, Hex("#A3A9B1"));

            SerializedProperty steps = new SerializedObject(flow).FindProperty("steps");
            for (int i = 0; i < steps.arraySize; i++)
            {
                var step = steps.GetArrayElementAtIndex(i).objectReferenceValue as InspectionStep;
                if (step != null)
                    BuildStepCard(content, i + 1, step);
            }

            NewRect("Espaco", content).gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;

            Image buttonImage = NewImage("Botao Iniciar", content, Hex("#2563EB"), 1.1f);
            var startButton = buttonImage.gameObject.AddComponent<Button>();
            startButton.targetGraphic = buttonImage;
            buttonImage.gameObject.AddComponent<LayoutElement>().preferredHeight = 150f;
            Text buttonLabel = NewText("Texto", buttonImage.transform, "Iniciar RA", 46, FontStyle.Bold, Color.white);
            buttonLabel.alignment = TextAnchor.MiddleCenter;
            Stretch(buttonLabel.rectTransform, 0f, 0f, 0f, 0f);

            Text footer = NewText("Rodape", content, "Aponte a câmera para o marcador da placa", 32, FontStyle.Normal, Hex("#A3A9B1"));
            footer.alignment = TextAnchor.UpperCenter;

            var home = canvas.AddComponent<HomeScreen>();
            var homeSo = new SerializedObject(home);
            homeSo.FindProperty("flow").objectReferenceValue = flow;
            homeSo.FindProperty("root").objectReferenceValue = root;
            homeSo.FindProperty("startButton").objectReferenceValue = startButton;
            homeSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildStepCard(Transform parent, int number, InspectionStep step)
        {
            Image card = NewImage($"Etapa {number}", parent, Hex("#15181D"), 1.1f);
            var layout = card.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(36, 36, 28, 28);
            layout.spacing = 32f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = layout.childForceExpandHeight = false;

            Image badge = NewRect("Numero", card.transform).gameObject.AddComponent<Image>();
            badge.sprite = circle;
            badge.color = Hex("#1B2A4A");
            badge.raycastTarget = false;
            var badgeElement = badge.gameObject.AddComponent<LayoutElement>();
            badgeElement.preferredWidth = badgeElement.minWidth = 84f;
            badgeElement.preferredHeight = badgeElement.minHeight = 84f;
            Text numberText = NewText("Texto", badge.transform, number.ToString(), 38, FontStyle.Bold, Hex("#93C5FD"));
            numberText.alignment = TextAnchor.MiddleCenter;
            Stretch(numberText.rectTransform, 0f, 0f, 0f, 0f);

            RectTransform texts = NewVertical("Textos", card.transform, 4f);
            texts.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            NewText("Titulo", texts, step.Title, 42, FontStyle.Bold, Color.white);
            NewText("Requisito", texts, step.RequirementId, 32, FontStyle.Normal, Hex("#A3A9B1"));
        }

        // ---------- Helpers ----------

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

        private static Image NewImage(string name, Transform parent, Color color, float cornerMultiplier)
        {
            Image image = NewRect(name, parent).gameObject.AddComponent<Image>();
            image.sprite = rounded;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = cornerMultiplier;
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

        private static void Stretch(RectTransform rt, float left, float right, float top, float bottom)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }
    }
}
