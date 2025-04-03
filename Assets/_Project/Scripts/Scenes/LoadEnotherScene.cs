using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Assets.Scripts.TriggerOjects;

public class LoadEnotherScene : MonoBehaviour
{
    [SerializeField] private List<DoorForEnotherScene> doors;

    private void Start()
    {
        foreach (var door in doors)
        {
            if (door != null)
            {
                door.OnActivated += LoadScene;
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        foreach (var door in doors)
        {
            if (door != null)
            {
                door.OnActivated -= LoadScene;
            }
        }
    }
}
