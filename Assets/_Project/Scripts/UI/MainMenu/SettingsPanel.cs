using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel; 

    void Start()
    {
        settingsPanel.SetActive(false); 
    }
    void Update()
    {
        if (settingsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideSettingsPanel();
        }
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
        
    }

    public void HideSettingsPanel()
    {
        settingsPanel.SetActive(false); 
    }

}
