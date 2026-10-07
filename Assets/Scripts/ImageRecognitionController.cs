using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Facens.WL01
{
    /// <summary>
    /// White label didatico para reconhecimento e tracking de imagens.
    /// Quando uma imagem de referencia e detectada, cria um cubo como filho
    /// do ARTrackedImage para manter o conteudo registrado ao marcador.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ImageRecognitionController : MonoBehaviour
    {
        [SerializeField] private ARTrackedImageManager trackedImageManager;
        [SerializeField] private Vector3 demoScale = new Vector3(0.05f, 0.05f, 0.05f);

        private readonly Dictionary<TrackableId, GameObject> contentByTrackable = new();

        private void Reset()
        {
            trackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        private void Awake()
        {
            if (trackedImageManager == null)
                trackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
#pragma warning disable CS0618
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
#pragma warning restore CS0618
        }

        private void OnDisable()
        {
#pragma warning disable CS0618
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
#pragma warning restore CS0618
        }

#pragma warning disable CS0618
        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
        {
            foreach (ARTrackedImage trackedImage in args.added)
                CreateOrUpdateContent(trackedImage);

            foreach (ARTrackedImage trackedImage in args.updated)
                CreateOrUpdateContent(trackedImage);

            foreach (ARTrackedImage trackedImage in args.removed)
            {
                if (contentByTrackable.TryGetValue(trackedImage.trackableId, out GameObject content))
                {
                    Destroy(content);
                    contentByTrackable.Remove(trackedImage.trackableId);
                }
            }
        }
#pragma warning restore CS0618

        private void CreateOrUpdateContent(ARTrackedImage trackedImage)
        {
            if (!contentByTrackable.TryGetValue(trackedImage.trackableId, out GameObject content))
            {
                content = CreateDemoContent(trackedImage.transform);
                contentByTrackable.Add(trackedImage.trackableId, content);
            }

            bool isTracking = trackedImage.trackingState == TrackingState.Tracking;
            content.SetActive(isTracking);
        }

        private GameObject CreateDemoContent(Transform parent)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "WL01_DemoContent";
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localScale = demoScale;
            cube.AddComponent<TapToToggleScale>();
            return cube;
        }
    }
}
