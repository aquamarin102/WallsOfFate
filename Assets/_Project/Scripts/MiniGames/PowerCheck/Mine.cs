using UnityEngine;

/// <summary>
/// Lightweight data‑holder for a spawned mine.
/// </summary>
public class Mine
{
    // ─────────────── Public, read‑only data ───────────────
    public uint Number { get; }   // ID in the pool / wave
    public float Cooldown { get; }   // Respawn time
    public GameObject MineGameObject { get; }   // Visual / collider root

    /// <summary>True only for the very first появление мины после создания.</summary>
    public bool IsFirstSpawn { get; set; } = true;

    // ─────────────── Ctor ───────────────
    public Mine(uint number, float cooldown, GameObject mineGameObject)
    {
        Debug.Assert(mineGameObject, "Mine ctor: GameObject reference is null");
        Number = number;
        Cooldown = cooldown;
        MineGameObject = mineGameObject;
    }

    // ─────────────── API ───────────────

    /// <summary>
    /// Слой‑обёртка над GameObject.activeSelf.  
    /// Для чтения: `mine.Active`; для записи: `mine.Active = true`.
    /// </summary>
    public bool Active
    {
        get => MineGameObject && MineGameObject.activeSelf;
        set
        {
            if (MineGameObject)
                MineGameObject.SetActive(value);
            else
                Debug.LogError("Mine: GameObject reference lost — cannot change active state.");
        }
    }

    /// <summary>Старый метод оставлен ради обратной совместимости.</summary>
    public void SetActive(bool isActive) => Active = isActive;
}
