using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MultiTextChanger : MonoBehaviour
{
    [SerializeField] private TMP_Text _textMeshPro;
    [SerializeField] private List<TriggerableTextData> _triggerTextData = new List<TriggerableTextData>();

    [Header("Tag Search")]
    [SerializeField] private string _searchTag;
    [SerializeField] private bool _autoSearchOnStart = true;

    private void Start()
    {
        if (_autoSearchOnStart && !string.IsNullOrEmpty(_searchTag))
        {
            FindTriggersByTag(_searchTag);
        }
    }

    private void Update()
    {
        foreach (var data in _triggerTextData)
        {
            if (data.actionsAreCommitted != null && data.actionsAreCommitted.IsDone)
            {
                _textMeshPro.text = data.doneText;

            }
        }
    }

    public void AddTrigger(ActionsAreCommitted actions, string doneText)
    {
        _triggerTextData.Add(new TriggerableTextData
        {
            actionsAreCommitted = actions,
            doneText = doneText
        });
    }

    [ContextMenu("Find Triggers By Tag")]
    public void FindTriggersByTag(string tag = null)
    {
        if (string.IsNullOrEmpty(tag))
        {
            if (string.IsNullOrEmpty(_searchTag))
            {
                Debug.LogWarning("No tag specified for search", this);
                return;
            }
            tag = _searchTag;
        }

        if (!TagExists(tag))
        {
            Debug.LogWarning($"Tag '{tag}' does not exist", this);
            return;
        }

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in taggedObjects)
        {
            var actions = obj.GetComponent<ActionsAreCommitted>();
            if (actions != null)
            {
                // Проверяем, чтобы не добавлять дубликаты
                if (!_triggerTextData.Exists(x => x.actionsAreCommitted == actions))
                {
                    AddTrigger(actions, ""); // Пустой текст, можно установить позже
                }
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private bool TagExists(string tagName)
    {
        try
        {
            GameObject.FindWithTag(tagName);
            return true;
        }
        catch (UnityException)
        {
            return false;
        }
    }
}
