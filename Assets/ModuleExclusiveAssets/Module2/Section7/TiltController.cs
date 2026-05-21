using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    private GameObject controllerHint; // ADD THIS
    public bool showControllerHint = false;

    private void OnEnable()
    {
        // ADD THIS - find hint on enable
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
        StartCoroutine(ShowHintNextFrame()); // ADD THIS
    }

    public void StopActivity()
    {
        StopAllCoroutines();
        activityEnabled = false;
        if (controllerHint != null) controllerHint.SetActive(false); // ADD THIS
    }

    private IEnumerator ShowHintNextFrame() // ADD THIS
    {
        yield return null;
        if (showControllerHint && controllerHint != null)
        {
            controllerHint.SetActive(true);
            Debug.Log("Hint shown");
        }
        else
        {
            Debug.Log("controllerHint still null after waiting");
        }
    }

    private IEnumerator CaptureInitialRotationDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        if (inputAction != null)
        {
            initialRotation = inputAction.action.ReadValue<Quaternion>();
            Debug.Log($"[TiltController] Captured neutral: {initialRotation.eulerAngles}");
        }
    }

    private void OnRotationChanged(InputAction.CallbackContext ctx)
    {
        if (!activityEnabled || hasTriggered) return;

        Quaternion currentRotation = ctx.ReadValue<Quaternion>();
        Vector3 deltaEuler = (Quaternion.Inverse(initialRotation) * currentRotation).eulerAngles;

        deltaEuler.x = NormalizeAngle(deltaEuler.x);
        deltaEuler.y = NormalizeAngle(deltaEuler.y);
        deltaEuler.z = NormalizeAngle(deltaEuler.z);

        float valueToCheck = 0f;
        switch (monitoredAxis)
        {
            case RotationAxis.Pitch: valueToCheck = deltaEuler.x; break;
            case RotationAxis.Yaw:   valueToCheck = deltaEuler.y; break;
            case RotationAxis.Roll:  valueToCheck = deltaEuler.z; break;
        }

        if ((rotationThreshold > 0 && valueToCheck >= rotationThreshold) || 
            (rotationThreshold < 0 && valueToCheck <= rotationThreshold))
        {
            hasTriggered = true;
            if (controllerHint != null) controllerHint.SetActive(false); // ADD THIS
            mas.OnExternalStepCompleted();
            activityEnabled = false;
            Destroy(gameObject);
        }

        Debug.Log($"dEuler = {deltaEuler} | valueToCheck({monitoredAxis}) = {valueToCheck}");
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}