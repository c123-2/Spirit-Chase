using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

/// <summary>
/// \u7f51\u7edc\u5316\u89d2\u8272\uff1aPhotonView \u521d\u59cb\u5316\u3001\u6240\u6709\u6743\u63a7\u5236\u3001RPC \u6ce8\u518c\u3002
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class NetworkedPlayer : MonoBehaviourPun, IPunObservable
{
    private PhotonView _pv;
    private PlayerController _controller;
    private PlayerAppearance _appearance;

    public bool IsMine => _pv != null && _pv.IsMine;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
        _controller = GetComponent<PlayerController>();
        _appearance = GetComponent<PlayerAppearance>();
    }

    private void Start()
    {
        // \u5982\u679c\u4e0d\u662f\u672c\u5730\u73a9\u5bb6\u7684\u89d2\u8272\uff0c\u7981\u7528\u8f93\u5165
        if (!_pv.IsMine)
        {
            var input = GetComponent<PlayerInput>();
            if (input != null) input.enabled = false;

            // \u7981\u7528\u63a7\u5236\u5668\u811a\u672c\u7684 Update \u903b\u8f91
            // \u8fdc\u7a0b\u73a9\u5bb6\u53ea\u540c\u6b65\u4f4d\u7f6e\uff0c\u4e0d\u54cd\u5e94\u8f93\u5165
        }

        // \u544a\u77e5\u6240\u6709\u4eba\u8fd9\u4e2a\u89d2\u8272\u7684\u521d\u59cb\u9635\u8425
        if (_pv.IsMine && _controller != null)
        {
            _pv.RPC(nameof(RPC_SetRole), RpcTarget.AllBuffered, (int)_controller.Role);
        }
    }

    /// <summary>RPC: \u540c\u6b65\u9635\u8425\u53d8\u5316</summary>
    [PunRPC]
    public void RPC_SetRole(int roleInt)
    {
        if (_controller != null)
        {
            _controller.SwitchRole((PlayerRole)roleInt);
        }
    }

    /// <summary>RPC: \u901a\u77e5\u611f\u67d3\u4e8b\u4ef6</summary>
    [PunRPC]
    public void RPC_OnInfected(int targetViewId)
    {
        var target = PhotonView.Find(targetViewId);
        if (target != null)
        {
            var nc = target.GetComponent<NetworkedPlayer>();
            if (nc != null)
                nc.RPC_SetRole((int)PlayerRole.SmallGhost);
        }
    }

    /// <summary>RPC: \u901a\u77e5\u51c0\u5316\u4e8b\u4ef6</summary>
    [PunRPC]
    public void RPC_OnPurified(int targetViewId)
    {
        var target = PhotonView.Find(targetViewId);
        if (target != null)
        {
            var nc = target.GetComponent<NetworkedPlayer>();
            if (nc != null)
                nc.RPC_SetRole((int)PlayerRole.Human);
        }
    }

    /// <summary>Photon \u4f4d\u7f6e\u540c\u6b65</summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
