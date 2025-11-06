using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

// Call API to create, join lobby and get data from relay 
[Serializable]
public class EdgegapLobbyManager : MonoBehaviour
{
    private const string BASE_URL = "https://api.edgegap.com/v1";
    private const string API_KEY = "token cb6723b3-fa8e-4edc-a342-b950be449882";
    private LobbyService lobby_Service;
    //private const string SERVICE_NAME = "KartingOn";
    //private const string API_SERVICE = "token f07e138a-2606-4a1d-a30e-4d15758a15d6";

    [Serializable]
    public class Player
    {
        public string id;
        public bool is_host;
        public long authorization_token;
    }

    [Serializable]
    public class Port
    {
        public string name;
        public int port;
        public string protocol;
    }

    [Serializable]
    public class Assignment
    {
        public string ip;
        public string host;
        public long authorization_token;
        public Port[] ports;
    }

    [Serializable]
    public class Lobby
    {
        public string lobby_id;
        public string name;
        public bool is_joinable;
        public bool is_started;
        public int capacity;
        public int player_count;
        public Assignment assignment;
        public Player[] players;
    }

    [Serializable]
    public class LobbyService
    {
        public string name;
        public string url;
        public string status;
    }

    public IEnumerator GetURLService(Action<LobbyService> callback)
    {
        var json = @"{ ""name"": ""tuanlobby"" }";
        UnityWebRequest req = new UnityWebRequest($"{BASE_URL}/lobbies:deploy", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Authorization", API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            lobby_Service = JsonUtility.FromJson<LobbyService>(req.downloadHandler.text);
            callback?.Invoke(lobby_Service);
        }
        else
        {
            Debug.LogError("Deloy lobby service failed: " + req.error + "\n" + req.downloadHandler.text);
        }
    }


    public IEnumerator CreateLobby(string playerId, string roomName, Action<Lobby> callback)
    {
        var json = @"{
            ""capacity"": 4,
            ""is_joinable"": true,
            ""name"": """ + roomName + @""",
            ""player"": { ""id"": """ + playerId + @""" }
        }";

        UnityWebRequest req = new UnityWebRequest($"{lobby_Service.url}/lobbies", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        //req.SetRequestHeader("Authorization", API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var lobby = JsonUtility.FromJson<Lobby>(req.downloadHandler.text);
            callback?.Invoke(lobby);
        }
        else
        {
            Debug.LogError("Create lobby failed: " + req.error + "\n" +  req.downloadHandler.text);
        }
    }    

    public IEnumerator JoinLobby(string lobbyId, string playerId, Action<bool> callback)
    {
        var json = @"{
            ""lobby_id"": """ + lobbyId + @""",
            ""player"": { ""id"": """ + playerId + @""" }
        }";

        UnityWebRequest req = new UnityWebRequest($"{BASE_URL}/lobbies:join", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Authorization", API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        callback?.Invoke(req.result == UnityWebRequest.Result.Success);
    }   
    
    public IEnumerator StartLobby(string lobbyId, Action<Lobby> callback)
    {
        var json = @"{ ""lobby_id"": """ + lobbyId + @""" }";

        UnityWebRequest req = new UnityWebRequest($"{BASE_URL}/lobbies:start", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Authorization", API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");
        
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var lobby = JsonUtility.FromJson<Lobby>(req.downloadHandler.text);
            callback?.Invoke(lobby);
        }
        else
        {
            Debug.LogError("Start lobby failed: " + req.error + "\n" + req.downloadHandler.text);
        }
    }

    public IEnumerator GetLobby(string lobbyId, Action<Lobby> callback)
    {
        UnityWebRequest req = new UnityWebRequest($"{BASE_URL}/lobbiees/{lobbyId}", "GET");
        req.SetRequestHeader("Authorization", API_KEY);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var lobby = JsonUtility.FromJson<Lobby>(req.downloadHandler.text);
            callback?.Invoke(lobby);
        }
        else
        {
            Debug.LogError("Get lobby failed: " + req.error + "\n" + req.downloadHandler.text);
        }

    }
}
