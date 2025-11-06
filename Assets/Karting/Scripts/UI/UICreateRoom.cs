using System;
using System.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICreateRoom : MonoBehaviour
{
    private Button buttonConfirm;
    private Button buttonBack;

    private TMP_InputField inputField;
    private string roomName;

    private GameObject uiButtonMainMenu;
    private GameObject uiRoom;

    private EdgegapRelayManager relayManager;
    private bool isPressed = false;
    private void Awake()
    {
        var objs = GetComponentsInChildren<Button>();
        foreach (var obj in objs)
        {
            if (obj.name == "ButtonConfirm")
                buttonConfirm = obj;
            else if (obj.name == "ButtonBack")
                buttonBack = obj;
        }

        inputField = GetComponentInChildren<TMP_InputField>();

        uiButtonMainMenu = transform.parent.Find("UIButton").gameObject;
        uiRoom = transform.parent.Find("UIRoom").gameObject;
        relayManager = NetworkManager.singleton.GetComponent<EdgegapRelayManager>();
    }
    private void OnEnable()
    {
        buttonConfirm.onClick.AddListener(OnConfirmCreateRoom);
        buttonBack.onClick.AddListener(OnBackMenu);
        inputField.onEndEdit.AddListener(OnNameEntered);
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        buttonConfirm.onClick.RemoveListener(OnConfirmCreateRoom);
        buttonBack.onClick.RemoveListener(OnBackMenu);
        inputField.onEndEdit.RemoveListener(OnNameEntered);
        inputField.text = string.Empty;
    }

    private async void OnConfirmCreateRoom()
    {
        if (isPressed) { return; }
        isPressed = true;
        if (string.IsNullOrEmpty(roomName))
            OnNameEntered(inputField.text);

        try
        {
            //uiRoom.SetActive(true);
            
            await relayManager.CreateSession(roomName);

            gameObject.SetActive(false);
            isPressed = false;

            uiRoom.GetComponent<UIRoom>().WaitSync(3f);
            uiRoom.GetComponent<UIRoom>().ShowUI();
            Debug.Log("Create room successful");   
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            isPressed = false;
            return;
        }
    }

    private void OnBackMenu()
    {
        if (isPressed) { return; }
        isPressed = true;

        uiButtonMainMenu.SetActive(true);
        gameObject.SetActive(false);

        isPressed = false;
    }

    private void OnNameEntered(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            roomName = "MyRoom";
        }
        else
        {
            roomName = input;
        }
    }    
}
