using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01.EditorTools
{
    /// <summary>
    /// Menu "B'Energy > Fase 3 - Criar marcações sobre a placa": cria o prefab StepOverlay
    /// (Canvas deitado sobre o marcador), troca o cubo por ele e preenche marcações de
    /// exemplo nas etapas que ainda não têm nenhuma.
    /// </summary>
    public static class OverlaySetup
    {
        private const string PrefabPath = "Assets/Prefabs/Overlays/StepOverlay.prefab";

        private static Font font;

        [MenuItem("B'Energy/Fase 3 - Criar marcações sobre a placa")]
        private static void Run()
        {
            var recognition = Object.FindFirstObjectByType<ImageRecognitionController>();
            var flow = Object.FindFirstObjectByType<InspectionFlow>();
            if (recognition == null || flow == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Abra a cena WL01RecognitionAR e rode a Fase 2 antes.", "OK");
                return;
            }

            if (Object.FindFirstObjectByType<StepOverlayController>() != null)
            {
                EditorUtility.DisplayDialog("B'Energy", "As marcações já estão configuradas nesta cena. Nada foi alterado.", "OK");
                return;
            }

            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject prefab = BuildOverlayPrefab();

            var recognitionSo = new SerializedObject(recognition);
            recognitionSo.FindProperty("contentPrefab").objectReferenceValue = prefab;
            recognitionSo.FindProperty("contentOffset").vector3Value = new Vector3(0f, 0.002f, 0f);
            recognitionSo.ApplyModifiedProperties();

            var controller = Undo.AddComponent<StepOverlayController>(flow.gameObject);
            var controllerSo = new SerializedObject(controller);
            controllerSo.FindProperty("recognition").objectReferenceValue = recognition;
            controllerSo.FindProperty("flow").objectReferenceValue = flow;
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            FillExampleHighlights(flow);

            EditorSceneManager.MarkSceneDirty(flow.gameObject.scene);
            Selection.activeObject = prefab;
            EditorUtility.DisplayDialog("B'Energy",
                "Marcações criadas e ligadas ao reconhecimento.\n\nSalve a cena com Ctrl+S.\n\nAs posições de exemplo ficam em cada etapa (Assets/Etapas), campo Highlights.",
                "OK");
        }

        // ---------- Prefab ----------

        private static GameObject BuildOverlayPrefab()
        {
            Sprite rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            Sprite circle = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

            var root = new GameObject("StepOverlay");
            var overlay = root.AddComponent<StepOverlay>();

            // Canvas deitado no plano da imagem: 1 unidade = 0,1 mm
            var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            canvasGo.transform.SetParent(root.transform, false);
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            canvasGo.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 3f;
            var canvasRt = (RectTransform)canvasGo.transform;
            canvasRt.sizeDelta = new Vector2(1200f, 1200f);
            canvasRt.localScale = Vector3.one * 0.0001f;
            canvasRt.localRotation = Quaternion.Euler(90f, 0f, 0f);

            RectTransform container = NewRect("Destaques", canvasRt);
            container.anchorMin = Vector2.zero;
            container.anchorMax = Vector2.one;
            container.offsetMin = container.offsetMax = Vector2.zero;

            // Modelo de marcação
            RectTransform template = NewRect("Destaque (modelo)", container);
            template.sizeDelta = Vector2.zero;

            Image dot = NewRect("Ponto", template).gameObject.AddComponent<Image>();
            dot.sprite = circle;
            dot.rectTransform.sizeDelta = new Vector2(180f, 180f);
            dot.raycastTarget = false;

            Image labelBg = NewRect("Etiqueta", template).gameObject.AddComponent<Image>();
            labelBg.sprite = rounded;
            labelBg.type = Image.Type.Sliced;
            labelBg.raycastTarget = false;
            RectTransform labelRt = labelBg.rectTransform;
            labelRt.pivot = new Vector2(0f, 0.5f);
            labelRt.anchoredPosition = new Vector2(120f, 0f);
            var labelLayout = labelBg.gameObject.AddComponent<HorizontalLayoutGroup>();
            labelLayout.padding = new RectOffset(36, 36, 18, 18);
            labelLayout.childControlWidth = labelLayout.childControlHeight = true;
            var labelFitter = labelBg.gameObject.AddComponent<ContentSizeFitter>();
            labelFitter.horizontalFit = labelFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Text label = NewRect("Texto", labelBg.transform).gameObject.AddComponent<Text>();
            label.font = font;
            label.fontSize = 90;
            label.fontStyle = FontStyle.Bold;
            label.text = "Marcação";
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            var view = template.gameObject.AddComponent<HighlightView>();
            SetRefs(view, ("dot", dot), ("labelBackground", labelBg), ("label", label));
            SetRefs(overlay, ("container", container), ("template", view));

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            return prefab;
        }

        // ---------- Marcações de exemplo ----------

        private static void FillExampleHighlights(InspectionFlow flow)
        {
            SerializedProperty steps = new SerializedObject(flow).FindProperty("steps");
            for (int i = 0; i < steps.arraySize; i++)
            {
                var step = steps.GetArrayElementAtIndex(i).objectReferenceValue as InspectionStep;
                if (step == null || step.Highlights.Count > 0)
                    continue;

                switch (step.RequirementId)
                {
                    case "RF01":
                        SetHighlights(step,
                            ("Solda OK", new Vector2(-3f, -9f), HighlightKind.Ok),
                            ("Verificar: solda fria?", new Vector2(3f, -9f), HighlightKind.Atencao));
                        break;
                    case "RF02":
                        SetHighlights(step,
                            ("TP1 · GND", new Vector2(-6f, -9f), HighlightKind.Info),
                            ("TP2 · [preencher]", new Vector2(0f, -9f), HighlightKind.Info),
                            ("TP3 · [preencher]", new Vector2(6f, -9f), HighlightKind.Info));
                        break;
                    case "RF03":
                        SetHighlights(step,
                            ("LED PWR [preencher]", new Vector2(9f, 3f), HighlightKind.Info),
                            ("LED STATUS [preencher]", new Vector2(9f, -1f), HighlightKind.Info),
                            ("Entrada de alimentação", new Vector2(-9f, -5f), HighlightKind.Atencao));
                        break;
                    case "RF04":
                        SetHighlights(step,
                            ("J1 · conector [preencher]", new Vector2(-9f, 1f), HighlightKind.Info),
                            ("SW1 · chave [preencher]", new Vector2(9f, -6f), HighlightKind.Atencao));
                        break;
                }
            }

            AssetDatabase.SaveAssets();
        }

        private static void SetHighlights(InspectionStep step, params (string label, Vector2 positionCm, HighlightKind kind)[] items)
        {
            var so = new SerializedObject(step);
            SerializedProperty list = so.FindProperty("highlights");
            list.arraySize = items.Length;
            for (int i = 0; i < items.Length; i++)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("label").stringValue = items[i].label;
                element.FindPropertyRelative("positionCm").vector2Value = items[i].positionCm;
                element.FindPropertyRelative("kind").enumValueIndex = (int)items[i].kind;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(step);
        }

        // ---------- Helpers ----------

        private static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        private static void SetRefs(Object target, params (string field, Object value)[] refs)
        {
            var so = new SerializedObject(target);
            foreach (var (field, value) in refs)
                so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
