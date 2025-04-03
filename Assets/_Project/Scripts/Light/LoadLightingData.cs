using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadLightingData : MonoBehaviour
{
    void Start()
    {
        LoadBakedLightmaps();
    }

    void LoadBakedLightmaps()
    {
#if UNITY_EDITOR
        // Для Unity 2019.3+
        if (Lightmapping.lightingDataAsset != null)
        {
            // Альтернативные методы в зависимости от версии Unity
#if UNITY_2020_1_OR_NEWER
            Lightmapping.Bake();
#else
            Lightmapping.ForceUpdate();
#endif
            Debug.Log("Baked lightmaps loaded!");
        }
        else
        {
            Debug.LogWarning("No baked lightmaps found!");
        }
#else
        Debug.Log("Lightmapping is Editor-only. Use Preloaded Assets in build.");
#endif
    }
}