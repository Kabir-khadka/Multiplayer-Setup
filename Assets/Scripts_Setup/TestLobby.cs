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
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;

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

        playerName = "CodeMonkey" + UnityEngine.Random.Range(1, 10);

        Debug.Log(playerName);


    }


    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollUpdates();
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

    private async void HandleLobbyPollUpdates()
    {
        if(joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;

            if(lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;

                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                joinedLobby = lobby;
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
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "CaptureTheFlag") },
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, "Dust 2") }
                }
            };

            //await method to create a lobby calling createlobbyasync
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            //Storing the instance
            hostLobby = lobby;
            joinedLobby = hostLobby;

            Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.LobbyCode);

            PrintPlayers(hostLobby);
       
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
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                   // new QueryFilter(QueryFilter.FieldOptions.S1, "CaptureTheFlag", QueryFilter.OpOptions.EQ)
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
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value);
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

            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer(),
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;

            Debug.Log("Lobby joined with the code: " + lobbyCode);

            PrintPlayers(joinedLobby);

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        

    }

    //creating a separate 'PLayer' return type method for storing player data so that it can be called anywhere.
    private Player GetPlayer()
    {
        return new Player
        {

            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
        };
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

    [Command]
    private void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }
    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby " + lobby.Name + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value);

        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    [Command]
    private async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) },

                }
            });

            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            //playerName = newPlayerName;

            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newPlayerName) }
                }
            });
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void MigrateLobby()
    {
        try
        {
           hostLobby =  await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions {
                HostId = joinedLobby.Players[1].Id,
            });

            joinedLobby = hostLobby;
            PrintPlayers(hostLobby);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
