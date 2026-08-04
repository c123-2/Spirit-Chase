using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

/// <summary>
/// Photon \u8fde\u63a5\u7ba1\u7406\uff1a\u767b\u5f55\u3001\u521b\u5efa/\u52a0\u5165\u623f\u95f4\u3001\u65ad\u5f00\u5904\u7406\u3002
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }

    [SerializeField] private string _appId = "0ecec075-e884-4ea1-bad9-d144f779fba3";
    [SerializeField] private string _gameVersion = "1.0";
    [SerializeField] private byte _maxPlayers = 5;
    [SerializeField] private bool _autoJoinLobby = true;

    public bool IsConnected => PhotonNetwork.IsConnected;
    public bool InRoom => PhotonNetwork.InRoom;
    public bool IsMaster => PhotonNetwork.IsMasterClient;
    public int PlayerCount => PhotonNetwork.CurrentRoom?.PlayerCount ?? 0;

    public event System.Action OnJoinedLobbyEvent;
    public event System.Action OnRoomListUpdated;
    public List<RoomInfo> CachedRoomList { get; private set; } = new List<RoomInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = _appId;
    }

    private void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (IsConnected)
        {
            Debug.Log("[PhotonManager] Already connected.");
            return;
        }
        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("[PhotonManager] Connecting to Photon...");
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    /// <summary>\u521b\u5efa\u623f\u95f4</summary>
    public void CreateRoom(string roomName)
    {
        if (!IsConnected) return;
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = _maxPlayers,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.CreateRoom(roomName, options);
    }

    /// <summary>\u52a0\u5165\u968f\u673a\u623f\u95f4</summary>
    public void JoinRandomRoom()
    {
        if (!IsConnected) return;
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>\u52a0\u5165\u6307\u5b9a\u623f\u95f4</summary>
    public void JoinRoom(string roomName)
    {
        if (!IsConnected) return;
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>\u79bb\u5f00\u623f\u95f4</summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>\u5f00\u59cb\u6e38\u620f\uff08\u4ec5 Master Client\uff09</summary>
    public void StartGame()
    {
        if (!IsMaster || !InRoom) return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("Main");
    }

    // \u2500\u2500 Photon Callbacks \u2500\u2500

    public override void OnConnectedToMaster()
    {
        Debug.Log("[PhotonManager] Connected to Master Server.");
        if (_autoJoinLobby)
            PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"[PhotonManager] Disconnected: {cause}");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[PhotonManager] Joined Lobby.");
        OnJoinedLobbyEvent?.Invoke();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        CachedRoomList = roomList;
        OnRoomListUpdated?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[PhotonManager] Joined Room: {PhotonNetwork.CurrentRoom.Name} (Players: {PlayerCount})");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[PhotonManager] Create Room Failed: {message}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"[PhotonManager] Join Random Failed, creating new room...");
        CreateRoom("SpiritChase_" + Random.Range(1000, 9999));
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[PhotonManager] Join Room Failed: {message}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PhotonManager] Player joined: {newPlayer.NickName} ({PlayerCount}/5)");
        EventManager.Instance?.Emit("PlayerJoinedRoom", newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PhotonManager] Player left: {otherPlayer.NickName} ({PlayerCount}/5)");
        EventManager.Instance?.Emit("PlayerLeftRoom", otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"[PhotonManager] New Master Client: {newMasterClient.NickName}");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("[PhotonManager] Left room.");
    }
}
