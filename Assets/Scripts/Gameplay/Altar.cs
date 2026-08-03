using UnityEngine;

/// <summary>
/// \u796d\u575b\u3002Trigger\u68c0\u6d4b\u5c0f\u9b3c\u8fdb\u5165\u2192\u51c0\u5316\u3002
/// \u4efb\u52a1 2.13
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Altar : MonoBehaviour
{
    [SerializeField] private PurificationSystem _purificationSystem;
    [SerializeField] private SpriteRenderer _visual;
    [SerializeField] private Color _readyColor = Color.cyan;
    [SerializeField] private Color _cooldownColor = Color.gray;

    private CircleCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.isTrigger = true;

        if (_purificationSystem == null)
            _purificationSystem = GetComponent<PurificationSystem>();
    }

    private void Update()
    {
        if (_visual != null && _purificationSystem != null)
        {
            _visual.color = _purificationSystem.IsReady ? _readyColor : _cooldownColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_purificationSystem == null || !_purificationSystem.IsReady) return;

        var smallGhost = other.GetComponent<SmallGhostController>();
        if (smallGhost != null)
        {
            _purificationSystem.TryPurify(smallGhost);
        }
    }

    private void OnDrawGizmos()
    {
        var col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, col.radius * transform.localScale.x);
        }
    }
}
