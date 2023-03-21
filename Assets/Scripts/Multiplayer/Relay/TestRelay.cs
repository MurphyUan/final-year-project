using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
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

        // await InitRelay(lobbySize);
    }

    private async void InitRelay(int lobbySize)
    {
        try{
          Allocation allocation = await RelayService.Instance.CreateAllocationAsync(lobbySize - 1);
          string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
          Debug.Log(joinCode);
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
        
    }
}
