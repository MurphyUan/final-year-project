using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

public class TestRelay : MonoBehaviour
{

    [SerializeField] private int lobbySize = 3;

    private async void Start()
    {
        // Asynchronous Call to Unity Game Services
        await UnityServices.InitializeAsync();
        // Asynchronous Callback to Display PlayerID
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
        };
        // Register User Anonymously (TEMP)
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void InitRelay(int lobbySize)
    {
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(lobbySize - 1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
        
    }

    private async void JoinRelay(string joinCode)
    {
        try{
            Debug.Log($"Joining Relay with {joinCode}");
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }catch(RelayServiceException e){
            Debug.Log(e);
        }
    }
}
