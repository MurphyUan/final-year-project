using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    [SerializeField] RelayController relay;

    private void Awake() 
    {
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        hostBtn.onClick.AddListener(() => {
            // NetworkManager.Singleton.StartHost();
            relay.InitRelay();
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }
}
