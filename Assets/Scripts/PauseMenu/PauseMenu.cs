using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Mahir - added for controller input

public class PauseMenu : MonoBehaviour
{
    // the container is just the thing we referenced in unity as the object in the canvas - Randy
    public GameObject container;

    [SerializeField] private InputActionReference pauseAction; // Mahir - A button on right controller
    [SerializeField] private float menuDistance = 4f; // Mahir - how far in front of the player the menu appears
    [SerializeField] private float menuHeightOffset = 1.5f; // Mahir - how high above eye level the menu appears
    private bool isPaused = false; // Mahir

    // Mahir - subscribe to A button on enable
    void OnEnable()
    {
        pauseAction.action.performed += OnPausePressed;
        pauseAction.action.Enable();
    }

    // Mahir - unsubscribe on disable
    void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;
        pauseAction.action.Disable();
    }

    // Mahir - called when the A button is pressed, toggles pause on and off
    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (isPaused)
            ResumeButton();
        else
            PauseButton();
    }

    public void PauseButton()
    {
        // Mahir - position the canvas right in front of wherever the player is looking, facing toward them
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 menuPos = cam.transform.position + cam.transform.forward * menuDistance + Vector3.up * menuHeightOffset;
            transform.position = menuPos;
            transform.rotation = Quaternion.LookRotation(cam.transform.position - menuPos, Vector3.up);
        }

        container.SetActive(true); // Mahir
        isPaused = true; // Mahir
        Time.timeScale = 0; // Randy
        AudioListener.pause = true; // Mahir
    }

    public void ResumeButton()
    {
        container.SetActive(false); // Mahir
        isPaused = false; // Mahir
        Time.timeScale = 1; // Randy
        AudioListener.pause = false; // Mahir
    }

    /* will make it so it resumes timescale for the scene but also just restarts the whole scene
    i think that its a bit difficult that this prototype is made in one whole scene but maybe we can create levels later down the line -Randy
    */
    public void MainMenuButton() // Randy
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainVRScene");
    }

    public void ExitButton() // Randy
    {
        Application.Quit();
    }
}
