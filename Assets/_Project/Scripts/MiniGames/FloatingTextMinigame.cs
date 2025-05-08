using UnityEngine;
using TMPro;

public class FloatingTextMinigame : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 1f;
    [SerializeField] private float lifetime = 1f;

    private TMP_Text tmp;
    private float startTime;
    private Camera mainCam;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        startTime = Time.time;
        mainCam = Camera.main;          // кешируем основную камеру
    }

    private void LateUpdate()
    {
        // Check for null references and destroy the GameObject if any are found
        if (tmp == null || mainCam == null)
        {
            Destroy(gameObject);
            return;
        }

        // 1) Поднимаем
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // 2) Затухаем
        float t = (Time.time - startTime) / lifetime;
        Color c = tmp.color;
        tmp.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t));

        // 3) Billboard — поворачиваем к камере
        Vector3 dir = transform.position - mainCam.transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

        // 4) Удаляем по окончанию жизни
        if (t >= 1f)
            Destroy(gameObject);
    }

    /// <summary>
    /// Устанавливает текст и цвет.
    /// </summary>
    public void Setup(string message, Color color)
    {
        tmp.text = message;
        tmp.color = color;
    }
}