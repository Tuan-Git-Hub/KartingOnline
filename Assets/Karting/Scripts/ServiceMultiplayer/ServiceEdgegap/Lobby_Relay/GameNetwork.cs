using System.Collections;
using Mirror;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.Events;

public class GameNetwork : MonoBehaviour
{
    //public NetworkManager networkManager;
    public EdgegapLobbyManager lobbyManager;
    public EdgegapGetDataRelay relay;

    private string myPlayerId = System.Guid.NewGuid().ToString(); // Generate a unique random identifier (ID)
    private string currentLobbyId;

    public UnityEvent<string, string> createdLobby;
    public void OnCreateLobby(string roomName)
    {
        StartCoroutine(lobbyManager.GetURLService(lobbySe =>
        {
            Debug.Log($"URL: {lobbySe.url}");
        }));

        StartCoroutine(lobbyManager.CreateLobby(myPlayerId, roomName, lobby =>
        {
            currentLobbyId = lobby.lobby_id;
            Debug.Log($"Lobby created: {currentLobbyId}");
            createdLobby?.Invoke(lobby.name, lobby.lobby_id);
        }));
    }

    public void OnStartLobby()
    {
        StartCoroutine(lobbyManager.StartLobby(currentLobbyId, lobby =>
        {
            relay.ExtractRelay(lobby);
            NetworkManager.singleton.networkAddress = relay.Ip;
            NetworkManager.singleton.StartHost();
        }));
    }

    public void OnJoinLobby(string lobbyId)
    {
        StartCoroutine(lobbyManager.JoinLobby(lobbyId, myPlayerId, success =>
        {
            if (success)
            {
                Debug.Log("Joined lobby, waiting for relay...");
                StartCoroutine(WaitForRelayReady(lobbyId));
            }
        }));
    }

    private IEnumerator WaitForRelayReady(string lobbyId)
    {
        bool ready = false;
        while (!ready)
        {
            yield return new WaitForSeconds(2);
            yield return lobbyManager.GetLobby(lobbyId, lobby =>
            {
                if (lobby.assignment != null && lobby.assignment.ip != null)
                {
                    relay.ExtractRelay(lobby);
                    ready = true;
                    NetworkManager.singleton.networkAddress = relay.Ip;
                    NetworkManager.singleton.StartClient();
                }
            });
        }
    }
}
