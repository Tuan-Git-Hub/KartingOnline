using System;
using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMessageEndGame : MonoBehaviour
{
    private RaceData raceData;
    private TextMeshProUGUI textPosition;
    private TextMeshProUGUI textTime;
    private TextMeshProUGUI textWarning;
    private bool isPressed = false;

    private Button buttonReturn;
    private Button buttonReplay;

    private EdgegapRelayManager relayManager;

    private void Awake()
    {
        var textsList = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var text in textsList)
        {
            if (text.name == "TextPosition")
                textPosition = text;
            else if (text.name == "TextTime")
                textTime = text;
            else if (text.name == "TextWarning")
                textWarning = text;
        }

        var buttonsList = GetComponentsInChildren<Button>();
        foreach (var b in buttonsList)
        {
            if (b.name == "ButtonReturn")
                buttonReturn = b;
            else if (b.name == "ButtonReplay")
                buttonReplay = b;
        }

        relayManager = NetworkManager.singleton.GetComponent<EdgegapRelayManager>();
    }

    private void OnEnable()
    {
        buttonReturn.onClick.AddListener(ReturnToMainMene);
        buttonReplay.onClick.AddListener(ReplayGame);
    }

    private void Start()
    {
        raceData = GameManager.Instance._raceData;
        GameManager.Instance.finishRace.AddListener(ShowMessageEndGame);
        textWarning.enabled = false;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        buttonReturn.onClick.RemoveListener(ReturnToMainMene);
        buttonReplay.onClick.RemoveListener(ReplayGame);
    }



    private void ShowMessageEndGame()
    {
        gameObject.SetActive(true);

        textPosition.text = "No " + raceData.currentRank;

        float etime = raceData.elapsedTime;
        int m = (int)(etime / 60) % 60;
        int s = (int)(etime % 60);
        int ms = (int)(etime * 100) % 100;
        textTime.text = $"{m:00}:{s:00}:{ms:00}";
    }

    private async void ReturnToMainMene()
    {
        if (isPressed) return;
        else isPressed = true;

        try
        {
            GameManager.Instance.isStartedGame = false;
            if (NetworkServer.active)
            {
                await relayManager.DeleteSession();
            }
            else if (!NetworkServer.active && NetworkClient.isConnected)
            {
                await relayManager.OutSession();
            }

            if (ChangeSceneManager.Instance.currentScene.name != "MainMenu")
            {
                ChangeSceneManager.Instance.ChangeScene("MainMenu", success =>
                {
                    if (success)
                    {
                        Debug.Log("Return to main menu");
                    }
                });
            }

            isPressed = false;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            isPressed = false;
            return;
        }
    }

    private void ReplayGame()
    {
        if (isPressed) return;
        else isPressed = true;

        if (NetworkServer.active)
        {
            Debug.Log("Replay Game");
            StartCoroutine(ReloadSceneAfterDelay(0.5f));
        }
        else
        {
            StartCoroutine(ShowWarning(2f));
        }

        isPressed = false;
    }

    IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.networkBridge.RpcResetGame();
    }

    IEnumerator ShowWarning(float timeShow)
    {
        textWarning.enabled = true;
        yield return new WaitForSeconds(timeShow);
        textWarning.enabled = false;
    }
}
