using UnityEngine;

// Mahir - Anchors a world-space canvas above the named plane and faces it toward the camera.
// Kinda over-engineered but looks quite nice on the headset over just doing the prefab
public class AircraftLabelPositioner : MonoBehaviour
{
    [SerializeField] private string planeName = "DA_40";
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 5f, 0f);

    private Transform planeTransform;
    private Camera mainCamera;

    void OnEnable()
    {
        mainCamera = Camera.main;

        // World Space canvas needs worldCamera set at runtime (can't reference scene camera from prefab)
        var canvas = GetComponentInChildren<Canvas>(true);
        if (canvas != null && mainCamera != null)
            canvas.worldCamera = mainCamera;

        // Try to find the plane immediately; LateUpdate will retry each frame if it's still inactive
        TryFindPlane();
    }

    void LateUpdate()
    {
        if (planeTransform == null)
            TryFindPlane();

        if (planeTransform != null)
            transform.position = planeTransform.position + worldOffset;

        if (mainCamera != null)
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        else
            mainCamera = Camera.main;
    }

    void TryFindPlane()
    {
        foreach (var t in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (t.gameObject.scene.IsValid() && t.name == planeName)
            {
                planeTransform = t;
                return;
            }
        }
    }
}
