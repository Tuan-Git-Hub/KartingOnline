using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Class singleton
    public static GameManager Instance { get; private set; }

    [SerializeField] private InitialData _initialData;
    public RaceData _raceData;
    public NetworkBridge networkBridge;

    public bool haveCountdownTimeBeforeRace = true;
    public float countdownTime = 10f;
    public bool isRacing = false;
    public bool isCompletedLap = false;
    public bool isStartedGame = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Load Data
        _raceData = new RaceData(_initialData);
        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {

        //Debug.Log($"ScountdownTime: {countdownTime}");
        //Debug.Log($"ShaveCountdownTimeBeforeRace: {haveCountdownTimeBeforeRace}");
        //Debug.Log($"SisCompletedLap: {isCompletedLap}");
        if (!NetworkManager.singleton.isNetworkActive || !isStartedGame)
            return;

        if (isRacing)
        {
            _raceData.elapsedTime += Time.deltaTime;
        }
        else if (!isRacing && haveCountdownTimeBeforeRace)
        {
            if (countdownTime >= 0 && NetworkServer.active)
            {
                countdownTime -= Time.deltaTime;
                networkBridge.RpcBoardcastCountdownTime(countdownTime);
            }
            else if (countdownTime < 0)
            {
                haveCountdownTimeBeforeRace = false;
                isRacing = true;
                isCompletedLap = false;
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public UnityEvent changeData; // Notify change data
    public UnityEvent finishRace;
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "InGameScene")
        {
            ResetDataGameManager();
            RankRacerManager.Instance.ResetDataRankRacerManager();

            if (NetworkServer.active)
            {
                (NetworkManager.singleton as MyNetwork).RespawnAllPlayer();
            }
        }
    }

    public void ResetDataGameManager()
    {
        _raceData = new RaceData(_initialData);
        haveCountdownTimeBeforeRace = true;
        countdownTime = 5f;
        isRacing = false;
        isCompletedLap = false;
        changeData?.Invoke();
        Debug.Log("Reset data");
    }

    public void StartFinishLap()
    {
        if (_raceData.currentLap >= _raceData.totalLaps && isCompletedLap)
        {
            isRacing = false;
            finishRace?.Invoke();
            Debug.Log("Finish Race");
        }
        else if (isCompletedLap && _raceData.currentLap != 0)
        {
            _raceData.FinishLap();
            isCompletedLap = false;
            Debug.Log("Completed a lap");
        }
        else if (_raceData.currentLap == 0 && _raceData.progressKart > 0.95)
        {
            _raceData.currentLap++;
        }
        else if (_raceData.currentLap == 1 && _raceData.progressKart < 0.05)
        {
            _raceData.currentLap--;
        }
    }
}
