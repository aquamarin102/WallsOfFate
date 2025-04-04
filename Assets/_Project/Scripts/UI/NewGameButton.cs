using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private GameObject newGamePanel;

    private SaveLoadManager _saveLoadManager;
    private void Awake()
    {
        _saveLoadManager = GetComponent<SaveLoadManager>();
    }
    void Start()
    {
        newGamePanel.SetActive(false);
    }
    void Update()
    {
        if (newGamePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideNewGamePanel();
        }
    }

    public void StartGameButton()
    {
        if (_saveLoadManager != null && _saveLoadManager.CanLoad())
        {
            ShowNewGamePanel();
        }
        else
        {
            StartGame();
        }
    }
    public void ShowNewGamePanel()
    {
        newGamePanel.SetActive(true);

    }
    public void StartGame()
    {
            SceneManager.LoadScene(1);
    }
    public void HideNewGamePanel()
    {
        newGamePanel.SetActive(false);
    }

}
