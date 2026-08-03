using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// \u73a9\u5bb6\u7ba1\u7406\u5668\uff0c\u7ba1\u74065\u4e2a\u73a9\u5bb6\u7684\u751f\u547d\u5468\u671f\u548c\u9635\u8425\u7edf\u8ba1\u3002
/// \u4efb\u52a1 2.3
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    private readonly List<PlayerController> _allPlayers = new List<PlayerController>();

    public int TotalCount => _allPlayers.Count;
    public int HumanCount => _allPlayers.Count(p => p.Role == PlayerRole.Human);
    public int OriginalGhostCount => _allPlayers.Count(p => p.Role == PlayerRole.OriginalGhost);
    public int SmallGhostCount => _allPlayers.Count(p => p.Role == PlayerRole.SmallGhost);

    /// <summary>\u6ce8\u518c\u73a9\u5bb6</summary>
    public void Register(PlayerController player)
    {
        if (!_allPlayers.Contains(player))
        {
            _allPlayers.Add(player);
            EventManager.Instance?.Emit("PlayerRegistered", player);
        }
    }

    /// <summary>\u6ce8\u9500\u73a9\u5bb6</summary>
    public void Unregister(PlayerController player)
    {
        if (_allPlayers.Remove(player))
        {
            EventManager.Instance?.Emit("PlayerUnregistered", player);
            CheckWinCondition();
        }
    }

    /// <summary>\u83b7\u53d6\u6307\u5b9a\u9635\u8425\u7684\u73a9\u5bb6\u5217\u8868</summary>
    public List<PlayerController> GetHumans() =>
        _allPlayers.Where(p => p.Role == PlayerRole.Human).ToList();

    public List<PlayerController> GetGhosts() =>
        _allPlayers.Where(p => p.Role == PlayerRole.OriginalGhost || p.Role == PlayerRole.SmallGhost).ToList();

    public List<PlayerController> GetOriginalGhosts() =>
        _allPlayers.Where(p => p.Role == PlayerRole.OriginalGhost).ToList();

    public List<PlayerController> GetSmallGhosts() =>
        _allPlayers.Where(p => p.Role == PlayerRole.SmallGhost).ToList();

    /// <summary>\u6309\u7c7b\u578b\u83b7\u53d6\u6700\u8fd1\u73a9\u5bb6</summary>
    public PlayerController GetNearestHuman(Vector2 position)
    {
        var humans = GetHumans();
        if (humans.Count == 0) return null;
        humans.Sort((a, b) =>
            Vector2.Distance(position, a.transform.position)
            .CompareTo(Vector2.Distance(position, b.transform.position)));
        return humans[0];
    }

    /// <summary>\u80dc\u5229\u6761\u4ef6\u68c0\u6d4b\uff1a\u5168\u90e8\u53d8\u9b3c\u2192\u9b3c\u80dc</summary>
    public void CheckWinCondition()
    {
        if (HumanCount == 0 && SmallGhostCount > 0)
        {
            GameManager.Instance?.GhostWin();
        }
    }
}
