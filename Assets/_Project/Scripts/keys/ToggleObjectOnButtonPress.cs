using Assets._Project.Scripts.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ToggleObjectOnButtonPress : MonoBehaviour
{
    // Сериализуемое поле для кнопок и их состояний
    [System.Serializable]
    public class ToggleButton
    {
        public KeyCode keyCode; // Клавиша
        public bool setActive;  // Состояние, в которое нужно перевести объект
    }

    // Список кнопок и их состояний
    [SerializeField] private List<ToggleButton> toggleButtons = new List<ToggleButton>();

    // Сериализуемое поле для объекта, который будем включать/выключать
    [field: SerializeField] public GameObject TargetObject { get; private set; }
    [SerializeField] private GameObject _objectToDisable;

    private bool _isPaused = false;

    [Inject]
    private void Construct(PlayerMoveController playerMoveController)
    {
        _objectToDisable = playerMoveController.gameObject;
    }

    void Update()
    {
        foreach (var button in toggleButtons)
        {
            if (Input.GetKeyDown(button.keyCode))
            {
                Activate(button.setActive);
            }
        }
    }
    public void Activate(bool station)
    {
        if (station)
        {
            TargetObject.SetActive(!TargetObject.activeSelf);
        }
        else
        {
            TargetObject.SetActive(false);
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused; // Переключаем состояние паузы

        if (_isPaused)
        {
            Time.timeScale = 0f; // Останавливаем время
            Debug.Log("Игра на паузе");
        }
        else
        {
            Time.timeScale = 1f; // Восстанавливаем время
            Debug.Log("Игра продолжается");
        }
    }
}
