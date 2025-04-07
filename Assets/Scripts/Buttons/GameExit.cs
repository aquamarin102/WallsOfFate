using UnityEngine;

public class GameExit : MonoBehaviour
{
    public void ExitGame()
    {
        // Если в редакторе, вывести сообщение
    #if UNITY_EDITOR
            Debug.Log("Игра завершена (в редакторе).");
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            // Если в билде, закрыть приложение
            Application.Quit();
    #endif
    }
}
