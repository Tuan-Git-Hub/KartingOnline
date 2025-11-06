using System.Collections;
using Mirror;
using UnityEngine;

public class MyNetwork : NetworkManager
{
    //public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    //{
    //    base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    //    GameManager.Instance.ResetDataGameManager();
    //    RankRacerManager.Instance.ResetDataRankRacerManager();
    //}

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        if (!conn.identity)
        {
            GameObject playerSpawnObject;
            if (ChangeSceneManager.Instance.currentScene.name == "InGameScene")
            {
                playerSpawnObject = spawnPrefabs[1];
            }
            else
            {
                playerSpawnObject = spawnPrefabs[0];
            }
            //Vector3 spawnPos = Vector3.zero;
            var playerObj = Instantiate(playerSpawnObject, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, playerObj);
        }
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        GameManager.Instance.isStartedGame = false;
        StartCoroutine(WaitTimeChangeScene(2));
    }
    private IEnumerator WaitTimeChangeScene(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (ChangeSceneManager.Instance.currentScene.name != "MainMenu")
        {
            ChangeSceneManager.Instance.ChangeScene("MainMenu", success =>
            {
                if (success)
                {
                    Debug.Log("Return to main menu2");
                }
            });
        }
    }    

    [Server]
    public void RespawnAllPlayer()
    {
        var startPo = NetworkManager.startPositions;
        int index = 0;
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn == null || conn.identity == null)
                continue;

            var po = Vector3.zero;
            var ro = Quaternion.identity;
            if (startPo.Count > 0)
            {
                po = startPo[index % startPo.Count].position;
                ro = startPo[index % startPo.Count].rotation;
                index++;
            }

            var player = Instantiate(spawnPrefabs[1], po, ro);
            NetworkServer.ReplacePlayerForConnection(conn, player, ReplacePlayerOptions.Destroy);
            //Debug.Log("startPo: " + startPo.Count + startPo[0].position);

            player.SetActive(true);
        }
    }


}
