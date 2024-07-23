using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPass : NetworkBehaviour
{
    public void LoadNextLevel()
    {
        int nextSceneName = SceneManager.GetActiveScene().buildIndex;
        nextSceneName++;
        SceneManager.LoadSceneAsync(nextSceneName);
        Scene loadThis = SceneManager.GetSceneByBuildIndex(nextSceneName);
        NetworkManager.Singleton.SceneManager.LoadScene(loadThis.name, LoadSceneMode.Single);
        Debug.Log(loadThis);
    }
}
