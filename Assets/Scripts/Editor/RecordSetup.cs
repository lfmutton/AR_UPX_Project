using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Facens.WL01.EditorTools
{
    /// <summary>
    /// Menu "B'Energy > Fase 4 - Criar checklist e medições": adiciona ao painel da etapa
    /// as abas Guia/Registro, o checklist (RF06) e os campos de medição (RF07), e preenche
    /// itens de exemplo nas etapas que ainda não têm nenhum.
    /// </summary>
    public static class RecordSetup
    {
        private static Font font;
        private static Sprite rounded;

        [MenuItem("B'Energy/Fase 4 - Criar checklist e medições")]
        private static void Run()
        {
            var stepPanel = Object.FindFirstObjectByType<StepPanel>(FindObjectsInactive.Include);
            var flow = Object.FindFirstObjectByType<InspectionFlow>();
            if (stepPanel == null || flow == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Abra a cena WL01RecognitionAR e rode a Fase 2 antes.", "OK");
                return;
            }

            var panelSo = new SerializedObject(stepPanel);
            if (panelSo.FindProperty("recordGroup").objectReferenceValue != null)
            {
                EditorUtility.DisplayDialog("B'Energy", "O checklist e as medições já existem nesta cena. Nada foi alterado.", "OK");
                return;
            }

            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

            var panel = ((GameObject)panelSo.FindProperty("panelRoot").objectReferenceValue).transform;
            var title = (Text)panelSo.FindProperty("titleText").objectReferenceValue;
            var instruction = (Text)panelSo.FindProperty("instructionText").objectReferenceValue;
            var cards = (Transform)panelSo.FindProperty("cardsContainer").objectReferenceValue;

            // Abas
            RectTransform tabs = NewRect("Abas", panel);
            tabs.SetSiblingIndex(title.transform.GetSiblingIndex() + 1);
            var tabsLayout = tabs.gameObject.AddComponent<HorizontalLayoutGroup>();
            tabsLayout.spacing = 20f;
            tabsLayout.childControlWidth = tabsLayout.childControlHeight = true;
            tabsLayout.childForceExpandWidth = true;
            tabsLayout.childForceExpandHeight = false;
            Button guideTab = NewButton("Aba Guia", tabs, "Guia", Hex("#2563EB"), 100f, 36, out _);
            Button recordTab = NewButton("Aba Registro", tabs, "Registro", Hex("#1B2A4A"), 100f, 36, out Text recordTabLabel);

            // Grupo Guia: move a instrução e os cards existentes para dentro
            RectTransform guide = NewVertical("Grupo Guia", panel, 28f);
            guide.SetSiblingIndex(tabs.GetSiblingIndex() + 1);
            Undo.SetTransformParent(instruction.transform, guide, "Mover instrução");
            Undo.SetTransformParent(cards, guide, "Mover cards");

            // Grupo Registro
            RectTransform record = NewVertical("Grupo Registro", panel, 28f);
            record.SetSiblingIndex(guide.GetSiblingIndex() + 1);
            RectTransform checklist = NewVertical("Checklist", record, 12f);
            ChecklistItemView checklistTemplate = BuildChecklistTemplate(checklist);
            RectTransform measurements = NewVertical("Medicoes", record, 24f);
            MeasurementFieldView measurementTemplate = BuildMeasurementTemplate(measurements);
            record.gameObject.SetActive(false);

            foreach (Object created in new Object[] { tabs.gameObject, guide.gameObject, record.gameObject })
                Undo.RegisterCreatedObjectUndo(created, "Criar registro");

            panelSo.FindProperty("tabsRoot").objectReferenceValue = tabs.gameObject;
            panelSo.FindProperty("guideTab").objectReferenceValue = guideTab;
            panelSo.FindProperty("recordTab").objectReferenceValue = recordTab;
            panelSo.FindProperty("recordTabLabel").objectReferenceValue = recordTabLabel;
            panelSo.FindProperty("guideGroup").objectReferenceValue = guide.gameObject;
            panelSo.FindProperty("recordGroup").objectReferenceValue = record.gameObject;
            panelSo.FindProperty("checklistContainer").objectReferenceValue = checklist;
            panelSo.FindProperty("checklistTemplate").objectReferenceValue = checklistTemplate;
            panelSo.FindProperty("measurementsContainer").objectReferenceValue = measurements;
            panelSo.FindProperty("measurementTemplate").objectReferenceValue = measurementTemplate;
            panelSo.ApplyModifiedProperties();

            FillExampleRecords(flow);

            EditorSceneManager.MarkSceneDirty(stepPanel.gameObject.scene);
            EditorUtility.DisplayDialog("B'Energy",
                "Abas Guia/Registro, checklist e medições criados.\n\nSalve a cena com Ctrl+S.\n\nOs itens de exemplo ficam em cada etapa (Assets/Etapas), campos Checklist e Measurements.",
                "OK");
        }

        // ---------- Modelos ----------

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
            var boxElement = box.gameObject.AddComponent<LayoutElement>();
            boxElement.minWidth = boxElement.preferredWidth = 72f;
            boxElement.minHeight = boxElement.preferredHeight = 72f;

            Image check = NewRect("Marca", box.transform).gameObject.AddComponent<Image>();
            check.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
            check.color = Hex("#60A5FA");
            check.raycastTarget = false;
            Stretch(check.rectTransform, 8f);

            Text label = NewText("Texto", row.transform, "Item do checklist", 38, FontStyle.Normal, Hex("#D4D8DD"));
            label.alignment = TextAnchor.MiddleLeft;
            label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var toggle = row.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = box;
            toggle.graphic = check;
            toggle.isOn = false;

            var view = row.gameObject.AddComponent<ChecklistItemView>();
            SetRefs(view, ("toggle", toggle), ("label", label));
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
            Stretch(fieldText.rectTransform, 28f);

            Text placeholder = NewText("Placeholder", fieldBg.transform, "0,00", 40, FontStyle.Italic, Hex("#6B7280"));
            placeholder.alignment = TextAnchor.MiddleLeft;
            Stretch(placeholder.rectTransform, 28f);

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
            SetRefs(view, ("label", label), ("input", input), ("unit", unit), ("status", status));
            return view;
        }

        // ---------- Itens de exemplo ----------

        private static void FillExampleRecords(InspectionFlow flow)
        {
            SerializedProperty steps = new SerializedObject(flow).FindProperty("steps");
            for (int i = 0; i < steps.arraySize; i++)
            {
                var step = steps.GetArrayElementAtIndex(i).objectReferenceValue as InspectionStep;
                if (step == null || step.HasRecord)
                    continue;

                var so = new SerializedObject(step);
                switch (step.RequirementId)
                {
                    case "RF01":
                        SetChecklist(so, "Inspecionei todas as soldas destacadas",
                            "Nenhuma solda fria ou ponte de solda",
                            "[preencher: critério da equipe]");
                        break;
                    case "RF02":
                        SetChecklist(so, "Multímetro em modo continuidade",
                            "Sem curto entre alimentação e GND");
                        AddMeasurement(so, "Resistência TP2–GND [preencher]", "Ω", false, 0f, 0f);
                        break;
                    case "RF03":
                        SetChecklist(so, "Fonte com limite de corrente configurado",
                            "LED PWR aceso");
                        AddMeasurement(so, "Tensão no trilho de 5 V (exemplo)", "V", true, 4.75f, 5.25f);
                        AddMeasurement(so, "Corrente consumida [preencher]", "mA", false, 0f, 0f);
                        break;
                    case "RF04":
                        SetChecklist(so, "Conectores encaixados",
                            "Chave SW1 acionada",
                            "LED STATUS conforme esperado [preencher]");
                        break;
                }
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(step);
            }

            AssetDatabase.SaveAssets();
        }

        private static void SetChecklist(SerializedObject so, params string[] items)
        {
            SerializedProperty list = so.FindProperty("checklist");
            list.arraySize = items.Length;
            for (int i = 0; i < items.Length; i++)
                list.GetArrayElementAtIndex(i).stringValue = items[i];
        }

        private static void AddMeasurement(SerializedObject so, string label, string unit, bool hasRange, float min, float max)
        {
            SerializedProperty list = so.FindProperty("measurements");
            list.arraySize++;
            SerializedProperty element = list.GetArrayElementAtIndex(list.arraySize - 1);
            element.FindPropertyRelative("label").stringValue = label;
            element.FindPropertyRelative("unit").stringValue = unit;
            element.FindPropertyRelative("hasRange").boolValue = hasRange;
            element.FindPropertyRelative("min").floatValue = min;
            element.FindPropertyRelative("max").floatValue = max;
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

        private static Button NewButton(string name, Transform parent, string label, Color color, float height, int fontSize, out Text labelText)
        {
            Image image = NewImage(name, parent, color);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var element = image.gameObject.AddComponent<LayoutElement>();
            element.preferredHeight = height;
            element.flexibleWidth = 1f;

            labelText = NewText("Texto", image.transform, label, fontSize, FontStyle.Bold, Color.white);
            labelText.alignment = TextAnchor.MiddleCenter;
            Stretch(labelText.rectTransform, 0f);
            return button;
        }

        private static void Stretch(RectTransform rt, float padding)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(padding, padding);
            rt.offsetMax = new Vector2(-padding, -padding);
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
