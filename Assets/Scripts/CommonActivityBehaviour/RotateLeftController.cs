using UnityEngine;
using UnityEngine.InputSystem;

/**
@author Chintana Luu | luucy003@mymail.unisa.edu.au
*/
public class RotateLeftController : MonoBehaviour, IActivityController
{
    private ModuleActivityScheduler mas => ModuleActivityScheduler.Instance; // Reference to MAS singleton

    public InputActionReference rotateAction; // Input action for rotating the left controller
    public float rotationThreshold = 0.1f; // Default, can be changed in inspector
    private bool activityEnabled = false; // Start as disabled

    // Subscribe to event listener and enable input action
    private void OnEnable()
    {
        if (rotateAction != null)
        {
            rotateAction.action.performed += OnRotatePerformed;
            rotateAction.action.Enable();
        }
    }

    // Unsubscribe to event listener and disable input action
    private void OnDisable()
    {
        if (rotateAction != null)
        {
            rotateAction.action.performed -= OnRotatePerformed;
            rotateAction.action.Disable();
        }
    }

    // 
    public void StartActivity()
    {
        activityEnabled = true;
//      rotateAction.action.Enable();
    }




    /**
    Rotate Method

    This method is subscribed onto the action performed in the OnEnable method and will execute when the controller is rotated.

    and this is an extra line
    @param nothing this is literally nothing
    @param this doesnt exist, literall the param this, does. not. exist.
    @return no why would you think this returns something bozo
    */
    private void OnRotatePerformed(InputAction.CallbackContext ctx)
    {
        if (!activityEnabled) return; // Only process if the activity is...y'know, active...

        float rotationAmount = ctx.ReadValue<float>();// Get the value from the controller
        
        if (rotationAmount >= rotationThreshold)
        {

            AxisRotationController aoaManip = GameObject.FindAnyObjectByType<AxisRotationController>();
            aoaManip.LerpAOA(-10); // Lower AOA by 10 degrees. // this value may be needed to change if you want to reuse the script for multiple rotation angles.

            mas.OnExternalStepCompleted();

            // This is a one-action activity, so disable after completed input
            activityEnabled = false;

            // Clean-up
            Destroy(gameObject);
        }
    }

}