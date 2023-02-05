using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject helpPanel;

    void Start()
    {
        creditsPanel.SetActive(false);
        helpPanel.SetActive(false);
    }

    public void OnPlay() 
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void OnHelp()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
    }
    
    public void OnCredits()
    {
        creditsPanel.SetActive(!creditsPanel.activeSelf);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
