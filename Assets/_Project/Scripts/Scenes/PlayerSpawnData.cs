using UnityEngine;

public static class PlayerSpawnData
{
    public static Vector3 SpawnPosition { get; set; } = Vector3.zero; // По умолчанию спавн в (0, 0, 0)
    public static Quaternion SpawnRotation { get; set; } = Quaternion.identity; // По умолчанию без поворота
}