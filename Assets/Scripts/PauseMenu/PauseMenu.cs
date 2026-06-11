using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    // the container is just the thing we referenced in unity as the object in the canvas - Randy
    public GameObject container;
    [SerializeField] private GameObject pauseMenuScreen; // Mahir - explicit reference so it can be active on each open

    [SerializeField] private float menuDistance = 4f; // Mahir - how far in front of the player the menu appears
    [SerializeField] private float menuHeightOffset = 1.5f; // Mahir - how high above eye level the menu appears
    private bool isPaused = false; // Mahir

    [SerializeField] private InputActionReference togglePauseAction;

    void OnEnable()
    {
        if (togglePauseAction != null)
        {
            togglePauseAction.action.performed += OnTogglePause;
            togglePauseAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (togglePauseAction != null)
        {
            togglePauseAction.action.performed -= OnTogglePause;
            togglePauseAction.action.Disable();
        }
    }

    // TogglePause is a Button action, so the Input System fires "performed" exactly once per
    // press (the rising edge) - no manual polling or edge tracking needed.
    private void OnTogglePause(InputAction.CallbackContext ctx)
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
        }

        container.SetActive(true); // Mahir
        if (pauseMenuScreen != null) pauseMenuScreen.SetActive(true); // Mahir - force active in case it got deactivated on previous close, shouldnt reallly happen tho
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

    // Called by ModuleManager when switching sections to reset pause state without user interaction
    public void ForceResume()
    {
        if (isPaused) ResumeButton();
    }

    /* will make it so it resumes timescale for the scene but also just restarts the whole scene
    i think that its a bit difficult that this prototype is made in one whole scene but maybe we can create levels later down the line -Randy
    */
    public void MainMenuButton() // Randy - //Amir have updated to load menu start
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuStart");
    }

    public void ExitButton() // Randy
    {
        Application.Quit();
    }
}
