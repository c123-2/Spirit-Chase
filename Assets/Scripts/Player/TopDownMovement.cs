using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 2D \u4fef\u89c6\u79fb\u52a8\u7ec4\u4ef6\u3002WASD \u79fb\u52a8\uff0cRigidbody2D.velocity \u9a71\u52a8\u3002
/// \u4efb\u52a1 2.8
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Vector2 _lastDirection = Vector2.down;

    public Vector2 MoveInput => _moveInput;
    public Vector2 LastDirection => _lastDirection;
    public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>\u7531 PlayerController \u8c03\u7528\uff0c\u8bbe\u7f6e\u6765\u81ea Input System \u7684\u79fb\u52a8\u503c</summary>
    public void SetMoveInput(Vector2 input)
    {
        _moveInput = input.normalized;
        if (_moveInput.sqrMagnitude > 0.01f)
            _lastDirection = _moveInput;
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveInput * _moveSpeed;
    }

    /// <summary>\u9762\u5411\u65b9\u5411\uff08\u7528\u4e8e\u52a8\u753b\u6216\u65cb\u8f6c\uff09</summary>
    public float GetFacingAngle()
    {
        return Mathf.Atan2(_lastDirection.y, _lastDirection.x) * Mathf.Rad2Deg;
    }
}
