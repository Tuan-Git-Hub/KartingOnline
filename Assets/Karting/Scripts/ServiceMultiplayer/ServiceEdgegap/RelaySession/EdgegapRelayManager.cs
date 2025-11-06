using System.Collections;
using System.Threading.Tasks;
using Edgegap;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EdgegapRelayManager : MonoBehaviour
{
    [SerializeField] private EdgegapKcpTransport _edgegapKcpTransport;
    public string roomName { get; private set; }
    public string roomID { get; private set; }
    

    private void Awake()
    {
        _edgegapKcpTransport = NetworkManager.singleton.GetComponent<EdgegapKcpTransport>();
    }

    public async Task CreateSession(string nameR)
    {
        ApiResponse data = await EdgegapRelayAPI.CreateSessionAsync();

        //Convert uint? to uint
        uint sessionAuthorizationToken = data.authorization_token ?? 0;
        uint userAuthorizationToken = data.session_users?[0].authorization_token ?? 0;

        _edgegapKcpTransport.relayAddress = data.relay.ip;
        _edgegapKcpTransport.relayGameServerPort = data.relay.ports.server.port;
        _edgegapKcpTransport.relayGameClientPort = data.relay.ports.client.port;
        _edgegapKcpTransport.sessionId = sessionAuthorizationToken;
        _edgegapKcpTransport.userId = userAuthorizationToken;

        NetworkManager.singleton.StartHost();
        
        roomName = nameR;
        roomID = data.session_id;
    }
    
    public async Task JoinSession(string sessionId)
    {
        ApiResponseJoinSession data = await EdgegapRelayAPI.JoinSessionAsync(sessionId);

        //Convert uint? to uint
        uint sessionAuthorizationToken = data.authorization_token ?? 0;
        uint userAuthorizationToken = data.session_user.authorization_token ?? 0;

        _edgegapKcpTransport.relayAddress = data.relay.ip;
        _edgegapKcpTransport.relayGameServerPort = data.relay.ports.server.port;
        _edgegapKcpTransport.relayGameClientPort = data.relay.ports.client.port;
        _edgegapKcpTransport.sessionId = sessionAuthorizationToken;
        _edgegapKcpTransport.userId = userAuthorizationToken;

        NetworkManager.singleton.StartClient();

        roomID = sessionId;
    }

    public async Task DeleteSession()
    {
        await EdgegapRelayAPI.DeleteSessionAsync(roomID);

        NetworkManager.singleton.StopHost();
    }

    public async Task OutSession()
    {
        var userAuthorizationToken = _edgegapKcpTransport.userId;

        await EdgegapRelayAPI.OutSessionAsync(roomID, userAuthorizationToken);

        NetworkManager.singleton.StopClient();
    }
}
