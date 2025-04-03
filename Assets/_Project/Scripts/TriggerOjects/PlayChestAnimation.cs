using UnityEngine;

public class PlayChestAnimation : MonoBehaviour, ICheckableTrigger
{
    [SerializeField] private string animationName = "Armature_Chest|Chest_Open";
    private Animator _animator;
    public bool IsDone { get; private set; }

    private void Awake()
    {
        // Ищем Animator на этом объекте или его детях
        _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Animator not found on this object or its children!", this);
        }
    }

    public void Trrigered()
    {
        OpenChest();
    }

    public void OpenChest()
    {
        if (_animator != null)
        {
            _animator.Play(animationName);
        }
    }

    private bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}