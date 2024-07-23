using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;

using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class LobbyGame : NetworkBehaviour
{
    public static LobbyGame Instance { get; private set; }
    string playerName;
    const string relayJoinCodeKey = "RelayJoinCode";
    const string playerNameKey = "playerNameKey";
    Lobby joinedLobby;
    float timer;
    private float listLobbiesTimer;

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }
    
    
    // Start is called before the first frame update
    
    async void Start()
    {
        try
        {
            if(UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                InitializationOptions initializationOptions = new InitializationOptions();
                initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());
            }
            
            
        }catch(ServicesInitializationException e)
        {
            Debug.Log(e);
        }
    }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerName = PlayerPrefs.GetString(playerNameKey, "PlayerName" + UnityEngine.Random.Range(0, 1000));

    }
    public string GetPlayerName()
    {
        return playerName;
    }
    private void OnDestroy()
    {
        DeleteLobby();
    }
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;

        PlayerPrefs.SetString(playerNameKey, playerName);
    }
    void HandleHeartbeat()
    {
        if(IsLobbyHost())
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                float maxTimer = 15f;
                timer = maxTimer;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
            
        }
    }
    bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            return allocation;
        }catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        HandlePeriodicListLobbies();
    }
    void HandlePeriodicListLobbies()
    {
        if(joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == "MainMenu") 
        {
            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0)
            {
                float timeMax = 4f;
                listLobbiesTimer = timeMax;
                ListLobbies();
            }

        }
        
    }
    async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
        
    }
    async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;

        }
        

    }
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        
        int maxPlayers = 2;
        try
        {
            if(joinedLobby == null)
            {
                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });
                Allocation allocation = await AllocateRelay();


                string relayJoinCode = await GetRelayJoinCode(allocation);
                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
            {
                {relayJoinCodeKey, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
            }
                });
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
                NetworkManager.Singleton.StartHost();
                NetworkManager.Singleton.SceneManager.LoadScene("lobbyScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            
            
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }
    public async void QuickJoinLobby()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = joinedLobby.Data[relayJoinCodeKey].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));


            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    } 
    public async void JoinLobbyCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayJoinCode = joinedLobby.Data[relayJoinCodeKey].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void JoinLobbyId(string lobbyId)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = joinedLobby.Data[relayJoinCodeKey].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public Lobby GetLobby()
    {
        return joinedLobby;
    }
    public async void DeleteLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
    }
    public async void LeaveLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
        
    }
    public async void KickPlayer(string playerId)
    {
        if(IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
        
    }
    async void ListLobbies()
    {
        try
        {
           QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs {
                lobbyList = queryResponse.Results
            });
            
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
