using UnityEngine;
using System;
using System.Collections.Generic;

public class CompositeTrigger : MonoBehaviour, ICheckableTrigger
{
    [SerializeField] private List<ActionsAreCommitted> _requiredTriggers = new List<ActionsAreCommitted>();
    [SerializeField] private bool _once = false;
    [SerializeField] private string _searchTag; 

    public event Action OnActivated;
    public bool IsDone { get; private set; }

    private void Awake()
    {
        if (_requiredTriggers.Count == 0 && !string.IsNullOrEmpty(_searchTag))
        {
            FindTriggersByTag(_searchTag);
        }

        if (_requiredTriggers.Count == 0)
        {
            Debug.LogWarning($"No triggers assigned to CompositeTrigger on {gameObject.name}", this);
        }
    }

    // Метод для поиска триггеров по тегу
    public void FindTriggersByTag(string tag)
    {
        if (!TagExists(tag))
        {
            Debug.LogWarning($"Tag '{tag}' does not exist!", this);
            return;
        }

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        _requiredTriggers.Clear();

        foreach (GameObject obj in taggedObjects)
        {
            // Пропускаем себя
            if (obj == gameObject) continue;

            var trigger = obj.GetComponent<ActionsAreCommitted>();
            if (trigger != null)
            {
                _requiredTriggers.Add(trigger);
            }
        }

        Debug.Log($"Found {_requiredTriggers.Count} triggers with tag '{tag}'");
    }

    // Проверка существования тега
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

    public void Trrigered()
    {
         //FindTriggersByTag(_searchTag);
        if (IsDone && _once) return;

        bool allDone = true;
        foreach (var trigger in _requiredTriggers)
        {
            ICheckableTrigger triggerIntr = trigger.gameObject.GetComponent<ICheckableTrigger>();
            if (triggerIntr == null)
            {
                Debug.LogError("Null trigger in CompositeTrigger list!", this);
                continue;
            }

            if (!triggerIntr.IsDone)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            IsDone = true;
            OnActivated?.Invoke();
        }
    }

    // Для ручного добавления триггеров
    public void AddTrigger(ActionsAreCommitted trigger)
    {
        if (trigger != null && !_requiredTriggers.Contains(trigger))
        {
            _requiredTriggers.Add(trigger);
        }
    }

    // Контекстное меню для редактора
    [ContextMenu("Find Triggers By Tag")]
    private void FindTriggersByTagEditor()
    {
        if (!string.IsNullOrEmpty(_searchTag))
        {
            FindTriggersByTag(_searchTag);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}