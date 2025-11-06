using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Playables;

public class RankRacerManager : MonoBehaviour
{
    public static RankRacerManager Instance { get; private set; }

    private readonly HashSet<KartProgress> registeredRacer = new();
    private readonly Dictionary<KartProgress, float> latestProgress = new();

    public float tickInterval = 0.1f; // Time to receive and update data
    private float timer = 0f;

    public int numberOfCompletedRacer = 0; // Number of completed racer

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!NetworkManager.singleton.isNetworkActive || !NetworkServer.active)
            return;

        if (registeredRacer.Count == 0) return; // if no racer
        timer += Time.deltaTime;
        if (timer < tickInterval) return; // Use timer, tickInterval to manage sync, to have time to recive data progress
        if (latestProgress.Count < registeredRacer.Count) return; // Not enough data received, waiting for next tickInterval
        
        timer = 0f;

        var sorted = latestProgress.OrderByDescending(kvp => kvp.Value); // sort the progress of karts in descending order
        var arrayNetId = sorted.Select(kvp => kvp.Key.netId).ToArray(); // sorted netId array

        GameManager.Instance.networkBridge.RpcRankRacer(arrayNetId, numberOfCompletedRacer);
        latestProgress.Clear();
    }

    public void RegisterRacer(KartProgress kartProgress)
    {
        if (!NetworkServer.active) return;   
        registeredRacer.Add(kartProgress);
    }
    public void UnregisterRacer(KartProgress kartProgress)
    {
        if (!NetworkServer.active) return;
        registeredRacer.Remove(kartProgress);
        if (latestProgress.ContainsKey(kartProgress))
        {
            latestProgress.Remove(kartProgress);
        }
    }

    public void ReportProgress(KartProgress k, float p)
    {
        if (!NetworkServer.active) return;
        latestProgress[k] = p;
    }

    public void ResetDataRankRacerManager()
    {
        registeredRacer.Clear();
        latestProgress.Clear();
        timer = 0f;
        numberOfCompletedRacer = 0;
    }

}
