using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// start menu controlled. scene selection empty 
public class StartMenu : MonoBehaviour
{
    //the play button in inspector
    [SerializeField] private Button playButton;

    private string selectedScene = "";

    void Start()
    {
        //when game is started, play button grayed out until scene chosen
        if (playButton != null)
        {
            playButton.interactable = false;
        }
    }

    //linked to tut button. once tut module clicekd the play button clickable
    public void SelectTutorial()
    {
        selectedScene = "v1.2";

        if (playButton != null)
        {
            playButton.interactable = true;
        }
    }

    //same as tut but for spinning stalling module
    public void SelectSpinningStalling()
    {
        selectedScene = "MainVRScene";

        if (playButton != null)
        {
            playButton.interactable = true;
        }
    }

    //do nothing if no scene is selected, if selected load the selected scene
    public void PlaySelectedScene()
    {
        if (string.IsNullOrEmpty(selectedScene))
        {
            return;
        }

        SceneManager.LoadScene(selectedScene);
    }
}