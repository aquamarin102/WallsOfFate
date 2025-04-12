
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private PlayerAnimator playerAnimator;

    // Сохраняем активный интерактивный объект, с которым находится игрок.
    private GameObject currentInteractable;

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        if (playerAnimator == null)
        {
            Debug.LogError("InteractionController: Не найден компонент PlayerAnimator!");
        }
    }

    // Вызывается, когда другой коллайдер входит в триггер-коллайдер игрока.
    private void OnTriggerEnter(Collider other)
    {
        // Определяем интерактивные объекты по тегу.
        if (other.CompareTag("PickupFloor") ||
            other.CompareTag("PickupBody") ||
            other.CompareTag("Chest"))
        {
            currentInteractable = other.gameObject;
        }
    }

    // Вызывается, когда другой коллайдер выходит из триггер-коллайдера игрока.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentInteractable)
        {
            currentInteractable = null;
        }
    }

    private void Update()
    {
        // Если нажата клавиша E и есть активный интерактивный объект
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            if (currentInteractable.CompareTag("PickupFloor"))
            {
                playerAnimator.PlayPickupFloor();
            }
            else if (currentInteractable.CompareTag("PickupBody"))
            {
                playerAnimator.PlayPickupBody();
            }
            else if (currentInteractable.CompareTag("Chest"))
            {
                playerAnimator.PlayOpenChest();
            }
        }
    }
}
