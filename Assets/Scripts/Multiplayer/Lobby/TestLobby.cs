using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    private Lobby hostLobby;
    private float heartbeatTimer;

    [SerializeField] private float HeartBeatTimerMax = 15f;

    private async void Start() {
        // Initialise Unity Services
        await UnityServices.InitializeAsync();

        // Anonymous User Sign in - Guest Sign on
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update() {
        HandleLobbyHeartbeat();
    }

    private async void CreateLobby(){
        try {
            // TODO - Pass through as parameter, as a LobbyParameters Object
            string lobbyName = "My Lobby";
            int maxPlayers = 4;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = true,
            };
            // END-OF-TODO
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;

            Debug.Log($"Create Lobby: {lobby.Name} {lobby.MaxPlayers} {lobby.Id} {lobby.LobbyCode}");
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void ListLobbies(){
        try{
            // TODO - Pass through as parameter, with options being selected in a menu
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Count = 25,
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder> {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };
            //END-OF-TODO

            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log($"Lobbies found: {response.Results.Count}");

            foreach(Lobby lobby in response.Results){
                Debug.Log($"{lobby.Name} {lobby.MaxPlayers}");
            }
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void HandleLobbyHeartbeat(){
        if (hostLobby == null) return;

        heartbeatTimer -= Time.deltaTime;

        if(heartbeatTimer > 0f) return;

        heartbeatTimer = HeartBeatTimerMax;

        await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
    }

    // Joins First Lobby it finds
    private async void JoinLobby(){
        try{
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Lobby localLobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);

            Debug.Log($"Joined: {localLobby.Name} {localLobby.Players.Count} {localLobby.Id}");
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }
}
