using Assets._Project.Scripts.TriggerOjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets._Project.Scripts.TriggerOjects;
using System;
using System.Collections;
using UnityEngine;
using Quest;

public class PocemonNPCNew : MonoBehaviour, ITriggerable
{
    // Events and interface implementation
    public event Action OnActivated;
    public bool IsDone { get; private set; }
    [SerializeField] private bool _once = false;

    [Header("Game Objects")]
    [SerializeField] private GameObject _miniGame;
    [SerializeField] private GameObject _castle;
    [SerializeField] private GameObject _uiPannel;
    [SerializeField] private CameraSwitch _switch;
    [SerializeField] private GameProcess _miniGameProcessor;
    [SerializeField] private GameObject _menu;

    private GameObject _winPanel;
    private GameObject _losePanel;
    private DialogueManager _dialogueManager;

    private void Start()
    {
        InitializeComponents();
        _miniGameProcessor.OnEndGame += EndMiniGame;
    }

    private void InitializeComponents()
    {
        _switch = GetComponentInChildren<CameraSwitch>();
        _dialogueManager = DialogueManager.GetInstance();

        if (_menu != null)
        {
            _winPanel = _menu.transform.Find("WinPanel")?.gameObject;
            _losePanel = _menu.transform.Find("LosePanel")?.gameObject;

            if (_winPanel != null && _losePanel != null)
            {
                _winPanel.SetActive(false);
                _losePanel.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Missing win/lose panels in menu object");
            }
        }
    }

    public void Triggered()
    {
        if (IsDone && _once) return;
        if (!CheckDialogueConditions()) return;

        StartMiniGame();
        IsDone = true;
        OnActivated?.Invoke();
    }

    private bool CheckDialogueConditions()
    {
        if (_dialogueManager.DialogueIsPlaying) return false;

        //var powerCheckStart = ((Ink.Runtime.BoolValue)_dialogueManager
        //    .GetVariablesState("PowerCheckStart")).value;

        if (QuestCollection.GetActiveQuestGroups().Count > 0 && QuestCollection.GetActiveQuestGroups()[0].CurrentTaskId == 5) return true;
        else return false;
        //return powerCheckStart;
    }

    private void StartMiniGame()
    {
        _switch.SwitchCamera();
        _uiPannel.SetActive(false);
        _miniGame.SetActive(true);
        _castle.SetActive(false);
    }

    public void EndMiniGame(string winnerName, string loserName)
    {
        StartCoroutine(EndMiniGameCoroutine(winnerName, loserName));
    }

    private IEnumerator EndMiniGameCoroutine(string winnerName, string loserName)
    {
        _miniGame.SetActive(false);
        var currentPanel = winnerName == "Player" ? _winPanel : _losePanel;

        TogglePanel(currentPanel, true);
        yield return new WaitForSeconds(2f);
        TogglePanel(currentPanel, false);

        CompleteMiniGame();
    }

    private void TogglePanel(GameObject panel, bool state)
    {
        if (panel != null) panel.SetActive(state);
    }

    private void CompleteMiniGame()
    {
        _switch.SwitchCamera();
        _uiPannel.SetActive(true);
        _castle.SetActive(true);
        DataBetweenLocations.ForgePerfom = true;
    }
}
