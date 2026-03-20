using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

/** 
Handles menu selection of each Module alongside a Play button to trigger Modules via the ModuleManager.

@author Caleb Martin | marcy066@mymail.unisa.edu.au
*/

public class ModuleMenuSelection : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] public Button playButton; /**< The interactible Play button GameObject. */
    [SerializeField] public Button module1Button; /**< The button for the first Module. */
    [SerializeField] public Button module2Button; /**< The button for the second Module. */
    [SerializeField] public Button module3Button; /**< The button for the third Module. */
    [SerializeField] public Button backButton; /**< The interactible Back button GameObject. */

    [Header("Module Manager")]
    [SerializeField] private ModuleManager moduleManager; /**< Reference to the ModuleManager. */

    private int selectedModule = -1; /**< Index for the currently selected Module, defaulting to -1 where no Module is selected. */

    void Start()
    {
        // No modules selected to start with, play button disabled
        playButton.interactable = false;

        // Buttons will call SelectModule passing in their unique index when clicked
        //module1Button.onClick.AddListener(() => SelectModule(0));
        module2Button.onClick.AddListener(() => SelectModule(1));
        //module3Button.onClick.AddListener(() => SelectModule(2));

        // Methods called when play or back buttons are clicked
        playButton.onClick.AddListener(OnPlayPressed);
        backButton.onClick.AddListener(OnBackPressed);
    }

    /**
    Changes the currently selected Module to a new one.
    @param moduleIndex The Module to be selected.
    @return void
    */
    void SelectModule(int moduleIndex)
    {
        // Update selected module
        selectedModule = moduleIndex;

        // Allow the play button to be interacted with
        playButton.interactable = true;
    }

    /**
    Called by the Play button, calls the ModuleManager to initiate the selected Module.
    @return void
    */
    void OnPlayPressed()
    {
        // No module selected, do nothing
        if (selectedModule == -1) return;

        // Disable the menu and reset state of menu
        playButton.interactable = false;
        gameObject.SetActive(false);

        // Play the selected module timeline by talking to ModuleManager
        moduleManager.PlayModule(selectedModule);
        selectedModule = -1;
    }

    /**
    When the Back button is pressed, reset the selected Module back to -1 as default.
    @return void
    */
    void OnBackPressed()
    {
        // Reset state of menu
        selectedModule = -1;
        playButton.interactable = false;
    }
}
