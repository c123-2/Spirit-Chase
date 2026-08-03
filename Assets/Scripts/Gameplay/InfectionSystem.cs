using UnityEngine;

/// <summary>
/// \u611f\u67d3\u7cfb\u7edf\u3002\u6302\u8f7d\u5728\u6bcd\u4f53\u9b3c\u4e0a\uff0cOnTriggerEnter2D \u68c0\u6d4b\u4eba\u7c7b\u5e76\u611f\u67d3\u3002
/// \u4efb\u52a1 2.11
/// </summary>
public class InfectionSystem : MonoBehaviour
{
    [SerializeField] private float _infectionCooldown = 3f;
    [SerializeField] private bool _enabled = true;
    private float _cooldownTimer;

    public bool IsReady => _cooldownTimer >= _infectionCooldown;
    public float CooldownProgress => Mathf.Clamp01(_cooldownTimer / _infectionCooldown);

    private void Start()
    {
        _cooldownTimer = _infectionCooldown;
    }

    private void Update()
    {
        if (_cooldownTimer < _infectionCooldown)
            _cooldownTimer += Time.deltaTime;
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    /// <summary>\u5c1d\u8bd5\u611f\u67d3\u76ee\u6807</summary>
    public bool TryInfect(HumanController target)
    {
        if (!_enabled || !IsReady) return false;
        if (target == null) return false;

        _cooldownTimer = 0;
        target.SwitchRole(PlayerRole.SmallGhost);
        EventManager.Instance?.Emit("Infection", gameObject, target.gameObject);
        return true;
    }

    /// <summary>\u533a\u57df\u611f\u67d3\uff1a\u5bf9\u8303\u56f4\u5185\u7684\u6240\u6709\u4eba\u7c7b\u6267\u884c\u611f\u67d3</summary>
    public int TryInfectArea(Vector2 center, float radius)
    {
        if (!_enabled || !IsReady) return 0;

        var hits = Physics2D.OverlapCircleAll(center, radius);
        int count = 0;
        foreach (var hit in hits)
        {
            var human = hit.GetComponent<HumanController>();
            if (human != null && TryInfect(human))
                count++;
        }
        return count;
    }
}
