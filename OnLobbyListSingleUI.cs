using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OnLobbyListSingleUI : MonoBehaviour
{
    Lobby lobby;
    [SerializeField] TextMeshProUGUI lobbyText;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            LobbyGame.Instance.JoinLobbyId(lobby.Id);
        });
    }
    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyText.text = lobby.Name;
    }
}
