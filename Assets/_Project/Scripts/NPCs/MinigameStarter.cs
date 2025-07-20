using System.Collections;
using UnityEngine;

public class MinigameStarter : MonoBehaviour
{
    [SerializeField] private GameObject _miniGame;
    [SerializeField] private GameObject _castle;
    [SerializeField] private GameObject _uiPannel;
    [SerializeField] private CameraSwitch _switch;
    [SerializeField] private GameProcess _miniGameProcessor;
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _trainingPrefab;
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private GameObject _startNewDayButt;

    private GameObject _winPanel;
    private GameObject _losePanel;

    private bool _isMiniGameActive = false;
    private bool _isWaitingForMiniGame = false;
    private bool _canStartMiniGame = true;

    private void Start()
    {
        _switch = GetComponentInChildren<CameraSwitch>();
        _miniGameProcessor.OnEndGame += EndMiniGame;

        InitializePanels();
    }

    private void InitializePanels()
    {
        if (_menu != null)
        {
            _winPanel = _menu.transform.Find("WinPanel")?.gameObject;
            _losePanel = _menu.transform.Find("LosePanel")?.gameObject;

            if (_winPanel == null || _losePanel == null)
            {
                Debug.LogWarning("WinPanel or LosePanel not found in _menu object.");
            }
            else
            {
                _winPanel.SetActive(false);
                _losePanel.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("_menu object is not assigned.");
        }
    }

    private void Update()
    {
        if (!_canStartMiniGame || _isMiniGameActive || _isWaitingForMiniGame)
            return;

        if (_bossPrefab == DialogueManager.GetInstance().PowerCheckPrefab) _startNewDayButt.SetActive(false);

        DialogueManager dialogeManager = DialogueManager.GetInstance();
        bool powerCheckStart = ((Ink.Runtime.BoolValue)dialogeManager.GetVariablesState("PowerCheckStart")).value;

        if (!dialogeManager.DialogueIsPlaying && powerCheckStart)
        {
            StartMiniGame();
            dialogeManager.SetVariableState("PowerCheckStart", false);
        }
    }

    private void StartMiniGame()
    {
        _isMiniGameActive = true;
        _canStartMiniGame = false;

        //MineSpawner mineSpawner = FindObjectOfType<MineSpawner>();
        //if (mineSpawner != null)
        //{
        //    mineSpawner.ClearAllMineObjects();
        //}

        AudioManager.GetInstance().StartMiniGameMusic();

        // ������������ � ����� ���������� ����-���� ��� "������"
        _miniGame.SetActive(false);
        _miniGame.SetActive(true);

        _switch.SwitchCamera();
        if (_uiPannel.activeSelf) _uiPannel.SetActive(false);
        _castle.SetActive(false);
    }

    public void EndMiniGame(string winnerName, string loserName)
    {
        StartCoroutine(EndMiniGameCoroutine(winnerName, loserName));
    }

    private IEnumerator EndMiniGameCoroutine(string winnerName, string loserName)
    {// ��������� ������� � ����������� �� ����������
        _isMiniGameActive = false;
        _miniGame.SetActive(false);
        AudioManager.GetInstance().StopMiniGameMusic();
        // ���������� ���������
        GameObject resultPanel = winnerName == "Player" ? _winPanel : _losePanel;
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            resultPanel.SetActive(false);
        }
        if(winnerName != "Player" && DialogueManager.GetInstance().PowerCheckPrefab == _bossPrefab)
        {
            GameResources.GameResources.ChangePeopleSatisfaction(-1000); // ��������� PeopleSatisfaction
        }
        else if(winnerName == "Player" && DialogueManager.GetInstance().PowerCheckPrefab == _bossPrefab)
        {
            GameResources.GameResources.ChangePeopleSatisfaction(1000); // ��������� PeopleSatisfaction
        }
        if (winnerName == "Player" && DialogueManager.GetInstance().PowerCheckPrefab != _trainingPrefab)
        {
            GameResources.GameResources.ChangeCastleStrength(10); // ����������� CastleStrength
        }
        else if(winnerName != "Player" && DialogueManager.GetInstance().PowerCheckPrefab != _trainingPrefab)
        {
            GameResources.GameResources.ChangePeopleSatisfaction(-1); // ��������� PeopleSatisfaction
        }

        

        // ���������� �������� ������
        _switch.SwitchCamera();
        if (!_uiPannel.activeSelf) _uiPannel.SetActive(true);
        _castle.SetActive(true);

        // �������� ����� ������������ ���������� �������
        yield return new WaitForSeconds(1f);
        _canStartMiniGame = true;
    }

    private void OnDestroy()
    {
        if (_miniGameProcessor != null)
        {
            _miniGameProcessor.OnEndGame -= EndMiniGame;
        }
    }
}