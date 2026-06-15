using UnityEngine;
using UnityEngine.InputSystem;

/** 
Handles the toggling of the ghost trail belonging to the DA-40 aircraft on and off.

@author Caleb Martin | marcy066@mymail.unisa.edu.au
*/

public class GhostTrailToggle : MonoBehaviour
{
    public ParticleSystem GhostTrail; /**< The ghost trail particle system. */

    private bool ghostTrailEnabled = false; /**< Flag determining if the ghost trail is currently enabled or not. */

    public InputActionReference toggleAction; /**< Reference to the InputAction that dictates the toggling of the ghost trail. */

    /**
    Code to be executed at runtime, subscribes to input action so that ghost trail can be toggled on and off.
    @return void
    */

    // Hard rule: the ghost trail must ONLY play when the user toggles it on from the controller.
    // Play-On-Awake is ruled out, so something else (a Timeline Control track in this section) is
    // calling Play() on it directly. Enforce the state every frame in LateUpdate, which runs AFTER
    // Timeline/Directors evaluate: if we're logically off but something started it, stop and clear
    // it the same frame so it never renders.
    private void LateUpdate()
    {
        if (!ghostTrailEnabled && GhostTrail != null && GhostTrail.isPlaying)
            GhostTrail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    //RANDY function made for button to toggle back and forth for the ghost trail
    public void ToggleGhostTrail()
    {
        ghostTrailEnabled = !ghostTrailEnabled;

        if (ghostTrailEnabled)
        {
            GhostTrail.Play();
        }
        else
        {
            GhostTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
    void OnEnable()
    {
        toggleAction.action.performed += OnTogglePressed;
        toggleAction.action.Enable();
    }

    /**
    Code to be executed at termination of application, unsubscribes to input action.
    @return void
    */
    void OnDisable()
    {
        toggleAction.action.performed -= OnTogglePressed;
        toggleAction.action.Disable();
    }

    /**
    Handles the toggling of the ghost trail on and off.
    @param ctx The callback context provided by the InputAction to interpret input.
    @return void
    */
    private void OnTogglePressed(InputAction.CallbackContext ctx)
    {
        ghostTrailEnabled = !ghostTrailEnabled;

        if (ghostTrailEnabled) // Start trail
        {
            GhostTrail.Play();
        }
        else // Stop trail
        {
            GhostTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
