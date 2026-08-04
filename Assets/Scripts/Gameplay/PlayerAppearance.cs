using UnityEngine;

/// <summary>
/// \u89d2\u8272\u5916\u89c2\u7ba1\u7406\u3002\u6839\u636e\u9635\u8425\u81ea\u52a8\u5207\u6362\u989c\u8272/\u56fe\u6807\u3002
/// \u4efb\u52a1 2.14
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAppearance : MonoBehaviour
{
    [Header("Colors per Role")]
    [SerializeField] private Color _humanColor = Color.blue;
    [SerializeField] private Color _ghostColor = Color.red;
    [SerializeField] private Color _smallGhostColor = new Color(0.6f, 0, 0.3f);

    private SpriteRenderer _sprite;
    private PlayerController _controller;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        UpdateAppearance();
        EventManager.Instance?.On("RoleChanged", OnRoleChanged);
    }

    private void OnDestroy()
    {
        EventManager.Instance?.Off("RoleChanged", OnRoleChanged);
    }

    private void OnRoleChanged(params object[] args)
    {
        if (args.Length >= 1 && args[0] is PlayerController pc && pc == _controller)
            UpdateAppearance();
    }

    public void UpdateAppearance()
    {
        if (_sprite == null) return;

        // Determine role: check data first, then controller type
        PlayerRole role;
        if (_controller != null && _controller.Data != null)
        {
            role = _controller.Role;
        }
        else
        {
            // Fallback: detect by controller type
            role = _controller switch
            {
                GhostController => PlayerRole.OriginalGhost,
                SmallGhostController => PlayerRole.SmallGhost,
                _ => PlayerRole.Human
            };
        }

        _sprite.color = role switch
        {
            PlayerRole.Human => _humanColor,
            PlayerRole.OriginalGhost => _ghostColor,
            PlayerRole.SmallGhost => _smallGhostColor,
            _ => Color.white
        };
    }
}
