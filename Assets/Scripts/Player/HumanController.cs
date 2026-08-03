using UnityEngine;

/// <summary>
/// \u4eba\u7c7b\u63a7\u5236\u5668\u3002WASD\u79fb\u52a8\uff08\u7ee7\u627fPlayerController\uff09\uff0cE\u952e\u4ea4\u4e92\u3002
/// \u4efb\u52a1 2.5
/// </summary>
public class HumanController : PlayerController
{
    [Header("Interaction")]
    [SerializeField] private float _interactRange = 2f;
    [SerializeField] private LayerMask _interactableLayer;

    private InteractableBase _currentInteractable;

    protected override void Awake()
    {
        base.Awake();
        if (_playerData != null)
            _playerData.SetRole(PlayerRole.Human);
    }

    /// <summary>Input System \u56de\u8c03\uff1a\u4ea4\u4e92\u952e E</summary>
    public void OnInteract()
    {
        TryInteract();
    }

    /// <summary>Input System \u56de\u8c03\uff1a\u6682\u505c</summary>
    public void OnPause()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;
        if (gm.CurrentState == GameManager.GameState.Playing)
            gm.SetState(GameManager.GameState.Paused);
        else if (gm.CurrentState == GameManager.GameState.Paused)
            gm.SetState(GameManager.GameState.Playing);
    }

    /// <summary>Input System \u56de\u8c03\uff1a\u5546\u5e97</summary>
    public void OnOpenShop()
    {
        EventManager.Instance?.Emit("OpenShop", this);
    }

    private void TryInteract()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _interactRange, _interactableLayer);
        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<InteractableBase>();
            if (interactable != null && interactable.CanInteract)
            {
                interactable.Interact(gameObject);
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<InteractableBase>();
        if (interactable != null)
            _currentInteractable = interactable;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<InteractableBase>();
        if (interactable == _currentInteractable)
            _currentInteractable = null;
    }
}
