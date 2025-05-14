using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Показывает кнопку на 3 с, когда любой наблюдаемый объект
/// становится неактивным, и плавно мигает альфой.
/// </summary>
public class ButtonBlinkOnItemDisable : MonoBehaviour
{
    [Header("Что отслеживаем")]
    [SerializeField] private GameObject[] watchedItems;   // предметы на сцене

    [Header("UI")]
    [SerializeField] private Button targetButton;         // кнопка, которую показываем
    [SerializeField] private float visibleTime = 3f;      // сколько секунд держать кнопку включённой
    [SerializeField] private float blinkSpeed = 2f;       // скорость мигания

    private CanvasGroup canvasGroup;
    private bool[] lastState;                             // состояние предметов в предыдущем кадре
    private Coroutine blinkRoutine;

    private void Awake()
    {
        // гарантируем наличие CanvasGroup, чтобы управлять альфой
        canvasGroup = targetButton.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = targetButton.gameObject.AddComponent<CanvasGroup>();

        // запоминаем стартовые состояния
        lastState = new bool[watchedItems.Length];
        for (int i = 0; i < watchedItems.Length; i++)
            lastState[i] = watchedItems[i].activeInHierarchy;

        // кнопку скрываем до события
        targetButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        // проверяем, не выключился ли какой-то предмет в этом кадре
        for (int i = 0; i < watchedItems.Length; i++)
        {
            bool current = watchedItems[i].activeInHierarchy;
            if (!current && lastState[i])        // событие «стал неактивным»
            {
                StartBlinking();
            }
            lastState[i] = current;              // обновляем историю
        }
    }

    /// <summary>Запускает мигание заново.</summary>
    private void StartBlinking()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    /// <summary>Показывает кнопку и плавно колеблет альфу visibleTime секунд.</summary>
    private IEnumerator BlinkRoutine()
    {
        targetButton.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < visibleTime)
        {
            timer += Time.deltaTime;

            // PingPong даёт 0→1→0; умножаем Time.time на скорость
            canvasGroup.alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);

            yield return null;
        }

        targetButton.gameObject.SetActive(false);
    }
}
