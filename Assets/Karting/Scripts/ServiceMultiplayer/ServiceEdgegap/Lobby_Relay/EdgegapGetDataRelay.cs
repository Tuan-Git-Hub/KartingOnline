using Edgegap;
using UnityEngine;

public class EdgegapGetDataRelay : MonoBehaviour
{
    public string Ip {  get; private set; }
    public int Port { get; private set; }
    public string Protocol { get; private set; }

    public void ExtractRelay(EdgegapLobbyManager.Lobby lobby)
    {
        if (lobby.assignment == null)
        {
            Debug.LogError("Lobby has no assignment (relay not started yet)");
            return; 
        }

        Ip = lobby.assignment.ip;
        Port = lobby.assignment.ports[0].port;
        Protocol = lobby.assignment.ports[0].protocol;

        Debug.Log($"Relay ready at {Ip}:{Port} ({Protocol})");
    }
}
