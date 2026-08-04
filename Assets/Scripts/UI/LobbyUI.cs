using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

/// <summary>
/// \u7b80\u5355\u5927\u5385 UI\uff1a\u663e\u793a\u623f\u95f4\u5217\u8868\u3001\u521b\u5efa/\u52a0\u5165\u623f\u95f4\u3002
/// </summary>
public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Button _createRoomBtn;
    [SerializeField] private Button _joinRandomBtn;
    [SerializeField] private Button _startGameBtn;
    [SerializeField] private Transform _roomListParent;
    [SerializeField] private GameObject _roomEntryPrefab;

    private void Start()
    {
        if (PhotonManager.Instance == null)
        {
            Debug.LogError("[LobbyUI] PhotonManager not found!");
            return;
        }

        PhotonManager.Instance.OnJoinedLobbyEvent += OnConnected;
        PhotonManager.Instance.OnRoomListUpdated += UpdateRoomList;

        _createRoomBtn?.onClick.AddListener(CreateRoom);
        _joinRandomBtn?.onClick.AddListener(JoinRandom);
        _startGameBtn?.onClick.AddListener(StartGame);

        if (_startGameBtn != null)
            _startGameBtn.interactable = false;

        UpdateStatus();
    }

    private void Update()
    {
        UpdateStatus();
        if (_startGameBtn != null)
            _startGameBtn.interactable = PhotonManager.Instance.IsMaster &&
                                          PhotonManager.Instance.InRoom &&
                                          PhotonManager.Instance.PlayerCount >= 2;
    }

    private void UpdateStatus()
    {
        if (_statusText == null) return;
        var pm = PhotonManager.Instance;
        if (pm == null) return;

        _statusText.text = pm.IsConnected
            ? $"Connected | Room: {(pm.InRoom ? PhotonNetwork.CurrentRoom.Name : "None")} | Players: {pm.PlayerCount}/5"
            : "Connecting...";
    }

    private void OnConnected()
    {
        Debug.Log("[LobbyUI] Connected to lobby.");
    }

    private void CreateRoom()
    {
        string name = _roomNameInput != null && !string.IsNullOrWhiteSpace(_roomNameInput.text)
            ? _roomNameInput.text
            : "SpiritChase_" + Random.Range(1000, 9999);
        PhotonManager.Instance.CreateRoom(name);
    }

    private void JoinRandom()
    {
        PhotonManager.Instance.JoinRandomRoom();
    }

    private void StartGame()
    {
        PhotonManager.Instance.StartGame();
    }

    private void UpdateRoomList()
    {
        // Simplified: just log available rooms
        var rooms = PhotonManager.Instance.CachedRoomList;
        Debug.Log($"[LobbyUI] Rooms available: {rooms.Count}");
    }

    private void OnDestroy()
    {
        if (PhotonManager.Instance != null)
        {
            PhotonManager.Instance.OnJoinedLobbyEvent -= OnConnected;
            PhotonManager.Instance.OnRoomListUpdated -= UpdateRoomList;
        }
    }
}
