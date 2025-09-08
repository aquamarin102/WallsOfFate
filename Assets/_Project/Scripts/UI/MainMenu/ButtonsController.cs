using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour
{
    public static ButtonsController Instance { get; private set; }

    [SerializeField] private GameObject firstButton;

    // »нтервал, в течение которого повторные нажати€ Enter/Space/E игнорируютс€
    [SerializeField] private float submitCooldown = 1f;
    private float lastSubmitTime = -Mathf.Infinity;

    private bool canAcceptInput = true;

    private float blockInputUntil = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.LoadingStarted += OnLoadingStarted;
            LoadingScreenManager.Instance.LoadingFinished += OnLoadingFinished;
        }
    }

    private void OnDisable()
    {
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.LoadingStarted -= OnLoadingStarted;
            LoadingScreenManager.Instance.LoadingFinished -= OnLoadingFinished;
        }
    }

    private void OnLoadingStarted()
    {
        canAcceptInput = false;
    }

    private void OnLoadingFinished()
    {
        StartCoroutine(EnableInputWithDelay(1f));
    }

    private IEnumerator EnableInputWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canAcceptInput = true;
    }

    public void BlockInputFor(float seconds)
    {
        blockInputUntil = Time.unscaledTime + seconds;
    }

    private void Update()
    {
        // Ѕлокируем ввод, пока идЄт загрузка или не истЄк начальный delay
        if (!canAcceptInput ||
            (LoadingScreenManager.Instance != null && LoadingScreenManager.Instance.IsLoading))
            return;

        if (Time.unscaledTime < blockInputUntil)
            return;

        // ќбнаружение мыши
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (InputModeTracker.UsingKeyboard)
            {
                InputModeTracker.NotifyMouseInput();
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        // ќбнаружение клавиатуры (стрелки или W/S)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            if (!InputModeTracker.UsingKeyboard)
                InputModeTracker.NotifyKeyboardInput();

            ClearMouseHoverEffect();

            if (EventSystem.current.currentSelectedGameObject == null)
                SetSelected(firstButton);
        }

        // Enter, Space или E Ч с задержкой 1 секунда между нажати€ми
        if (Input.GetKeyUp(KeyCode.Return)
            || Input.GetKeyUp(KeyCode.E))
        {
            if (Time.unscaledTime - lastSubmitTime < submitCooldown)
                return;

            lastSubmitTime = Time.unscaledTime;

            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                var btn = selected.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.Invoke();
            }
        }
    }

    private void ClearMouseHoverEffect()
    {
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var res in results)
        {
            var effect = res.gameObject.GetComponent<UIButtonEffects>();
            if (effect != null)
                effect.ForceExit();
        }
    }

    private void SetSelected(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
}
