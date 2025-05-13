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

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float _typingSpeed = 0.04f;

    [Header("Load Global JSON")]
    [SerializeField] private TextAsset _loadGlobalJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private GameObject _dialogueBackground;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _displayNameText;
    [SerializeField] private Animator _portraitAnimator;

    public event System.Action DialogueFinished;

    private Animator _layoutAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] _choices;

    private TextMeshProUGUI[] _choicesText;

    private Story _currentStory;
    public bool DialogueIsPlaying { get; private set; }

    private bool _canContinueToNextLine = false;
    private bool _isSelectingChoice = false;

    private Coroutine _displayLineCoroutine;

    private static DialogueManager _instance;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private DialogueVariables _dialogueVariables;

    public GameObject PowerCheckPrefab { get; set; }
    private int? _currentQuestId = null;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        _instance = this;

        _dialogueVariables = new DialogueVariables(_loadGlobalJSON);
    }

    public static DialogueManager GetInstance()
    {
        return _instance;
    }

    public static bool HasInstance => _instance != null;

    private void Start()
    {
        DialogueIsPlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueBackground.SetActive(false);
        _layoutAnimator = _dialoguePanel.GetComponent<Animator>();

        _choicesText = new TextMeshProUGUI[_choices.Length];
        int index = 0;
        foreach (GameObject choice in _choices)
        {
            _choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!DialogueIsPlaying)
        {
            return;
        }

        // Обработка подтверждения выбора
        if (_isSelectingChoice && InputManager.GetInstance().GetInteractPressed())
        {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject != null)
            {
                for (int i = 0; i < _choices.Length; i++)
                {
                    if (_choices[i] == selectedObject)
                    {
                        MakeChoice(i);
                        _isSelectingChoice = false;
                        return;
                    }
                }
            }
        }

        // Обработка продолжения диалога
        if (_canContinueToNextLine
            && _currentStory.currentChoices.Count == 0
            && InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }
    public static string TrimAfterLastSlash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        int lastSlashIndex = input.LastIndexOf('/');

        return lastSlashIndex >= 0
            ? input.Substring(0, lastSlashIndex)
            : input;
    }

    public void EnterDialogueMode(string dialogueFileName, int? questId = null)
    {
        _currentQuestId = questId;
        if (DialogueIsPlaying)
        {
            // Защита от двойного запуска
            StopAllCoroutines();
            StartCoroutine(ExitDialogueMode());   // мягко закрыть и открыть заново
        }
        //dialogueFileName = dialogueFileName.Replace(".json", "");
        // Формируем путь к файлу: Dialogue/имя_файла
        dialogueFileName = TrimAfterLastSlash(dialogueFileName);

#if UNITY_EDITOR
        // Полный путь к папке Resources/Dialogue на диске
        string fullPath = Application.dataPath + $"/Resources/Dialogue/{dialogueFileName}";
        Debug.Log($"Полный путь на диске: {fullPath}");

        // Проверка существования папки
        if (!Directory.Exists(fullPath))
        {
            Debug.LogError($"Папка не существует: {fullPath}");
            return;
        }

        // Проверка наличия файлов .json в папке
        string[] files = Directory.GetFiles(fullPath, "*.json");
        Debug.Log($"Найдено файлов .json: {files.Length}");
        foreach (string file in files)
        {
            Debug.Log($"Файл: {file}");
        }
#endif

        TextAsset[] allDialogue = Resources.LoadAll<TextAsset>($"Dialogue/{dialogueFileName}");
        Debug.Log($"Found {allDialogue.Length} files:");
        foreach (var file in allDialogue)
        {
            Debug.Log(file.name);
        }
        //TextAsset inkJSON = Resources.Load<TextAsset>($"Dialogue/{dialogueFileName}");
        TextAsset inkJSON = allDialogue[allDialogue.Count() - 1];

        //if (inkJSON == null)
        //{
        //    inkJSON = allDialogue[0];
        //    //Debug.LogError($"Dialogue file 'Dialogue/{dialogueFileName}' not found!");
        //    //return;
        //}

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

        // Проверяем, нужно ли завершить квест
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
        if (_currentStory.canContinue)
        {
            if (_displayLineCoroutine != null)
            {
                StopCoroutine(_displayLineCoroutine);
            }
            string nextLine = _currentStory.Continue();

            if (nextLine.Equals("") && !_currentStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            }
            else
            {
                HandleTags(_currentStory.currentTags);
                _displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        _dialogueText.text = line;
        _dialogueText.maxVisibleCharacters = 0;

        HideChoices();

        _canContinueToNextLine = false;
        _isSelectingChoice = false;

        bool _isAddingRichTextTag = false;

        foreach (char letter in line.ToCharArray())
        {
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                _dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            if (letter == '<' || _isAddingRichTextTag)
            {
                _isAddingRichTextTag = true;
                if (letter == '>')
                {
                    _isAddingRichTextTag = false;
                }
            }
            else
            {
                _dialogueText.maxVisibleCharacters++;
                yield return TypingDelay();
            }
        }

        DisplayChoices();

        _canContinueToNextLine = true;
    }

    IEnumerator TypingDelay() 
    { 
        float t = 0; 
        while (t < _typingSpeed) 
        { 
            t += Time.unscaledDeltaTime; yield return null; 
        } 
    }

    private void HideChoices()
    {
        _choices[0].gameObject.transform.parent.gameObject.SetActive(false);
        foreach (GameObject choiceButton in _choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.Log("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    _displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    _portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    _layoutAnimator.Play(tagValue);
                    break;
                default:
                    Debug.Log("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = _currentStory.currentChoices;

        if (currentChoices.Count > _choices.Length)
        {
            Debug.Log("More choices were given then the UI can support. Number of choices given: " + currentChoices.Count);
        }

        if (currentChoices.Count > 0)
        {
            int index = 0;
            _choices[index].gameObject.transform.parent.gameObject.SetActive(true);
            foreach (Choice choice in currentChoices)
            {
                _choices[index].gameObject.SetActive(true);
                _choicesText[index].text = choice.text;
                index++;
            }
            for (int i = index; i < _choices.Length; i++)
            {
                _choices[i].gameObject.SetActive(false);
            }

            _isSelectingChoice = true;
            StartCoroutine(SelectFirstChoice());
        }
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(_choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (_canContinueToNextLine)
        {
            _currentStory.ChooseChoiceIndex(choiceIndex);
            InputManager.GetInstance().RegisterSubmitPressed();
            _isSelectingChoice = false;
            _portraitAnimator.SetTrigger("blink");
            ContinueStory();
        }
    }

    public Ink.Runtime.Object GetVariablesState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        _dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Var was found to be null: " + variableName);
        }
        return variableValue;
    }
    
    public bool SetVariableState(string variableName, object value)
    {
        return _dialogueVariables.SetVariable(variableName, value);
    }
}