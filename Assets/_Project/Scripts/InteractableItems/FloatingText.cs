using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private SpriteRenderer backgroundSR;

    [Header("Настройки")]
    [Tooltip("Во сколько раз ширина фона больше самой ширины текста")]
    [SerializeField] private float scaleFactorX = 1.2f;
    [Tooltip("Во сколько раз высота фона больше самой высоты текста")]
    [SerializeField] private float scaleFactorY = 1.2f;
    [Tooltip("Время жизни текста в секундах")]
    [SerializeField] private float lifeTime = 1f;

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshPro>();
        if (backgroundSR == null)
            backgroundSR = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Устанавливает текст и сразу же подгоняет фон по отдельным факторам X и Y
    /// </summary>
    public void SetText(string msg)
    {
        // обновляем текст
        textMesh.text = msg;
        textMesh.ForceMeshUpdate();

        // получаем габариты текста
        Vector2 textSize = textMesh.GetRenderedValues(false);

        // подгоняем фон по X и Y
        backgroundSR.size = new Vector2(
            textSize.x * scaleFactorX,
            textSize.y * scaleFactorY
        );
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
            Destroy(gameObject);
    }
}
