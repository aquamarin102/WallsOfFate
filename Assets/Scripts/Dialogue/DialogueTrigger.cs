using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject _visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset _inkJSON;

    private bool _playerInRange;

    private void Awake()
    {
        _playerInRange = false;
        _visualCue.SetActive(false);
    }

    private void Update()
    {
        if (_playerInRange && !DialogueManager.GetInstance().DialogueIsPlaying)
        {
            _visualCue.SetActive(true);

            if (InputManager.GetInstance().GetInteractPressed())
            {
                DialogueManager.GetInstance().EnterDialogueMode(_inkJSON);
            }
        }
        else
        {
            _visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _playerInRange = false;
        }
    }
}
