using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class TestRelay : MonoBehaviour
{

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
}
