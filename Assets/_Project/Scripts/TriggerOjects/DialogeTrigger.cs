    using Assets._Project.Scripts.TriggerOjects;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class DialogeTrigger : MonoBehaviour, ICheckableTrigger
    {

        [Header("Ink JSON Dialogues")]
        [SerializeField] private List<TextAsset> _inkJSONDialogues = new List<TextAsset>(); // Список диалогов

        [Header("Dependent Triggers")]
        [SerializeField] private List<ITriggerable> _requiredTriggers = new List<ITriggerable>();

        [SerializeField] private int _indexForCheck = 1;
        [SerializeField] private string _requeeredDataName = "";

        private int dialogIndex = 0;

        public bool IsDone { get; private set; }

        private void Start()
        {
            FindAndAddRequiredTriggersByTag();
        }

        public void Trrigered()
        {
            if (CanTrigger(_indexForCheck) && _inkJSONDialogues.Count > 0)
            {
                if (dialogIndex > _inkJSONDialogues.Count - 1) dialogIndex = _inkJSONDialogues.Count - 1;
            
                DialogueManager dialogueManager = DialogueManager.GetInstance();

                TextAsset selectedDialogue = _inkJSONDialogues[dialogIndex];
                dialogIndex++;

                dialogueManager.EnterDialogueMode(selectedDialogue);
                if(dialogIndex == _inkJSONDialogues.Count) IsDone = true;
            }
        }

        private bool CanTrigger(int numberOfDialog)
        {
            foreach (var trigger in _requiredTriggers)
            {
                if (trigger == null) continue;

                if (trigger is ICheckableTrigger checkable && !checkable.IsDone && numberOfDialog == dialogIndex)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(_requeeredDataName))
            {
                // Получаем тип DataBetweenLocations
                Type dataType = typeof(DataBetweenLocations);

                // Ищем поле с указанным именем
                var field = dataType.GetField(_requeeredDataName);

                if (field != null && field.FieldType == typeof(bool) /*&& numberOfDialog == dialogIndex*/)
                {
                    // Получаем значение поля
                    bool value = (bool)field.GetValue(null);

                    // УДАЛИТЬ!!!!
                    if (value)
                    {
                        dialogIndex = 1;
                        return true; // Поле найдено, но равно false
                    }
                }
            }

            return true;
        }

        // Добавление нового диалога в список
        public void AddDialogue(TextAsset newDialogue)
        {
            if (newDialogue != null && !_inkJSONDialogues.Contains(newDialogue))
            {
                _inkJSONDialogues.Add(newDialogue);
            }
        }

        // Очистка списка диалогов
        public void ClearDialogues()
        {
            _inkJSONDialogues.Clear();
        }

        // Для отображения в инспекторе
        private void OnValidate()
        {
            // Конвертируем GameObject'ы в компоненты ITriggerable
            for (int i = 0; i < _requiredTriggers.Count; i++)
            {
                if (_requiredTriggers[i] is DialogeTrigger gameObj)
                {
                    _requiredTriggers[i] = gameObj.GetComponent<DialogeTrigger>();
                }
            }
        }

        // Новый метод для поиска триггеров по тегу
        [ContextMenu("Find Required Triggers By Tag")]
        public void FindAndAddRequiredTriggersByTag()
        {
            if (transform.parent == null) return;

            // Получаем имя родителя, убираем "(clone)" если есть
            string parentName = transform.parent.name;
            parentName = parentName.Replace("(Clone)", "").Trim();

            // Формируем тег
            string searchTag = parentName + "RequiredTrigger";

            if (!TagExists(searchTag))
            {
                Debug.LogWarning($"Тег '{searchTag}' не существует. Проверьте настройки тегов в Project Settings.", this);
                return;
            }

            // Ищем все объекты с этим тегом
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(searchTag);

            if(taggedObjects != null)
            {
                //_requiredTriggers.Clear();

                foreach (GameObject obj in taggedObjects)
                {
                    // Проверяем, чтобы не добавить себя самого
                    if (obj == this.gameObject) continue;

                    ITriggerable trigger = obj.GetComponent<ITriggerable>();
                    if (trigger != null)
                    {
                        _requiredTriggers.Add(trigger);
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
                // Этот способ работает в Runtime и Editor
                GameObject.FindWithTag(tagName); // Если тег не существует, Unity выкинет ошибку
                return true;
            }
            catch (UnityException)
            {
                return false;
            }
        }
    }