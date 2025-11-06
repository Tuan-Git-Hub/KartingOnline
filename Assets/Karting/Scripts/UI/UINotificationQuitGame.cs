using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UINotificationQuitGame : MonoBehaviour
{
    private Button buttonAgreeToReturnToMainMene;
    private Button buttonRefuseToReturnToMainMene;

    private EdgegapRelayManager relayManager;
    private bool isPressed = false;
    private void Awake()
    {
        var buttonsList = GetComponentsInChildren<Button>();
        foreach (var b in buttonsList)
        {
            if (b.name == "ButtonYes")
                buttonAgreeToReturnToMainMene = b;
            else if (b.name == "ButtonNo")
                buttonRefuseToReturnToMainMene = b;
        }

        relayManager = NetworkManager.singleton.GetComponent<EdgegapRelayManager>();
    }

    private void OnEnable()
    {
        buttonAgreeToReturnToMainMene.onClick.AddListener(ReturnToMainMenu);
        buttonRefuseToReturnToMainMene.onClick.AddListener(ReturnToGame);
    }

    private void OnDisable()
    {
        buttonAgreeToReturnToMainMene.onClick.RemoveListener(ReturnToMainMenu);
        buttonRefuseToReturnToMainMene.onClick.RemoveListener(ReturnToGame);
    }

    private async void ReturnToMainMenu()
    {
        if (isPressed) { return; }
        isPressed = true;

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
                        Debug.Log("Return to main menu1");
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

    private void ReturnToGame()
    {
        if (isPressed) { return; }
        isPressed = true;

        gameObject.SetActive(false);
        Debug.Log("Return to game");
        isPressed = false;
    }

    public void ShowNotificationQuitGame()
    {
        gameObject.SetActive(true);
        Debug.Log("Show notification quit game");
    }
}
