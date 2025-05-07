using UnityEngine;

public class PlayChestAnimation : MonoBehaviour, ITriggerable   // ключевая строка
{
    [SerializeField] private string animationName = "Armature_Chest|Chest_Open";
    private Animator _anim;
    private bool _opened;

    private void Awake() => _anim = GetComponentInChildren<Animator>();

    // вызывается InteractManager-ом через TryTrigger(...)
    public void Triggered()
    {
        if (_opened) return;
        _opened = true;
        if (_anim) _anim.Play(animationName);
    }
}
