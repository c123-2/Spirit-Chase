using UnityEngine;

/// <summary>
/// \u51c0\u5316\u7cfb\u7edf\u3002\u5c0f\u9b3c\u8fdb\u5165\u796d\u575b\u533a\u57df\u2192\u53d8\u56de\u4eba\u7c7b\u3002
/// \u4efb\u52a1 2.12
/// </summary>
public class PurificationSystem : MonoBehaviour
{
    [SerializeField] private float _purificationDelay = 2f;
    [SerializeField] private float _purificationCooldown = 5f;
    private float _cooldownTimer;

    public bool IsReady => _cooldownTimer >= _purificationCooldown;

    private void Start()
    {
        _cooldownTimer = _purificationCooldown;
    }

    private void Update()
    {
        if (_cooldownTimer < _purificationCooldown)
            _cooldownTimer += Time.deltaTime;
    }

    /// <summary>\u51c0\u5316\u4e00\u4e2a\u5c0f\u9b3c</summary>
    public bool TryPurify(SmallGhostController target)
    {
        if (!IsReady || target == null) return false;
        if (!target.CanBePurified) return false;

        _cooldownTimer = 0;
        StartCoroutine(PurifyRoutine(target));
        return true;
    }

    private System.Collections.IEnumerator PurifyRoutine(SmallGhostController target)
    {
        yield return new WaitForSeconds(_purificationDelay);
        if (target != null)
            target.OnPurified();
    }
}
