using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using Quest;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float _typingSpeed = 0.04f;
    [SerializeField] private float _inputBufferTime = 0.2f; // debounce interval

    [Header("Load Global JSON")]
    [SerializeField] private TextAsset _loadGlobalJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private GameObject _dialogueBackground;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _displayNameText;
    [SerializeField] private Animator _portraitAnimator;

    public event Action DialogueFinished;

    private Animator _layoutAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] _choices;

    private TextMeshProUGUI[] _choicesText;
    private Button[] _choicesButton;

    private Story _currentStory;
    public bool DialogueIsPlaying { get; private set; }

    private bool _canContinueToNextLine = false;
    private bool _isSelectingChoice = false;
    private Coroutine _displayLineCoroutine;

    // Input debounce and selection
    private float _lastInputTime = 0f;
    private int _selectedChoiceIndex = 0;

    private static DialogueManager _instance;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private DialogueVariables _dialogueVariables;
    public GameObject PowerCheckPrefab { get; set; }
    private int? _currentQuestId = null;

    private bool _skipNextLineIfChoice;
    private string _lastChosenText;

    private void Awake()
    {
        if (_instance != null)
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        _instance = this;

        _dialogueVariables = new DialogueVariables(_loadGlobalJSON);
    }

    public static DialogueManager GetInstance() => _instance;
    public static bool HasInstance => _instance != null;

    private void Start()
    {
        DialogueIsPlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueBackground.SetActive(false);
        _layoutAnimator = _dialoguePanel.GetComponent<Animator>();

        int choiceCount = _choices.Length;
        _choicesText = new TextMeshProUGUI[choiceCount];
        _choicesButton = new Button[choiceCount];

        for (int i = 0; i < choiceCount; i++)
        {
            _choicesText[i] = _choices[i].GetComponentInChildren<TextMeshProUGUI>();
            _choicesButton[i] = _choices[i].GetComponent<Button>();
            int idx = i;
            // click
            _choicesButton[i].onClick.AddListener(() => OnChoiceClick(idx));
            // hover
            EventTrigger trigger = _choices[i].GetComponent<EventTrigger>() ?? _choices[i].AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry.callback.AddListener((_) => OnChoiceHover(idx));
            trigger.triggers.Add(entry);
        }
    }
    private void Update()
    {
        if (!DialogueIsPlaying) return;

        float now = Time.time;
        // Skip text animation: Space or left click
        if (_displayLineCoroutine != null && !_canContinueToNextLine)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                && now - _lastInputTime > _inputBufferTime)
            {
                _dialogueText.maxVisibleCharacters = _dialogueText.text.Length;
                _lastInputTime = now;
            }
        }

        if (_isSelectingChoice)
        {
            int count = _currentStory.currentChoices.Count;
            // Navigate Up
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                && now - _lastInputTime > _inputBufferTime)
            {
                _selectedChoiceIndex = (_selectedChoiceIndex - 1 + count) % count;
                EventSystem.current.SetSelectedGameObject(_choices[_selectedChoiceIndex]);
                _lastInputTime = now;
            }
            // Navigate Down
            if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                && now - _lastInputTime > _inputBufferTime)
            {
                _selectedChoiceIndex = (_selectedChoiceIndex + 1) % count;
                EventSystem.current.SetSelectedGameObject(_choices[_selectedChoiceIndex]);
                _lastInputTime = now;
            }
            // Select via Enter or E
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.E))
                && now - _lastInputTime > _inputBufferTime)
            {
                MakeChoice(_selectedChoiceIndex);
                _lastInputTime = now;
            }
        }
        else
        {
            // Continue dialogue only on Space or Left Click when full text shown and no choices
            if (_canContinueToNextLine && _currentStory.currentChoices.Count == 0
                && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                && now - _lastInputTime > _inputBufferTime)
            {
                ContinueStory();
                _lastInputTime = now;
            }
        }
    }

    private void OnChoiceHover(int index)
    {
        if (_isSelectingChoice)
        {
            _selectedChoiceIndex = index;
            EventSystem.current.SetSelectedGameObject(_choices[index]);
        }
    }

    private void OnChoiceClick(int index)
    {
        if (_isSelectingChoice && _canContinueToNextLine
            && Time.time - _lastInputTime > _inputBufferTime)
        {
            _selectedChoiceIndex = index;
            MakeChoice(index);
            _lastInputTime = Time.time;
        }
    }

    public static string TrimAfterLastSlash(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        int lastSlashIndex = input.LastIndexOf('/');
        return lastSlashIndex >= 0 ? input.Substring(0, lastSlashIndex) : input;
    }

    public void EnterDialogueMode(string dialogueFileName, int? questId = null)
    {
        _currentQuestId = questId;
        if (DialogueIsPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(ExitDialogueMode());
        }
        dialogueFileName = TrimAfterLastSlash(dialogueFileName);

#if UNITY_EDITOR
        string fullPath = Application.dataPath + $"/Resources/Dialogue/{dialogueFileName}";
        if (!Directory.Exists(fullPath)) { Debug.LogError($"Папка не существует: {fullPath}"); return; }
        string[] files = Directory.GetFiles(fullPath, "*.json");
#endif
        TextAsset[] allDialogue = Resources.LoadAll<TextAsset>($"Dialogue/{dialogueFileName}");
        TextAsset inkJSON = allDialogue[allDialogue.Length - 1];

        _currentStory = new Story(inkJSON.text);
        DialogueIsPlaying = true;
        _dialoguePanel.SetActive(true);
        _dialogueBackground.SetActive(true);
        _dialogueVariables.StartListening(_currentStory);
        _displayNameText.text = "?";
        _portraitAnimator.Play("default");
        _layoutAnimator.Play("right");
        ContinueStory();
    }


    public IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        if (_currentQuestId.HasValue)
        {
            bool shouldComplete = GetVariablesState("QuestComplete")?.ToString().ToLower() == "true";
            QuestCollection.ForceCompleteQuest(_currentQuestId.Value, shouldComplete);
            SetVariableState("QuestComplete", false);
        }
        _dialogueVariables.StopListening(_currentStory);
        DialogueIsPlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueBackground.SetActive(false);
        _dialogueText.text = "";
        DialogueFinished?.Invoke();
    }

    private void ContinueStory()
    {
        while (_currentStory.canContinue)
        {
            string nextLine = _currentStory.Continue();
            HandleTags(_currentStory.currentTags);

            if (string.IsNullOrWhiteSpace(nextLine))
                continue;
            // ― отбрасываем первую строку после клика,
            //   если она совпадает с текстом выбранного варианта
            if (_skipNextLineIfChoice &&
                !string.IsNullOrWhiteSpace(nextLine) &&
                nextLine.Trim().Equals(_lastChosenText, StringComparison.Ordinal))
            {
                _skipNextLineIfChoice = false;   // cброс флажка
                continue;                        // сразу читаем следующую
            }

            _skipNextLineIfChoice = false;       // на всякий случай

            // обычная печать
            if (_displayLineCoroutine != null)
                StopCoroutine(_displayLineCoroutine);

            _displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            return;
        }

        if (_currentStory.currentChoices.Count > 0)
            DisplayChoices();
        else
            StartCoroutine(ExitDialogueMode());
    }



    private IEnumerator DisplayLine(string line)
    {
        _dialogueText.text = line;
        _dialogueText.maxVisibleCharacters = 0;
        HideChoices();
        _canContinueToNextLine = false;
        _isSelectingChoice = false;

        // 1) Печать текста
        bool inTag = false;
        foreach (char c in line)
        {
            if (c == '<' || inTag)
            {
                inTag = inTag && c != '>';
                _dialogueText.maxVisibleCharacters++;
            }
            else
            {
                _dialogueText.maxVisibleCharacters++;
                yield return TypingDelay();
            }
            // Если удержали Space/клик, то сразу показываем всё
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                _dialogueText.maxVisibleCharacters = line.Length;
                break;
            }
        }

        // 2) Ждём один кадр, чтобы «съесть» прежний Input
        yield return null;

        // 3) Ждём реального нажатия Space или ЛКМ
        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButtonDown(0))
            yield return null;

        // 4) Показываем варианты (или разрешаем обновлению вызвать ContinueStory)
        if (_currentStory.currentChoices.Count > 0)
        {
            DisplayChoices();
            _canContinueToNextLine = true;
        }
        else
        {
            // Если вариантов нет, сразу продолжаем историю
            ContinueStory();
        }
    }


    IEnumerator TypingDelay()
    {
        float t = 0f;
        while (t < _typingSpeed)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void HideChoices()
    {
        if (_choices.Length > 0)
            _choices[0].transform.parent.gameObject.SetActive(false);
        foreach (var c in _choices) c.SetActive(false);
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            var parts = tag.Split(':');
            if (parts.Length != 2)
            {
                Debug.Log("Tag could not be appropriately parsed: " + tag);
                continue;
            }
            string key = parts[0].Trim();
            string val = parts[1].Trim();
            switch (key)
            {
                case SPEAKER_TAG:
                    _displayNameText.text = val;
                    break;
                case PORTRAIT_TAG:
                    _portraitAnimator.Play(val);
                    break;
                case LAYOUT_TAG:
                    _layoutAnimator.Play(val);
                    break;
                default:
                    Debug.Log("Unhandled tag: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        var currentChoices = _currentStory.currentChoices;
        if (currentChoices.Count == 0) return;
        int index = 0;
        _choices[0].transform.parent.gameObject.SetActive(true);
        foreach (var choice in currentChoices)
        {
            _choices[index].SetActive(true);
            _choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < _choices.Length; i++)
            _choices[i].SetActive(false);

        _isSelectingChoice = true;
        _canContinueToNextLine = true;   // ← добавьте эту строку
        _selectedChoiceIndex = 0;
        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(_choices[_selectedChoiceIndex]);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (_canContinueToNextLine)
        {
            // запоминаем текст, который был виден в кнопке
            _lastChosenText = _currentStory.currentChoices[choiceIndex].text.Trim();
            _skipNextLineIfChoice = true;

            _currentStory.ChooseChoiceIndex(choiceIndex);
            _portraitAnimator.SetTrigger("blink");
            ContinueStory();
        }
    }
    public Ink.Runtime.Object GetVariablesState(string variableName)
    {
        _dialogueVariables.variables.TryGetValue(variableName, out var variableValue);
        if (variableValue == null)
            Debug.LogWarning("Ink Var was found to be null: " + variableName);
        return variableValue;
    }

    public bool SetVariableState(string variableName, object value)
    {
        return _dialogueVariables.SetVariable(variableName, value);
    }
}