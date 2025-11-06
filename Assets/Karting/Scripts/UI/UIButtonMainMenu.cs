using UnityEngine;
using UnityEngine.UI;

public class UIButtonMainMenu : MonoBehaviour
{
    private Button buttonCreateRoom;
    private Button buttonJoinRoom;
    private Button buttonExitGame;

    private GameObject uiCreateRoom;
    private GameObject uiJoinRoom;
    private void Awake()
    {
        var objs = GetComponentsInChildren<Button>();
        foreach (var obj in objs)
        {
            if (obj.name == "ButtonCreateRoom")
                buttonCreateRoom = obj;
            else if (obj.name == "ButtonJoinRoom")
                buttonJoinRoom = obj;
            else if (obj.name == "ButtonExit")
                buttonExitGame = obj;
        }

        uiCreateRoom = transform.parent.Find("UICreateRoom").gameObject;
        uiJoinRoom = transform.parent.Find("UIJoinRoom").gameObject;
    }

    private void OnEnable()
    {
        buttonCreateRoom.onClick.AddListener(OnCreateRoom);
        buttonJoinRoom.onClick.AddListener(OnJoinRoom);
        buttonExitGame.onClick.AddListener(OnExitGame);
    }
    private void Start()
    {
        gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        buttonCreateRoom.onClick.RemoveListener(OnCreateRoom);
        buttonJoinRoom.onClick.RemoveListener(OnJoinRoom);
        buttonExitGame.onClick.RemoveListener(OnExitGame);
    }

    private void OnCreateRoom()
    {
        uiCreateRoom.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnJoinRoom()
    {
        uiJoinRoom.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnExitGame()
    {
        Application.Quit();
    }    
}
