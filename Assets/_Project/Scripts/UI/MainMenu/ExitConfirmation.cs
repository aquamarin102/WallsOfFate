using UnityEngine;

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

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}
