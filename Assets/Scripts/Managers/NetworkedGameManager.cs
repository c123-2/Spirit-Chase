using UnityEngine;
using Photon.Pun;

/// <summary>
/// Master Client \u6743\u5a01\u6e38\u620f\u903b\u8f91\uff1a\u5012\u8ba1\u65f6\u540c\u6b65\u3001\u611f\u67d3\u51c0\u5316 RPC\u3001\u80dc\u5229\u5e7f\u64ad\u3002
/// </summary>
public class NetworkedGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private float _gameDuration = 300f;
    private float _remainingTime;
    private bool _gameStarted;
    private bool _gameEnded;

    public float RemainingTime => _remainingTime;

    public static NetworkedGameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _remainingTime = _gameDuration;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient || !_gameStarted || _gameEnded) return;

        _remainingTime -= Time.deltaTime;

        // \u6bcf\u79d2\u540c\u6b65\u4e00\u6b21\u5012\u8ba1\u65f6
        if (Mathf.FloorToInt(_remainingTime) != Mathf.FloorToInt(_remainingTime + Time.deltaTime))
        {
            GetComponent<PhotonView>()?.RPC(nameof(RPC_SyncTimer), RpcTarget.Others, Mathf.FloorToInt(_remainingTime));
        }

        if (_remainingTime <= 0)
        {
            _remainingTime = 0;
            _gameEnded = true;
            GetComponent<PhotonView>()?.RPC(nameof(RPC_GameOver), RpcTarget.All, "Humans Win!");
        }
    }

    /// <summary>Master \u5f00\u59cb\u6e38\u620f</summary>
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _gameStarted = true;
        _remainingTime = _gameDuration;
        GetComponent<PhotonView>()?.RPC(nameof(RPC_StartGame), RpcTarget.AllBuffered, _gameDuration);
    }

    /// <summary>\u611f\u67d3\u5224\u5b9a\uff08Master \u6743\u5a01\uff09</summary>
    public void ReportInfection(int ghostViewId, int humanViewId)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var humanView = PhotonView.Find(humanViewId);
        if (humanView != null)
        {
            GetComponent<PhotonView>()?.RPC(nameof(RPC_OnInfected), RpcTarget.All, humanViewId);
        }
    }

    /// <summary>\u51c0\u5316\u5224\u5b9a\uff08Master \u6743\u5a01\uff09</summary>
    public void ReportPurification(int smallGhostViewId)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GetComponent<PhotonView>()?.RPC(nameof(RPC_OnPurified), RpcTarget.All, smallGhostViewId);
    }

    /// <summary>\u68c0\u6d4b\u5168\u611f\u67d3\u2192\u9b3c\u80dc</summary>
    public void CheckGhostWin()
    {
        int humanCount = 0;
        var views = FindObjectsByType<NetworkedPlayer>(FindObjectsSortMode.None);
        foreach (var np in views)
        {
            if (np != null && np.GetComponent<PlayerController>()?.Role == PlayerRole.Human)
                humanCount++;
        }

        if (humanCount == 0 && !_gameEnded)
        {
            _gameEnded = true;
            GetComponent<PhotonView>()?.RPC(nameof(RPC_GameOver), RpcTarget.All, "Ghosts Win!");
        }
    }

    // \u2500\u2500 RPCs \u2500\u2500

    [PunRPC]
    private void RPC_StartGame(float duration)
    {
        _gameDuration = duration;
        _remainingTime = duration;
        _gameStarted = true;
        _gameEnded = false;
        Debug.Log($"[NetworkedGM] Game started! {duration}s");
    }

    [PunRPC]
    private void RPC_SyncTimer(int seconds)
    {
        _remainingTime = seconds;
    }

    [PunRPC]
    private void RPC_OnInfected(int humanViewId)
    {
        var humanView = PhotonView.Find(humanViewId);
        if (humanView != null)
        {
            var nc = humanView.GetComponent<NetworkedPlayer>();
            if (nc != null)
                nc.RPC_SetRole((int)PlayerRole.SmallGhost);

            EventManager.Instance?.Emit("Infection", humanView.gameObject);
        }

        // Master checks win condition
        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(CheckGhostWin), 0.5f);
    }

    [PunRPC]
    private void RPC_OnPurified(int smallGhostViewId)
    {
        var ghostView = PhotonView.Find(smallGhostViewId);
        if (ghostView != null)
        {
            var nc = ghostView.GetComponent<NetworkedPlayer>();
            if (nc != null)
                nc.RPC_SetRole((int)PlayerRole.Human);

            EventManager.Instance?.Emit("Purified", ghostView.gameObject);
        }
    }

    [PunRPC]
    private void RPC_GameOver(string result)
    {
        _gameEnded = true;
        EventManager.Instance?.Emit("GameOver", result);
        Debug.Log($"[NetworkedGM] Game Over: {result}");
    }
}
