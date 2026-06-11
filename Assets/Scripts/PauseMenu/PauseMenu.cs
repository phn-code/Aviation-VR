using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    // the container is just the thing we referenced in unity as the object in the canvas - Randy
    public GameObject container;
    [SerializeField] private GameObject pauseMenuScreen; 

    [SerializeField] private float menuDistance = 4f; 
    [SerializeField] private float menuHeightOffset = 1.5f;
    private bool isPaused = false; 

    [SerializeField] private InputActionReference togglePauseAction;

    void OnEnable() // randy
    {
        if (togglePauseAction != null)
        {
            togglePauseAction.action.performed += OnTogglePause;
            togglePauseAction.action.Enable();
        }
    }

    void OnDisable() // randy
    {
        if (togglePauseAction != null)
        {
            togglePauseAction.action.performed -= OnTogglePause;
            togglePauseAction.action.Disable();
        }
    }

    private void OnTogglePause(InputAction.CallbackContext ctx) // randy
    {
        if (isPaused)
            ResumeButton();
        else
            PauseButton();
    }

    public void PauseButton() // randy and mahir
    {
        //  position the canvas right in front of wherever the player is looking, facing toward them - mahir
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 menuPos = cam.transform.position + cam.transform.forward * menuDistance + Vector3.up * menuHeightOffset;
            transform.position = menuPos;
        }

        container.SetActive(true); 
        if (pauseMenuScreen != null) pauseMenuScreen.SetActive(true); // force active in case it got deactivated on previous close, shouldnt reallly happen tho
        isPaused = true; 
        Time.timeScale = 0; 
        AudioListener.pause = true; 
    }

    public void ResumeButton() // randy and mahir
    {
        container.SetActive(false);
        isPaused = false; 
        Time.timeScale = 1; 
        AudioListener.pause = false; 
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
