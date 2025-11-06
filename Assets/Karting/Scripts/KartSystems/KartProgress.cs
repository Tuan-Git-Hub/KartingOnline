using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class KartProgress : NetworkBehaviour
{
    // Progress of the kart of the race
    public SplineContainer splineContainer;
    private bool checkPoint1 = false;
    private bool checkPoint2 = false;

    [SyncVar] public float progress;

    private void OnEnable()
    {
        GameManager.Instance.finishRace.AddListener(FinishedRace); // Finish race and unregister racer
    }

    void Start()
    {
        if (isServer)
        {
            RankRacerManager.Instance?.RegisterRacer(this); // Registration is to manage the number of racers
        }
    }
    private void OnDisable()
    {
        GameManager.Instance.finishRace.RemoveListener(FinishedRace); 
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        RankRacerManager.Instance?.RegisterRacer(this);
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        RankRacerManager.Instance?.UnregisterRacer(this);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!GameManager.Instance.isRacing)
            return;

        float3 carPos = (float3)transform.position;
        float3 nearest;
        float t;

        SplineUtility.GetNearestPoint(splineContainer.Spline, carPos, out nearest, out t);

        GameManager.Instance._raceData.progressKart = t;
        //float traveled = t * totalLength;
        //float percent = t * 100f;
        //Debug.Log($"Distance: {t*100}%");

        if (Mathf.Abs(t*100f - 50f) < 1)  // Checkpoint 1 at 50%
            checkPoint1 = true;
        else if (Mathf.Abs(t*100f - 99f) < 1 && checkPoint1) // Checkpoint 2 at 99%
            checkPoint2 = true;

        if (checkPoint1 && checkPoint2) // Passing 2 checkpoint is considered completing a lap
        { 
            GameManager.Instance.isCompletedLap = true;
            checkPoint1 = false;
            checkPoint2 = false;
        }
        //Debug.Log($"checkpoint1: {checkPoint1}, checkpoint2: {checkPoint2}");

        float currentProgress = t + GameManager.Instance._raceData.currentLap;
        CmdReportProgress(currentProgress);
    }

    public void FinishedRace() // Finish race and unregister racer
    {
        CmdFinishedRaceAndUnregisterRacer();
    }

    [Command]
    void CmdReportProgress(float p)
    {
        progress = p;
        RankRacerManager.Instance.ReportProgress(this, p);
    }

    [Command]
    void CmdFinishedRaceAndUnregisterRacer()
    {
        RankRacerManager.Instance.numberOfCompletedRacer++;
        RankRacerManager.Instance.UnregisterRacer(this);
    }
}
