using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;

    private string selectedScene = "";

    void Start()
    {
        if (playButton != null)
        {
            playButton.interactable = false;
        }
    }

    public void SelectTutorial()
    {
        selectedScene = "v1.2";

        if (playButton != null)
        {
            playButton.interactable = true;
        }
    }

    public void SelectSpinningStalling()
    {
        selectedScene = "MainVRScene";

        if (playButton != null)
        {
            playButton.interactable = true;
        }
    }

    public void PlaySelectedScene()
    {
        if (string.IsNullOrEmpty(selectedScene))
        {
            return;
        }

        SceneManager.LoadScene(selectedScene);
    }
}