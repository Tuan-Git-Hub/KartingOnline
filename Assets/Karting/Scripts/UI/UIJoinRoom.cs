using System;
using System.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIJoinRoom : MonoBehaviour
{
    private Button buttonJoin;
    private Button buttonBack;

    private TMP_InputField inputField;
    private string roomID;

    private GameObject uiButtonMainMenu;
    private GameObject uiRoom;

    private EdgegapRelayManager relayManager;
    private bool isPressed = false;

    private void Awake()
    {
        var objs = GetComponentsInChildren<Button>();
        foreach (var obj in objs)
        {
            if (obj.name == "ButtonJoin")
                buttonJoin = obj;
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
        buttonJoin.onClick.AddListener(OnJoinRoom);
        buttonBack.onClick.AddListener(OnBackMenu);
        inputField.onEndEdit.AddListener(OnIDEntered);
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        buttonJoin.onClick.RemoveListener(OnJoinRoom);
        buttonBack.onClick.RemoveListener(OnBackMenu);
        inputField.onEndEdit.RemoveListener(OnIDEntered);
        inputField.text = string.Empty;
    }

    private async void OnJoinRoom()
    {
        if (isPressed) { return; }
        isPressed = true;

        if (string.IsNullOrEmpty(roomID))
            OnIDEntered(inputField.text);
        if (string.IsNullOrEmpty(roomID))
            return;
        try
        {
            //uiRoom.SetActive(true);

            await relayManager.JoinSession(roomID);
            
            gameObject.SetActive(false);
            isPressed = false;

            //uiRoom.GetComponent<UIRoom>().WaitSync(5f);
            uiRoom.GetComponent<UIRoom>().ShowUI();
            Debug.Log("Join room successful");
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

    private void OnIDEntered(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            roomID = string.Empty;
        }
        else
        {
            roomID = input;
        }
    }
}
