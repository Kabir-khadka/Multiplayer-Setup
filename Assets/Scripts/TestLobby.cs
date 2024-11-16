using QFSW.QC;
using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    //Just storing the reference of the lobby created by CreateLobby method.
    private Lobby hostLobby;
    private float heartbeatTimer;

    //Function to start unity services
    private async void Start()
    {
        //Start Unity Services
        await UnityServices.InitializeAsync();

        //Adding an event listener
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        //Letting users sign in anonymously
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat ()
    {
        if(hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;

            if(heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }

    }

    //Method for Creating Lobby
    [Command]
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "My Lobby";
            int maxPlayers = 4;

            //Making lobby private touching CreateLobbyOptions
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false
            };

            //await method to create a lobby calling createlobbyasync
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            //Storing the instance
            hostLobby = lobby;

            Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.LobbyCode);
       
        } catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void ListLobbies()
    {
        try
        {
            //Adding filters to the lobby
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };


          QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Method to join lobby
    [Command]
    private async void JoinLobbyByCode(string lobbyCode)
    {
        try {
            /*//Method that retrieves the available lobby 
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            //Joins lobby first one
            Lobby joinLobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);*/

            await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);

            Debug.Log("Lobby joined with the code: " + lobbyCode);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        

    }

    [Command]
    private async void QuickJoinLobby()
    {
        try
        {
            Lobby quickJoinLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("Lobby Joined " + quickJoinLobby.Name);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
