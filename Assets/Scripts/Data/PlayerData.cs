using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// \u73a9\u5bb6\u6570\u636e ScriptableObject\uff0c\u5b9a\u4e49\u89d2\u8272\u5c5e\u6027\u3002
/// \u4efb\u52a1 2.2
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "SpiritChase/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Identity")]
    public string playerName = "Player";
    public PlayerRole role = PlayerRole.Human;

    [Header("Movement")]
    [Range(1f, 15f)] public float moveSpeed = 7f;
    public float humanSpeed = 8f;
    public float ghostSpeed = 6f;
    public float smallGhostSpeed = 5f;

    [Header("Abilities")]
    public float infectionCooldown = 3f;
    public float infectionRange = 1.5f;

    [Header("Buff")]
    public List<string> activeBuffs = new List<string>();
    public int maxBuffs = 3;

    /// <summary>\u6839\u636e\u5f53\u524d\u9635\u8425\u83b7\u53d6\u5bf9\u5e94\u7684\u79fb\u52a8\u901f\u5ea6</summary>
    public float GetCurrentSpeed()
    {
        return role switch
        {
            PlayerRole.Human => humanSpeed,
            PlayerRole.OriginalGhost => ghostSpeed,
            PlayerRole.SmallGhost => smallGhostSpeed,
            _ => moveSpeed
        };
    }

    /// <summary>\u5207\u6362\u9635\u8425</summary>
    public void SetRole(PlayerRole newRole)
    {
        role = newRole;
        moveSpeed = GetCurrentSpeed();
    }
}
