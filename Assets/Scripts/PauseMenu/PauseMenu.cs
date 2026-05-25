using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    // the container is just the thing we referenced in unity as the object in the canvas - Randy
    public GameObject container;
    [SerializeField] private GameObject pauseMenuScreen; // Mahir - explicit reference so it can be forced active on each open

    [SerializeField] private float menuDistance = 4f; // Mahir - how far in front of the player the menu appears
    [SerializeField] private float menuHeightOffset = 1.5f; // Mahir - how high above eye level the menu appears
    private bool isPaused = false; // Mahir
    private bool prevPressed = false; // Mahir - tracks previous button state for manual edge detection

    private InputAction togglePauseAction; // Mahir - found at runtime so no serialized reference is needed

    void Awake()
    {
        // Mahir - find TogglePause action by searching all loaded InputActionAssets rather than using a
        // serialized reference, which would be lost whenever Unity reloads the scene file
        foreach (var asset in Resources.FindObjectsOfTypeAll<InputActionAsset>())
        {
            var action = asset.FindAction("VRControls/TogglePause");
            if (action != null)
            {
                togglePauseAction = action;
                break;
            }
        }
    }

    void OnEnable()
    {
        togglePauseAction?.Enable();
    }

    void OnDisable()
    {
        togglePauseAction?.Disable();
    }

    // Mahir - manual rising-edge detection so toggle works reliably across multiple presses
    void Update()
    {
        if (togglePauseAction == null) return;

        bool isPressed = togglePauseAction.ReadValue<float>() > 0.5f;
        if (isPressed && !prevPressed)
        {
            if (isPaused)
                ResumeButton();
            else
                PauseButton();
        }
        prevPressed = isPressed;
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
        if (pauseMenuScreen != null) pauseMenuScreen.SetActive(true); // Mahir - force active in case it got deactivated on previous close
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
