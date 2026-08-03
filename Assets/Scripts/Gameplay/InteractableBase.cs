using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// \u4ea4\u4e92\u7269\u57fa\u7c7b\u3002\u9760\u8fd1\u65f6\u663e\u793a\u63d0\u793a\uff0c\u6309\u952e\u89e6\u53d1\u4ea4\u4e92\u3002
/// \u4efb\u52a1 5.1\uff08\u63d0\u524d\u5b9e\u73b0\uff0cHumanController \u4f9d\u8d56\uff09
/// </summary>
public class InteractableBase : MonoBehaviour
{
    [SerializeField] protected string _prompt = "\u6309 E \u4ea4\u4e92";
    [SerializeField] protected float _interactCooldown = 0.5f;
    [SerializeField] protected bool _oneTimeUse = false;

    public UnityEvent<GameObject> OnInteracted;
    public bool CanInteract => !_oneTimeUse || _interactCount == 0;
    public string Prompt => _prompt;

    protected float _lastInteractTime;
    protected int _interactCount;

    public virtual void Interact(GameObject interactor)
    {
        if (Time.time - _lastInteractTime < _interactCooldown) return;
        if (!CanInteract) return;

        _lastInteractTime = Time.time;
        _interactCount++;
        OnInteracted?.Invoke(interactor);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            EventManager.Instance?.Emit("ShowPrompt", _prompt);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            EventManager.Instance?.Emit("HidePrompt");
    }
}
