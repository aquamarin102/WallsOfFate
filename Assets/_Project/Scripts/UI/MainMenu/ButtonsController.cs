using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour
{
    [SerializeField] private GameObject firstButton;

    private bool usingKeyboard = true;

    void OnEnable()
    {
        SetSelected(firstButton);
    }

    void Update()
    {
        // Обнаружение мыши
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (InputModeTracker.UsingKeyboard)
            {
                InputModeTracker.NotifyMouseInput();
                EventSystem.current.SetSelectedGameObject(null); // сброс
            }
        }

        // Обнаружение клавиатуры
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            if (!InputModeTracker.UsingKeyboard)
                InputModeTracker.NotifyKeyboardInput();

            ClearMouseHoverEffect();

            if (EventSystem.current.currentSelectedGameObject == null)
                SetSelected(firstButton);
        }

        // Enter или Space
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                Button btn = selected.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.Invoke();
            }
        }
    }

  

    void ClearMouseHoverEffect()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            var effect = result.gameObject.GetComponent<UIButtonEffects>();
            if (effect != null)
            {
                effect.ForceExit(); // Принудительно сбрасываем эффект наведения
            }
        }
    }

    void SetSelected(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null); // сброс перед установкой
        EventSystem.current.SetSelectedGameObject(go);
    }
}
