using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// \u73a9\u5bb6\u63a7\u5236\u5668\u57fa\u7c7b\u3002\u6301\u6709 PlayerData\u3001TopDownMovement\uff0c\u7ed1\u5b9a Input System\u3002
/// \u4efb\u52a1 2.4
/// </summary>
[RequireComponent(typeof(TopDownMovement))]
public abstract class PlayerController : MonoBehaviour
{
    [SerializeField] protected PlayerData _playerData;
    protected TopDownMovement _movement;
    protected PlayerInput _playerInput;

    public PlayerRole Role => _playerData != null ? _playerData.role : PlayerRole.Human;
    public PlayerData Data => _playerData;
    public TopDownMovement Movement => _movement;

    protected virtual void Awake()
    {
        _movement = GetComponent<TopDownMovement>();
        _playerInput = GetComponent<PlayerInput>();

        if (_playerData != null)
            _movement.MoveSpeed = _playerData.GetCurrentSpeed();
    }

    protected virtual void Start()
    {
        PlayerManager.Instance?.Register(this);
    }

    protected virtual void OnDestroy()
    {
        PlayerManager.Instance?.Unregister(this);
    }

    /// <summary>\u5207\u6362\u9635\u8425\uff0c\u5b50\u7c7b\u53ef\u91cd\u5199\u6dfb\u52a0\u7279\u6548</summary>
    public virtual void SwitchRole(PlayerRole newRole)
    {
        if (_playerData == null) return;
        var oldRole = _playerData.role;
        _playerData.SetRole(newRole);
        _movement.MoveSpeed = _playerData.GetCurrentSpeed();

        EventManager.Instance?.Emit("RoleChanged", this, oldRole, newRole);
    }

    /// <summary>Input System \u56de\u8c03\uff1a\u79fb\u52a8</summary>
    public void OnMove(InputValue value)
    {
        _movement.SetMoveInput(value.Get<Vector2>());
    }
}
