//using UnityEngine;
//using UnityEditor; // Требуется для Lightmapping.Bake()

//public class BakeLightingOnStart : MonoBehaviour
//{
//    void Start()
//    {
//#if UNITY_EDITOR
//        if (!Appdlication.isPlaying)
//            return;
//#endif

//        Debug.Log("Baking lighting...");
//        Lightmapping.Bake(); // Запускает запекание
//    }
//}