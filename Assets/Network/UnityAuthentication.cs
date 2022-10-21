using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class UnityAuthentication : MonoBehaviour
{
    private string _lobbyId;

    private RelayHostData _hostData;
    private RelayJoinData _joinData;

   private async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log(UnityServices.State);

        SetupEvents();

        await SignInAnonymouslyAsync();
    }

    #region
    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.Log(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            Debug.Log($"PlayerdID: {AuthenticationService.Instance.PlayerId}");
        }

        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }

        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    #endregion

    #region Lobby

    public async void FindMatch()
    {
        Debug.Log(message: "Looking for a lobby...");

        try
        {
            //looking for a lobby


            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);

            Debug.Log(message: "Joined Lobby: " + lobby.Id);
            Debug.Log(message: "Lobby Players: " + lobby.Players.Count);

            //Retrieve the relay code previously set in create match
            string joinCode = lobby.Data["joinCode"].Value;

            Debug.Log(message: "Received Code: " + joinCode);

            JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

            //create object 
            _joinData = new RelayJoinData
            {
                Key = allocation.Key,
                Port = (ushort) allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };

            //set transport data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                _joinData.IPv4Address,
                _joinData.Port,
                _joinData.AllocationIDBytes,
                _joinData.Key,
                connectionDataBytes: _joinData.ConnectionData,
                _joinData.HostConnectionData);

            //start client
            NetworkManager.Singleton.StartClient();
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(message: "Cannot find a lobby: " + e);
            CreateMatch();
        }
    }

    private async void CreateMatch()
    {
        Debug.Log(message: "Creating a new lobby...");

        //external Connections
        int maxConnections = 1;

        try
        {
            //Create Relay object
            Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
            _hostData = new RelayHostData
            {
                Key = allocation.Key,
                Port = (ushort) allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };

            //Retrieve JoinCode
            _hostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

            string lobbyName = "game_lobby";
            int maxPlayers = 2;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;

            //Put Join Code in the lobby data
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _hostData.JoinCode)
                },
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            _lobbyId = lobby.Id;

            Debug.Log(message: "Created lobby" + lobby.Id);

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            //Set transport data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                _hostData.IPv4Address,
                _hostData.Port,
                _hostData.AllocationIDBytes,
                _hostData.Key,
                _hostData.ConnectionData);

            //Start host
            NetworkManager.Singleton.StartHost();
        }

        catch (LobbyServiceException e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log(message: "Lobby Heartbit");
            yield return delay;
        }
    }

    private void OnDestroy()
    {
        Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
    }

    #endregion

    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }

    public struct RelayJoinData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }
}
