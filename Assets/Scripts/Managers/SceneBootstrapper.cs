using UnityEngine;

/// <summary>
/// \u573a\u666f\u521d\u59cb\u5316\uff1a\u542f\u52a8\u6e38\u620f\u6d41\u7a0b\u3002
/// </summary>
public class SceneBootstrapper : MonoBehaviour
{
    [SerializeField] private float _gameDuration = 180f;
    [SerializeField] private bool _autoStart = true;

    private void Start()
    {
        if (_autoStart)
            Invoke(nameof(StartGame), 0.5f);
    }

    public void StartGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetState(GameManager.GameState.Playing);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
