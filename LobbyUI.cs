using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button createLobbyBtn;
    [SerializeField] Button quickJoinBtn;
    [SerializeField] Button joinLobbyCodeBtn;
    [SerializeField] Button leaveBtn;
    [SerializeField] TMP_InputField joinCodeInput;
    [SerializeField] LobbyCreateUI lobbyCreateUI;
    [SerializeField] Transform lobbyContainer;
    [SerializeField] Transform lobbyTemplate;
    private void Awake()
    {
        createLobbyBtn.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        quickJoinBtn.onClick.AddListener(() =>
        {
            LobbyGame.Instance.QuickJoinLobby();
        });
        leaveBtn.onClick.AddListener(() =>
        {
            LobbyGame.Instance.LeaveLobby();
        });
        joinLobbyCodeBtn.onClick.AddListener(() =>
        {
            LobbyGame.Instance.JoinLobbyCode(joinCodeInput.text);
        });
        LobbyGame.Instance.OnLobbyListChanged += LobbyGame_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
        lobbyTemplate.gameObject.SetActive(false);
    }

    private void LobbyGame_OnLobbyListChanged(object sender, LobbyGame.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach(Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);

        }
        foreach(Lobby lobby in lobbyList)
        {
            Transform lobbyTransfrom = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransfrom.gameObject.SetActive(true);
            lobbyTransfrom.GetComponent<OnLobbyListSingleUI>().SetLobby(lobby);
        }
    }
    private void OnDestroy()
    {
        LobbyGame.Instance.OnLobbyListChanged -= LobbyGame_OnLobbyListChanged;
    }
}
