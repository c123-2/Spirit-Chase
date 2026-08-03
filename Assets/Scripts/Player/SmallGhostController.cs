using UnityEngine;

/// <summary>
/// \u5c0f\u9b3c\u63a7\u5236\u5668\u3002\u88ab\u611f\u67d3\u540e\u7684\u72b6\u6001\uff0c\u79fb\u52a8\u901f\u5ea6\u964d\u4f4e\uff0c\u53ef\u88ab\u51c0\u5316\u3002
/// \u4efb\u52a1 2.7
/// </summary>
public class SmallGhostController : PlayerController
{
    [Header("Purification")]
    [SerializeField] private bool _canBePurified = true;

    public bool CanBePurified => _canBePurified;

    protected override void Awake()
    {
        base.Awake();
        if (_playerData != null)
            _playerData.SetRole(PlayerRole.SmallGhost);
    }

    /// <summary>\u88ab\u51c0\u5316\u56de\u8c03\uff0c\u7531 PurificationSystem \u8c03\u7528</summary>
    public void OnPurified()
    {
        SwitchRole(PlayerRole.Human);
        EventManager.Instance?.Emit("Purified", this);
    }

    public override void SwitchRole(PlayerRole newRole)
    {
        base.SwitchRole(newRole);
    }
}
