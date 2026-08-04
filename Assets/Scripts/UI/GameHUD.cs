using UnityEngine;
using TMPro;

/// <summary>
/// HUD controller: countdown, faction count, win/lose display.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _factionText;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _resultText;

    private bool _gameEnded;
    private bool _gameStarted;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeChanged += UpdateTimer;
            GameManager.Instance.OnStateChanged += OnStateChanged;
        }

        EventManager.Instance?.On("RoleChanged", _ => UpdateFactionCount());
        EventManager.Instance?.On("GhostWin", _ => ShowResult("Ghosts Win!"));
        EventManager.Instance?.On("TimeUp", _ => ShowResult("Humans Win!"));

        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);

        UpdateFactionCount();

        // Auto-start the game after a brief delay
        Invoke(nameof(StartPlaying), 1f);
    }

    private void StartPlaying()
    {
        if (GameManager.Instance != null && !_gameStarted)
        {
            _gameStarted = true;
            GameManager.Instance.SetState(GameManager.GameState.Playing);
        }
    }

    private void Update()
    {
        if (_gameEnded) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            UpdateTimer(GameManager.Instance.RemainingTime);
        }
    }

    private void UpdateTimer(float time)
    {
        if (_timerText == null) return;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        _timerText.text = $"{minutes:D2}:{seconds:D2}";

        if (time <= 30)
            _timerText.color = Color.red;
        else if (time <= 60)
            _timerText.color = Color.yellow;
    }

    private void UpdateFactionCount()
    {
        if (_factionText == null || PlayerManager.Instance == null) return;
        _factionText.text = $"Humans: {PlayerManager.Instance.HumanCount}  Ghosts: {PlayerManager.Instance.OriginalGhostCount + PlayerManager.Instance.SmallGhostCount}";
    }

    private void OnStateChanged(GameManager.GameState oldState, GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.GameOver && !_gameEnded)
        {
            _gameEnded = true;
            if (GameManager.Instance.RemainingTime <= 0)
                ShowResult("Humans Win!");
        }
    }

    private void ShowResult(string msg)
    {
        if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
        if (_resultText != null) _resultText.text = msg;
        _gameEnded = true;
    }

    public void OnRetryClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeChanged -= UpdateTimer;
            GameManager.Instance.OnStateChanged -= OnStateChanged;
        }
    }
}
