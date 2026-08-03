using UnityEngine;

/// <summary>
/// \u6bcd\u4f53\u9b3c\u63a7\u5236\u5668\u3002\u79fb\u52a8 + \u611f\u67d3\u653b\u51fb\uff08\u7a7a\u683c\u952e\uff09\u3002
/// \u4efb\u52a1 2.6
/// </summary>
public class GhostController : PlayerController
{
    [Header("Infection")]
    [SerializeField] private float _infectionCooldown = 3f;
    [SerializeField] private float _infectionRange = 1.5f;
    private float _infectionTimer;

    public float CooldownRemaining => Mathf.Max(0, _infectionCooldown - _infectionTimer);
    public float CooldownPercent => _infectionTimer / _infectionCooldown;
    public bool CanInfect => _infectionTimer >= _infectionCooldown;

    protected override void Awake()
    {
        base.Awake();
        if (_playerData != null)
            _playerData.SetRole(PlayerRole.OriginalGhost);
        _infectionTimer = _infectionCooldown;
    }

    private void Update()
    {
        if (_infectionTimer < _infectionCooldown)
            _infectionTimer += Time.deltaTime;
    }

    /// <summary>Input System \u56de\u8c03\uff1a\u611f\u67d3\u653b\u51fb\uff08\u7a7a\u683c\u952e\u7ed1\u5b9a\u6216\u81ea\u5b9a\u4e49\u6309\u952e\uff09</summary>
    public void OnInfect()
    {
        if (!CanInfect) return;
        TryInfect();
    }

    private void TryInfect()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _infectionRange);
        foreach (var hit in hits)
        {
            var human = hit.GetComponent<HumanController>();
            if (human != null)
            {
                _infectionTimer = 0;
                human.SwitchRole(PlayerRole.SmallGhost);
                EventManager.Instance?.Emit("Infection", this, human);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _infectionRange);
    }
}
