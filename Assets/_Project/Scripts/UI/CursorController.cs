using UnityEngine;

public class CursorController : MonoBehaviour
{
    // Ссылки на панели интерфейса, для которых должен быть включён курсор
    [SerializeField] private GameObject[] uiPanels;

    private void Start()
    {
        // Изначально курсор выключен: скрыт и заблокирован
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        bool anyPanelActive = false;
        foreach (GameObject panel in uiPanels)
        {
            // Если хотя бы одна панель активна, назначаем флаг
            if (panel.activeSelf)
            {
                anyPanelActive = true;
                break;
            }
        }

        if (anyPanelActive)
        {
            // Если хотя бы одна из указанных панелей активна – делаем курсор видимым и разблокированным
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Если ни одна из панелей не активна – скрываем курсор и блокируем его
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
