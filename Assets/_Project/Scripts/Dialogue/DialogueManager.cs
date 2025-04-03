using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float _typingSpeed = 0.04f;

    [Header("Load Global JSON")]
    [SerializeField] private TextAsset _loadGlobalJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _displayNameText;
    [SerializeField] private Animator _portraitAnimator;

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

    private void Start()
    {
        DialogueIsPlaying = false;
        _dialoguePanel.SetActive(false);

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

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        _currentStory = new Story(inkJSON.text);
          

        DialogueIsPlaying = true;
        _dialoguePanel.SetActive(true);

        _dialogueVariables.StartListening(_currentStory);

        _displayNameText.text = "?";
        _portraitAnimator.Play("default");
        _layoutAnimator.Play("right");

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        _dialogueVariables.StopListening(_currentStory);

        DialogueIsPlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueText.text = "";
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
                yield return new WaitForSeconds(_typingSpeed);
            }
        }

        DisplayChoices();

        _canContinueToNextLine = true;
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
}