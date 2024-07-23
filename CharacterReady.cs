using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterReady : NetworkBehaviour
{
    public static CharacterReady Instance { get; private set; }
    Dictionary<ulong, bool> playerReadyDictionary;
    public string level;
    [SerializeField] TextMeshProUGUI lobbyNameTxt;
    [SerializeField] TextMeshProUGUI lobbyCodeTxt;
    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();

    }
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
            if(allClientsReady)
            {
                LobbyGame.Instance.DeleteLobby();
                NetworkManager.Singleton.SceneManager.LoadScene(level, LoadSceneMode.Single);
            }
        }
    }
    private void Start()
    {
        Lobby lobby = LobbyGame.Instance.GetLobby();

        lobbyNameTxt.text = "Lobby Name: " + lobby.Name;
        lobbyCodeTxt.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
