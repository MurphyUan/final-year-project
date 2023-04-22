using System;
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
    private string playerName;

    [SerializeField] private float HeartBeatTimerMax = 15f;

    private async void Start() {
        // Initialise Unity Services
        await UnityServices.InitializeAsync();

        // Anonymous User Sign in - Guest Sign on
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Murphy" + UnityEngine.Random.Range(10,99);
        Debug.Log(playerName);
    }

    public async void AnonymousSignOn(){
        try{
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        } catch (Exception e){
            Debug.Log(e);
        }
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
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Race")}
                }
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
                Debug.Log($"{lobby.Name} {lobby.MaxPlayers} {lobby.Data["GameMode"].Value}");
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

    private async void JoinLobby(string lobbyCode){
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions{
                Player = GetPlayer()
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            Debug.Log($"Joined Lobby: {lobby.Name} {lobby.Players.Count} {lobby.Id}");
            PrintPlayers(lobby);
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void QuickJoinLobby(){
        try{
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            Debug.Log($"Joined Lobby: {lobby.Name} {lobby.Players.Count} {lobby.Id}");
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private Player GetPlayer(){
        return new Player {
            Data = new Dictionary<string, PlayerDataObject>{
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
            }
        };
    }

    private void PrintPlayers(Lobby lobby) {
        Debug.Log($"Players in Lobby: {lobby.Name}");
        foreach (Player player in lobby.Players)
        {
            Debug.Log($"{player.Id} {player.Data["PlayerName"].Value}");
        }
    }

    private async void UpdateLobbyGameMode(string gameMode){
        try{
            await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject>(){
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
                }
            });
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }
}
