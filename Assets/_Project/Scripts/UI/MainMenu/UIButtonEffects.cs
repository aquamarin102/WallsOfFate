using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Изображения кнопки")]
    [SerializeField] private Image buttonImage; // Компонент Image
    [SerializeField] private Sprite normalSprite; // Обычное изображение
    [SerializeField] private Sprite hoverSprite; // Изображение при наведении

    [Header("Звуковые эффекты")]
    [SerializeField] private AudioClip hoverSound; // Звук наведения
    [SerializeField] private AudioClip clickSound; // Звук нажатия

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSprite != null)
            buttonImage.sprite = hoverSprite; // Меняем фон при наведении

        if (hoverSound != null)
            AudioManager.Instance.PlayUI(hoverSound); // Воспроизводим звук наведения через AudioManager
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalSprite != null)
            buttonImage.sprite = normalSprite; // Возвращаем обычное изображение
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            AudioManager.Instance.PlayUI(clickSound); // Воспроизводим звук нажатия через AudioManager
    }
}
