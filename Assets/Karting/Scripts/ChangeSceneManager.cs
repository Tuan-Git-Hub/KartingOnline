using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    // Class singleton
    public static ChangeSceneManager Instance { get; private set; }

    public Scene currentScene { get; private set; }

    private bool isLoading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "SceneManager")
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("SceneManager"));
        ChangeScene("MainMenu", success => { if (success) Debug.Log("Scene: MainMenu"); });
    }

    public void ChangeScene(string sceneName, Action<bool> callback)
    {
        if (isLoading) 
        {
            callback?.Invoke(false);
            return; 
        }
        else
            isLoading = true;

        StartCoroutine(LoadSceneRoutine(sceneName));
        callback?.Invoke(true);

        isLoading = false;
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (currentScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return op;

        currentScene = SceneManager.GetSceneByName(sceneName);
    }
}
