using Mirror;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ServiceRelay_Lobby : MonoBehaviour
{
    public NetworkManager networkManager;
    private string nameRoom;
    private string codeRoom;
    private string joinCode;

    public async void CreateRoom(string nRoom)
    {
        // Create Relay allocation
        Allocation alloc = await RelayService.Instance.CreateAllocationAsync(4);

        // join code
        joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

        // Create Lobby
        nameRoom = nRoom;
        Lobby lob = await LobbyService.Instance.CreateLobbyAsync(nRoom, 4, new CreateLobbyOptions
        {
            Data = new System.Collections.Generic.Dictionary<string, DataObject>
            {
                {"JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode)},
            }
        });
        codeRoom = lob.LobbyCode;

        // Setup transport
        var transport = networkManager.transport;
        
    }

}
