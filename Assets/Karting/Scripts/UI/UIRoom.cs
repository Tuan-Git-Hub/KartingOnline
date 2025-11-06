using System;
using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoom : NetworkBehaviour
{
    [SyncVar] public string roomName;
    [SyncVar] public string roomID;

    private TextMeshProUGUI textRoomName;
    private TextMeshProUGUI textRoomID;

    private TextMeshProUGUI[] textPlayerSlot;

    private Button buttonStart;
    private Button buttonBack;

    private GameObject uiButtonMainMenu;

    private EdgegapRelayManager relayManager;
    private bool isPressed = false;

    private void Awake()
    {
        var objs = GetComponentsInChildren<Button>();
        foreach (var obj in objs)
        {
            if (obj.name == "ButtonStart")
                buttonStart = obj;
            else if (obj.name == "ButtonBack")
                buttonBack = obj;
        }

        var ob1 = transform.Find("TextName");
        textRoomName = ob1.GetComponent<TextMeshProUGUI>();
        var ob2 = transform.Find("TextID");
        textRoomID = ob2.GetComponent<TextMeshProUGUI>();

        textPlayerSlot = transform.Find("PlayerSlot").GetComponentsInChildren<TextMeshProUGUI>();

        uiButtonMainMenu = transform.parent.Find("UIButton").gameObject;
        relayManager = NetworkManager.singleton.GetComponent<EdgegapRelayManager>();
    }
    private void OnEnable()
    {
        buttonStart.onClick.AddListener(OnStartRace);
        buttonBack.onClick.AddListener(OnBackMenu);
    }
    private void Start()
    {
        //HideUI();
    }

    private void Update()
    {
        if (NetworkClient.isConnected && !isServer)
            ChangeNameAndIdRoom(roomName, roomID);

        if (isServer)
        {
            var qu = NetworkServer.connections.Count;
            RpcShowSlotPlayer(qu);
        }
    }
    private void OnDisable()
    {
        buttonStart.onClick.RemoveListener(OnStartRace);
        buttonBack.onClick.RemoveListener(OnBackMenu);
    }
    private void OnStartRace()
    {
        if (isPressed) { return; }
        isPressed = true;

        if (isServer)
        {
            RpcStartRace();
            isPressed = false;
            Debug.Log("OnStartRace");
        }
        else
        {
            isPressed = false;
            return;
        }
    }

    private async void OnBackMenu()
    {
        if (isPressed) { return; }
        isPressed = true;
        try
        {
            if (NetworkManager.singleton.isNetworkActive && isServer)
            {
                await relayManager.DeleteSession();
            }
            else if (NetworkManager.singleton.isNetworkActive && isClient)
            {
                await relayManager.OutSession();
            }

            uiButtonMainMenu.SetActive(true);
            HideUI();

            isPressed = false;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            isPressed = false;
            return;
        }
    }

    private void ChangeNameAndIdRoom(string rName, string rID)
    {
        if (isServer)
        {
            roomName = rName;
            roomID = rID;
        }
        textRoomName.text = "Name: " + roomName;
        textRoomID.text = "ID: " + roomID;
    }

    private void ChangeSlotPlayer(int sl)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < sl)
            {
                textPlayerSlot[i].text = $"Player{i + 1}";
            }
            else
                textPlayerSlot[i].text = "Empty";
        }
    }

    public void WaitSync(float tm)
    {
        StartCoroutine(WaitSyncData(tm));
    }
    // Wait to sync data, connect transport
    private IEnumerator WaitSyncData(float ti)
    {
        isPressed = true;
        yield return new WaitForSeconds(1);
        ChangeNameAndIdRoom(relayManager.roomName, relayManager.roomID);
        yield return new WaitForSeconds(ti);
        isPressed = false;
        
    }

    public void HideUI()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        Debug.Log("HideUIRoom");
    }
    public void ShowUI()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;

    }

    [ClientRpc]
    public void RpcStartRace()
    {
        ChangeSceneManager.Instance.ChangeScene("InGameScene", success =>
        {
            if (success)
            {
                GameManager.Instance.isStartedGame = true;
                Debug.Log("Start Game");
            }
            else
            {
                Debug.Log("Error Start Game");
            }
        });
    }

    [ClientRpc]
    public void RpcShowSlotPlayer(int x)
    {
        ChangeSlotPlayer(x);
    }
}
