using UnityEngine;
using TMPro;

/// <summary>
/// HUD \u63a7\u5236\u5668\uff1a\u5012\u8ba1\u65f6\u3001\u9635\u8425\u8ba1\u6570\u3001\u80dc\u5229/\u5931\u8d25\u663e\u793a\u3002
/// </summary>
public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _factionText;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _resultText;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeChanged += UpdateTimer;
            GameManager.Instance.OnStateChanged += OnStateChanged;
        }

        EventManager.Instance?.On("RoleChanged", _ => UpdateFactionCount());
        EventManager.Instance?.On("GhostWin", _ => ShowResult("\u9b3c\u80dc\u5229\uff01\u5168\u5458\u88ab\u611f\u67d3\uff01"));

        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);

        UpdateFactionCount();
    }

    private void Update()
    {
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
        _factionText.text = $"\u4eba\u7c7b: {PlayerManager.Instance.HumanCount}  \u9b3c: {PlayerManager.Instance.OriginalGhostCount + PlayerManager.Instance.SmallGhostCount}";
    }

    private void OnStateChanged(GameManager.GameState oldState, GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.GameOver)
        {
            if (GameManager.Instance.RemainingTime <= 0)
                ShowResult("\u4eba\u7c7b\u80dc\u5229\uff01\u65f6\u95f4\u8017\u5c3d\uff01");
        }
    }

    private void ShowResult(string msg)
    {
        if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
        if (_resultText != null) _resultText.text = msg;
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
