using UnityEngine;

/// <summary>
/// Контроллер перемещения/поворота любой «толкаемой» платформы (ящика, тележки и т. п.).
/// </summary>
public class MovingPlatformController : MonoBehaviour
{
    #region Inspector
    [Header("Платформа")]
    [SerializeField] private Transform _platform;          // Объект, который двигаем
    public Transform Platform => _platform;

    [Header("Параметры движения")]
    [SerializeField] private float _moveSpeed = 2.0f;  // м/с
    [SerializeField] private float _rotationSpeed = 200f;  // °/с
    #endregion

    public bool IsMoving => _needToMove;

    /*-------------------------------------------------------------------*/

    private bool _needToMove = false;
    private Rigidbody _rb;                      // необязательный Rigidbody

    private void Awake()
    {
        if (_platform == null)
        {
            Debug.LogWarning($"{name}: Платформа не назначена, берём self");
            _platform = transform;
        }

        _rb = _platform.GetComponent<Rigidbody>();
    }

    /*======================== INPUT & LOGIC ==========================*/

    private void Update()
    {
        if (!_needToMove) return;

        // --- Получаем ввод игрока -----------------------------------
        InputManager inputMgr = InputManager.GetInstance();
        if (inputMgr == null) { Debug.LogError("InputManager not found"); return; }

        Vector2 input = inputMgr.GetMoveDirection();               // (x = горизонталь, y = вертикаль)
        Vector3 move = new Vector3(input.x, 0f, input.y).normalized;

        if (move.sqrMagnitude < 0.01f) return;                     // ничего не делаем

        // --- Поворот (делаем здесь, т.к. RotateTowards не зависит от физики)
        Quaternion targetRot = Quaternion.LookRotation(move);
        _platform.rotation = Quaternion.RotateTowards(
            _platform.rotation,
            targetRot,
            _rotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_needToMove) return;

        InputManager inputMgr = InputManager.GetInstance();
        if (inputMgr == null) return;

        Vector2 input = inputMgr.GetMoveDirection();
        Vector3 move = new Vector3(input.x, 0f, input.y).normalized;

        if (move.sqrMagnitude < 0.01f) return;

        // --- Перемещение (если есть Rigidbody — безопаснее через него)
        float step = _moveSpeed * Time.fixedDeltaTime;
        Vector3 newPos = _platform.position + move * step;

        if (_rb != null && !_rb.isKinematic)
            _rb.MovePosition(newPos);
        else
            _platform.position = newPos;
    }

    /*======================== PUBLIC API =============================*/

    /// <summary>Вызывается извне (например, из PlayerMoveController) при нажатии E.</summary>
    public void ToggleMovement()
    {
        _needToMove = !_needToMove;
        Debug.Log($"{name}: moving = {_needToMove}");
    }
}
