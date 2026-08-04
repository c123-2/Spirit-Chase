using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

/// <summary>
/// \u89d2\u8272\u521d\u59cb\u5316\uff1a\u533a\u5206\u672c\u5730\u73a9\u5bb6 vs \u8fdc\u7a0b\u73a9\u5bb6\u3002
/// </summary>
public class PlayerSetup : MonoBehaviour
{
    [SerializeField] private Behaviour[] _localOnlyComponents;

    private PhotonView _pv;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!_pv.IsMine)
        {
            // \u8fdc\u7a0b\u73a9\u5bb6\uff1a\u7981\u7528\u672c\u5730\u4e13\u7528\u7ec4\u4ef6
            foreach (var comp in _localOnlyComponents)
            {
                if (comp != null) comp.enabled = false;
            }

            // \u7981\u7528 PlayerInput
            var input = GetComponent<PlayerInput>();
            if (input != null) input.enabled = false;

            // \u7981\u7528 Controller \u7684 Update\uff08\u901a\u8fc7\u7981\u7528\u811a\u672c\uff09
            var controller = GetComponent<PlayerController>();
            if (controller != null) controller.enabled = false;
        }
        else
        {
            // \u672c\u5730\u73a9\u5bb6\uff1a\u8bbe\u7f6e\u540d\u79f0\u548c Tag
            gameObject.name = "LocalPlayer_" + PhotonNetwork.LocalPlayer.NickName;
        }
    }
}
