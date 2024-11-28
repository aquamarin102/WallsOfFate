using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    private Story _currentStory;
    public bool DialogueIsPlaying { get; private set; }

    private static DialogueManager _instance;

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
    }

    private void Update()
    {
        if (!DialogueIsPlaying)
        {
            return;
        }
        if(Input.GetMouseButton(1))
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        _currentStory = new Story(inkJSON.text);
        DialogueIsPlaying = true;
        _dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        DialogueIsPlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (_currentStory.canContinue)
        {
            _dialogueText.text = _currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }
}
