using UnityEngine;

public class ChangeHierarchyOrder : MonoBehaviour
{
    [Header("Parent and Children")]
    [SerializeField] private Transform parentPanel; // Родительский объект
    [SerializeField] private Transform firstChild; // Первый дочерний элемент
    [SerializeField] private Transform secondChild; // Второй дочерний элемент

    // Основной метод для управления порядком
    public void SetOrder(bool bringFirstToFront)
    {
        if (bringFirstToFront)
        {
            BringToFront(firstChild);
            SendToBack(secondChild);
        }
        else
        {
            BringToFront(secondChild);
            SendToBack(firstChild);
        }
    }

    private void BringToFront(Transform child)
    {
        if (child == null || parentPanel == null)
        {
            Debug.LogWarning("Child or parent is not assigned!");
            return;
        }
        child.SetAsLastSibling();
    }

    private void SendToBack(Transform child)
    {
        if (child == null || parentPanel == null)
        {
            Debug.LogWarning("Child or parent is not assigned!");
            return;
        }
        child.SetAsFirstSibling();
    }

    public void BringFirstToFront() => SetOrder(true);
    public void BringSecondToFront() => SetOrder(false);
}