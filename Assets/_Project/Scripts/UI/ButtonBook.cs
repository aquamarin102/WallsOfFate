using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class ButtonBlinkOnChildDisable : MonoBehaviour
{
    [Header("Корневой объект для наблюдения")]
    [SerializeField] private GameObject watchedRoot;  // за этим объектом берём всех детей

    [Header("UI")]
    [SerializeField] private Button targetButton;
    [SerializeField] private float visibleTime = 3f;
    [SerializeField] private float blinkSpeed = 2f;

    private CanvasGroup canvasGroup;
    private GameObject[] watchedItems;
    private bool[] lastState;
    private Coroutine blinkRoutine;

    private void Awake()
    {
        // 1) Подготовка кнопки
        canvasGroup = targetButton.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = targetButton.gameObject.AddComponent<CanvasGroup>();
        targetButton.gameObject.SetActive(false);

        // 2) Собираем ВСЕ дочерние ГеймОбъекты (включая неактивные)
        watchedItems = watchedRoot
            .GetComponentsInChildren<Transform>(true)    // true = включать неактивные
            .Select(t => t.gameObject)
            .Where(go => go != watchedRoot)               // исключаем сам root
            .ToArray();

        // 3) Запоминаем их стартовые состояния
        lastState = new bool[watchedItems.Length];
        for (int i = 0; i < watchedItems.Length; i++)
            lastState[i] = watchedItems[i].activeInHierarchy;
    }

    private void Update()
    {
        // Для каждого потомка проверяем: был активным → стал неактивным?
        for (int i = 0; i < watchedItems.Length; i++)
        {
            bool now = watchedItems[i].activeInHierarchy;
            if (lastState[i] && !now)
            {
                StartBlinking();
                // если нужно реагировать только на первый оффлайн в кадре, можно break;
            }
            lastState[i] = now;
        }
    }

    private void StartBlinking()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        targetButton.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < visibleTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            yield return null;
        }

        targetButton.gameObject.SetActive(false);
    }
}
