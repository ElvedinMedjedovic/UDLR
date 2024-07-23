using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : NetworkBehaviour
{
    [SerializeField] Button startHostBtn;
    [SerializeField] Button startClientBtn;
    [SerializeField] Button playAgainBtn;
    // Start is called before the first frame update
    void Start()
    {
        //NetworkManager.Singleton.StartHost();
        startHostBtn.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartHost();
        });
        startClientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
        playAgainBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Level1", LoadSceneMode.Single);
            //SceneManager.LoadScene(0);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
