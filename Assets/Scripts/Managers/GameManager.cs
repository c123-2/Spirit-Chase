using UnityEngine;
using System;

/// <summary>
/// \u6e38\u620f\u4e3b\u7ba1\u7406\u5668\uff0c\u7ba1\u7406\u6e38\u620f\u72b6\u6001\u673a\uff1aLobby \u2192 Playing \u2192 GameOver
/// \u4efb\u52a1 1.2\uff1aGameManager \u6e38\u620f\u4e3b\u7ba1\u7406\u5668
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>\u6e38\u620f\u72b6\u6001\u679a\u4e3e</summary>
    public enum GameState
    {
        Lobby,     // \u7b49\u5f85\u73a9\u5bb6\u52a0\u5165
        Playing,   // \u6e38\u620f\u8fdb\u884c\u4e2d
        Paused,    // \u6682\u505c
        GameOver   // \u6e38\u620f\u7ed3\u675f
    }

    public GameState CurrentState { get; private set; } = GameState.Lobby;

    public event Action<GameState, GameState> OnStateChanged;

    [Header("Game Settings")]
    [SerializeField] private float _gameDuration = 600f; // \u9ed8\u8ba410\u5206\u949f
    [SerializeField] private int _humanCount = 4;
    [SerializeField] private int _ghostCount = 1;

    public float GameDuration => _gameDuration;
    public int HumanCount => _humanCount;
    public int GhostCount => _ghostCount;

    private float _remainingTime;

    public float RemainingTime
    {
        get => _remainingTime;
        private set
        {
            _remainingTime = value;
            OnTimeChanged?.Invoke(_remainingTime);
        }
    }

    public event Action<float> OnTimeChanged;

    protected override void Awake()
    {
        base.Awake();
        RemainingTime = _gameDuration;
    }

    /// <summary>\u5207\u6362\u6e38\u620f\u72b6\u6001</summary>
    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        var oldState = CurrentState;
        CurrentState = newState;

        EventManager.Instance?.Emit("GameStateChanged", oldState, newState);
        OnStateChanged?.Invoke(oldState, newState);

        Debug.Log($"[GameManager] State: {oldState} \u2192 {newState}");

        switch (newState)
        {
            case GameState.Lobby:
                OnEnterLobby();
                break;
            case GameState.Playing:
                OnEnterPlaying();
                break;
            case GameState.Paused:
                OnEnterPaused();
                break;
            case GameState.GameOver:
                OnEnterGameOver();
                break;
        }
    }

    private void OnEnterLobby()
    {
        RemainingTime = _gameDuration;
        Time.timeScale = 1f;
    }

    private void OnEnterPlaying()
    {
        RemainingTime = _gameDuration;
        Time.timeScale = 1f;
    }

    private void OnEnterPaused()
    {
        Time.timeScale = 0f;
    }

    private void OnEnterGameOver()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            RemainingTime -= Time.deltaTime;
            if (RemainingTime <= 0)
            {
                RemainingTime = 0;
                OnTimeUp();
            }
        }
    }

    private void OnTimeUp()
    {
        // \u5012\u8ba1\u65f6\u5f52\u96f6 \u2192 \u4eba\u7c7b\u80dc\u5229
        EventManager.Instance?.Emit("TimeUp");
        Debug.Log("[GameManager] Time's up! Humans win!");
        SetState(GameState.GameOver);
    }

    /// <summary>\u6dfb\u52a0\u65f6\u95f4\uff08\u4eba\u7c7b\u4f7f\u7528Buff\u65f6\u8c03\u7528\uff09</summary>
    public void AddTime(float amount)
    {
        RemainingTime += amount;
    }

    /// <summary>\u51cf\u5c11\u65f6\u95f4\uff08\u9b3c\u4f7f\u7528Buff\u65f6\u8c03\u7528\uff09</summary>
    public void ReduceTime(float amount)
    {
        RemainingTime -= amount;
    }

    /// <summary>\u9b3c\u80dc\u5229</summary>
    public void GhostWin()
    {
        if (CurrentState != GameState.Playing) return;
        EventManager.Instance?.Emit("GhostWin");
        Debug.Log("[GameManager] All humans infected! Ghosts win!");
        SetState(GameState.GameOver);
    }
}
