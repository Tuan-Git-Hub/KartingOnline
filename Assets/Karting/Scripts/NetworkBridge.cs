using UnityEngine;
using Mirror;

public class NetworkBridge : NetworkBehaviour
{
    private void Awake()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.networkBridge = this;
    }

    [ClientRpc]
    public void RpcBoardcastCountdownTime(float countdown)
    {
        GameManager.Instance.countdownTime = countdown;
    }

    [ClientRpc]
    public void RpcRankRacer(uint[] nID, int finishedracer)
    {
        for (int i = 0; i < nID.Length; i++)
        {
            if (NetworkClient.localPlayer.netId == nID[i])
            {
                GameManager.Instance._raceData.ChangeDataRank(i + 1 + finishedracer);
            }
        }
    }

    [ClientRpc]
    public void RpcResetGame()
    {
        ChangeSceneManager.Instance.ChangeScene("InGameScene", success =>
        {
            if (success)
            {
                GameManager.Instance.isStartedGame = true;
                Debug.Log("Reset Game");
            }
            else
            {
                Debug.Log("Error Reset Game");
            }
        });
    }
}
