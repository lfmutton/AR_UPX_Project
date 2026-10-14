using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Facens.WL01.EditorTools
{
    /// <summary>
    /// Menu "B'Energy > Fase 7 - Colocar placa 3D sobre o marcador": coloca o modelo da VCU
    /// (Assets/Models/VCU/VCU.fbx) dentro do prefab StepOverlay, deitado, em tamanho real e
    /// centralizado no marcador. Mede o modelo para corrigir orientação e escala, já que o
    /// FBX passou por Altium → GLB → FBX.
    /// </summary>
    public static class ModelSetup
    {
        private const string ModelPath = "Assets/Models/VCU/VCU.fbx";
        private const string OverlayPath = "Assets/Prefabs/Overlays/StepOverlay.prefab";
        private const float BoardWidth = 0.085f;   // 85 mm
        private const float BoardLength = 0.098f;  // 98 mm (sem o conector)

        [MenuItem("B'Energy/Fase 7 - Colocar placa 3D sobre o marcador")]
        private static void Run()
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            if (model == null)
            {
                EditorUtility.DisplayDialog("B'Energy", $"Não encontrei {ModelPath}.", "OK");
                return;
            }

            GameObject overlay = PrefabUtility.LoadPrefabContents(OverlayPath);
            if (overlay == null)
            {
                EditorUtility.DisplayDialog("B'Energy", "Rode a Fase 3 antes (prefab StepOverlay).", "OK");
                return;
            }

            if (overlay.transform.Find("Placa VCU") != null)
            {
                PrefabUtility.UnloadPrefabContents(overlay);
                EditorUtility.DisplayDialog("B'Energy", "A placa 3D já está no StepOverlay. Nada foi alterado.", "OK");
                return;
            }

            // Pivô = centro da placa sobre o centro do marcador
            var pivot = new GameObject("Placa VCU").transform;
            pivot.SetParent(overlay.transform, false);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model, pivot);
            Transform t = instance.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            // 1) Deitar: o eixo mais fino (altura dos componentes) tem que ser o Y
            Bounds b = WorldBounds(instance);
            Vector3 s = b.size;
            if (s.x < s.y && s.x < s.z)
                t.localRotation = Quaternion.Euler(0f, 0f, 90f) * t.localRotation;
            else if (s.z < s.y && s.z < s.x)
                t.localRotation = Quaternion.Euler(90f, 0f, 0f) * t.localRotation;

            // 2) Escala real: o lado menor da placa no chão é a largura (85 mm)
            b = WorldBounds(instance);
            float width = Mathf.Min(b.size.x, b.size.z);
            t.localScale *= BoardWidth / width;

            // 3) Largura no eixo X (lado comprido no Z)
            b = WorldBounds(instance);
            if (b.size.x > b.size.z)
                t.localRotation = Quaternion.Euler(0f, 90f, 0f) * t.localRotation;

            // 4) Componentes para cima: a mediana dos vértices fica perto da superfície da placa
            //    (muitos SMDs baixos); acima dela as peças vão bem mais longe (conector ~28 mm)
            //    do que abaixo (~6,5 mm). Se estiver ao contrário, vira a placa.
            List<Vector3> points = WorldVertices(instance);
            b = WorldBounds(instance);
            float surface = Median(points, p => p.y);
            if (b.max.y - surface < surface - b.min.y)
                t.localRotation = Quaternion.Euler(0f, 0f, 180f) * t.localRotation;

            // 5) Conector (a peça mais alta) para o lado de baixo da imagem (-Z)
            points = WorldVertices(instance);
            b = WorldBounds(instance);
            float tallLimit = b.min.y + b.size.y * 0.6f;
            float tallZ = Average(points.FindAll(p => p.y > tallLimit), p => p.z);
            if (tallZ > b.center.z)
                t.localRotation = Quaternion.Euler(0f, 180f, 0f) * t.localRotation;

            // 6) Centralizar a PCB (sem o conector) no marcador, com a face de baixo da PCB em y = 0
            points = WorldVertices(instance);
            b = WorldBounds(instance);
            float topEdge = b.max.z;
            List<Vector3> nearTop = points.FindAll(p => p.z > topEdge - 0.004f);
            float pcbBottom = nearTop.Count > 0 ? Min(nearTop, p => p.y) : b.min.y;
            Vector3 pcbCenter = new Vector3(b.center.x, pcbBottom, topEdge - BoardLength / 2f);
            t.position -= pcbCenter;

            // As marcações ficam logo acima da superfície da PCB
            Transform canvas = overlay.transform.Find("Canvas");
            if (canvas != null)
                canvas.localPosition = new Vector3(0f, 0.0025f, 0f);

            PrefabUtility.SaveAsPrefabAsset(overlay, OverlayPath);
            PrefabUtility.UnloadPrefabContents(overlay);

            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(OverlayPath));
            EditorUtility.DisplayDialog("B'Energy",
                "Placa 3D colocada no StepOverlay, centralizada no marcador.\n\n" +
                "O prefab foi aberto para você conferir: a placa deve estar deitada, com os componentes para cima e o conector azul para o lado de baixo (-Z).\n\n" +
                "Depois é só fazer o Build and Run.",
                "OK");
        }

        private static Bounds WorldBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer r in renderers)
                bounds.Encapsulate(r.bounds);
            return bounds;
        }

        private static List<Vector3> WorldVertices(GameObject go)
        {
            var points = new List<Vector3>();
            foreach (MeshFilter filter in go.GetComponentsInChildren<MeshFilter>())
            {
                Mesh mesh = filter.sharedMesh;
                if (mesh == null)
                    continue;
                Matrix4x4 m = filter.transform.localToWorldMatrix;
                Vector3[] vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i += 4) // amostra: suficiente para medir
                    points.Add(m.MultiplyPoint3x4(vertices[i]));
            }
            return points;
        }

        private static float Average(List<Vector3> points, System.Func<Vector3, float> axis)
        {
            if (points.Count == 0)
                return 0f;
            float sum = 0f;
            foreach (Vector3 p in points)
                sum += axis(p);
            return sum / points.Count;
        }

        private static float Median(List<Vector3> points, System.Func<Vector3, float> axis)
        {
            var values = points.ConvertAll(p => axis(p));
            values.Sort();
            return values.Count > 0 ? values[values.Count / 2] : 0f;
        }

        private static float Min(List<Vector3> points, System.Func<Vector3, float> axis)
        {
            float min = float.MaxValue;
            foreach (Vector3 p in points)
                min = Mathf.Min(min, axis(p));
            return min;
        }
    }
}
