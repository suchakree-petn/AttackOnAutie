using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFitBG : MonoBehaviour
{
    public SpriteRenderer[] backgrounds;

    void Start()
    {
        FitCameraToCombinedBackgrounds();
    }

    [Button]
    void FitCameraToCombinedBackgrounds()
    {
        if (backgrounds == null || backgrounds.Length == 0) return;

        Bounds combinedBounds = backgrounds[0].bounds;
        for (int i = 1; i < backgrounds.Length; i++)
        {
            combinedBounds.Encapsulate(backgrounds[i].bounds);
        }

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetWidth = combinedBounds.size.x;
        float camOrthoSize = (targetWidth / screenRatio) / 2f;

        Camera cam = Camera.main;
        cam.orthographicSize = camOrthoSize;

        float cameraHeight = cam.orthographicSize * 2f;
        float bgBottom = combinedBounds.min.y;
        float targetY = bgBottom + cameraHeight / 2f;

        cam.transform.position = new Vector3(
            combinedBounds.center.x,
            targetY,
            cam.transform.position.z
        );
    }
}
