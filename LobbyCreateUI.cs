using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button createPublicBtn;
    [SerializeField] Button createPrivateBtn;
    [SerializeField] TMP_InputField lobbyNameInput;
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] GameObject image;
    private void Awake()
    {
        createPublicBtn.onClick.AddListener(() =>
        {
            LobbyGame.Instance.CreateLobby(lobbyNameInput.text, false);
            StartCoroutine(ImageTimer());
            
        });
        createPrivateBtn.onClick.AddListener(() =>
        {
            LobbyGame.Instance.CreateLobby(lobbyNameInput.text, true);
            StartCoroutine(ImageTimer());
        });
        closeBtn.onClick.AddListener(() =>
        {
            Hide();
        });
    }
    private void Start()
    {
        Hide();
        playerNameInput.text = LobbyGame.Instance.GetPlayerName();
        playerNameInput.onValueChanged.AddListener((string newText) =>
        {
            LobbyGame.Instance.SetPlayerName(newText);
        });
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }
    IEnumerator ImageTimer()
    {
        image.SetActive(true);
        yield return new WaitForSeconds(4);
        image.SetActive(false);
    }
}
