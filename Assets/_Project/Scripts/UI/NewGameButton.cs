using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private GameObject newGamePanel;

    public static event System.Action NewGameStarted;

    private SaveLoadManager _saveLoadManager;
    private LoadingScreenManager _loadingScreenManager;
    private void Awake()
    {
        _saveLoadManager = GetComponent<SaveLoadManager>();
        _loadingScreenManager = GetComponent<LoadingScreenManager>();
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
        if (_saveLoadManager != null)
        {
            // —бросить все сохранЄнные данные, чтобы нова€ игра начиналась с чистого листа
            _saveLoadManager.ClearSavs();
        }
        NewGameStarted?.Invoke();        // оповестили всех подписчиков
        _loadingScreenManager.panelGameOver.SetActive(false);
        _loadingScreenManager.panelVictory.SetActive(false);
        LoadingScreenManager.Instance.OnConfirmEndOfDay();
    }

    public void BackToMenuGame()
    {
        _loadingScreenManager.panelGameOver.SetActive(false);
        _loadingScreenManager.panelVictory.SetActive(false);
        LoadingScreenManager.Instance.LoadScene("MainMenu");
    }

    public void HideNewGamePanel()
    {
        newGamePanel.SetActive(false);
    }

}
