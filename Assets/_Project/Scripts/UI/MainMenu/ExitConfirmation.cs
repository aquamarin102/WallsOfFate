using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitConfirmation : MonoBehaviour
{
    [SerializeField] private GameObject exitPanel; 

    void Start()
    {
        exitPanel.SetActive(false); 
    }
    void Update()
    {
        if (exitPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideExitPanel();
        }
    }

    public void ShowExitPanel()
    {
        exitPanel.SetActive(true);
        
    }

    public void HideExitPanel()
    {
        exitPanel.SetActive(false);
    }

    public void QuitToMenuGame()
    {
        LoadingScreenManager.Instance.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
