using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Monitors a VR controller's rotation along a specified axis (Pitch, Yaw, or Roll).
// When the controller exceeds the rotation threshold, it notifies the ModuleActivityScheduler
// and destroys itself. Used as part of the tutorial activity system.

public class TiltController : MonoBehaviour, IActivityController
{
    private ModuleActivityScheduler mas => ModuleActivityScheduler.Instance;

    public InputActionReference inputAction;
    public float rotationThreshold = 25f;

    public enum RotationAxis { Pitch, Yaw, Roll }
    public RotationAxis monitoredAxis;

    private Quaternion initialRotation;
    private bool activityEnabled = false;
    private bool hasTriggered = false;
    private GameObject controllerHint; 
    public bool showControllerHint = false;

    private void OnEnable()
    {
        // Find the controller hint object in the scene
        ControllerHint[] hints = Resources.FindObjectsOfTypeAll<ControllerHint>();
        if (hints.Length > 0)
            controllerHint = hints[0].gameObject;

        if (inputAction != null)
        {
            inputAction.action.performed += OnRotationChanged;
            inputAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks or ghost callbacks after destruction
        if (inputAction != null)
        {
            inputAction.action.performed -= OnRotationChanged;
            inputAction.action.Disable();
        }
    }

    public void StartActivity()
    {
        activityEnabled = true;
        hasTriggered = false;
        StartCoroutine(CaptureInitialRotationDelayed());
        StartCoroutine(ShowHintNextFrame()); 
    }

    public void StopActivity()
    {
        StopAllCoroutines(); // stop any in-progress waits or timed steps so they don't carry over into the next section
        activityEnabled = false; // stop accepting controller input
        if (controllerHint != null) controllerHint.SetActive(false);
    }

    // Waits one frame before showing the hint to ensure the GameObject is ready
    private IEnumerator ShowHintNextFrame()
    {
        yield return null;
        if (showControllerHint && controllerHint != null)
        {
            controllerHint.SetActive(true);
            // Debug.Log("Hint shown");
        }
        else
        {
            // Debug.Log("controllerHint still null after waiting");
        }
    }

    private IEnumerator CaptureInitialRotationDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        if (inputAction != null)
        {
            initialRotation = inputAction.action.ReadValue<Quaternion>();
            // Debug.Log($"[TiltController] Captured neutral: {initialRotation.eulerAngles}");
        }
    }

    // Fires each time the controller rotation changes.
    // Computes the delta from the neutral rotation and checks if the threshold is exceeded.
    private void OnRotationChanged(InputAction.CallbackContext ctx)
    {
        if (!activityEnabled || hasTriggered) return;

        // Calculate how far the controller has rotated from its neutral position
        Quaternion currentRotation = ctx.ReadValue<Quaternion>();
        Vector3 deltaEuler = (Quaternion.Inverse(initialRotation) * currentRotation).eulerAngles;

        // Normalise each angle to the -180..180
        deltaEuler.x = NormalizeAngle(deltaEuler.x);
        deltaEuler.y = NormalizeAngle(deltaEuler.y);
        deltaEuler.z = NormalizeAngle(deltaEuler.z);

        // Pick the axis we're monitoring
        float valueToCheck = 0f;
        switch (monitoredAxis)
        {
            case RotationAxis.Pitch: valueToCheck = deltaEuler.x; break;
            case RotationAxis.Yaw:   valueToCheck = deltaEuler.y; break;
            case RotationAxis.Roll:  valueToCheck = deltaEuler.z; break;
        }

        // Threshold can be positive (tilt one way) or negative (tilt the other way)
        if ((rotationThreshold > 0 && valueToCheck >= rotationThreshold) || 
            (rotationThreshold < 0 && valueToCheck <= rotationThreshold))
        {
            hasTriggered = true;
            if (controllerHint != null) controllerHint.SetActive(false); 
            // Notify the scheduler that this step is done, then clean up
            mas.OnExternalStepCompleted();
            activityEnabled = false;
            Destroy(gameObject);
        }

        // Debug.Log($"dEuler = {deltaEuler} | valueToCheck({monitoredAxis}) = {valueToCheck}");
    }

    // Converts a 0..360 angle to -180..180 so threshold comparisons work intuitively
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}