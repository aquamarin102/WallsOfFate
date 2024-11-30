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

    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialoguePanel;

    [SerializeField] private GameObject _continueIcon;

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

    private Coroutine _displayLineCoroutine;

    private static DialogueManager _instance;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("Found more than one Dialogue Manager in the scene");
        }
        _instance = this;
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

        //reset portrait, layout and speaker
        _displayNameText.text = "?";
        _portraitAnimator.Play("default");
        _layoutAnimator.Play("right");

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

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

            _displayLineCoroutine = StartCoroutine(DisplayLine(_currentStory.Continue()));
            HandleTags(_currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        _dialogueText.text = "";

        _continueIcon.SetActive(false);
        HideChoices();

        _canContinueToNextLine = false;

        foreach (char letter in line.ToCharArray())
        {
            if(InputManager.GetInstance().GetSubmitPressed())
            {
                _dialogueText.text = line;
                break;
            }

            _dialogueText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }

        _continueIcon.SetActive(true);
        DisplayChoices();

        _canContinueToNextLine = true;
    }

    private void HideChoices()
    {
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

        int index = 0;
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
        StartCoroutine(SelectFirstChoice());
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
            //InputManager.GetInstance().RegisterSubmitPressed();
            //ContinueStory();
        }
    }
}
