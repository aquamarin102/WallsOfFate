using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private GameObject newGamePanel;

    public static event System.Action NewGameStarted;

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
        if (_saveLoadManager != null)
            _saveLoadManager.ClearSavs();

        // —бросить все параметры ресурсов на стартовые
        GameResources.GameResources.Gold = 50;
        GameResources.GameResources.Food = 50;
        GameResources.GameResources.PeopleSatisfaction = 6;
        GameResources.GameResources.CastleStrength = 200;

        NewGameStarted?.Invoke();
        LoadingScreenManager.Instance.panelGameOver.SetActive(false);
        LoadingScreenManager.Instance.panelVictory.SetActive(false);
        LoadingScreenManager.Instance.OnConfirmEndOfDay();
    }

    public void BackToMenuGame()
    {
        GameResources.GameResources.Gold = 50;
        GameResources.GameResources.Food = 50;
        GameResources.GameResources.PeopleSatisfaction = 6;
        GameResources.GameResources.CastleStrength = 200;

        LoadingScreenManager.Instance.panelGameOver.SetActive(false);
        LoadingScreenManager.Instance.panelVictory.SetActive(false);
        LoadingScreenManager.Instance.LoadScene("MainMenu");
    }

    public void HideNewGamePanel()
    {
        newGamePanel.SetActive(false);
    }

}
