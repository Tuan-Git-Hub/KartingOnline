using TMPro;
using UnityEngine;

public class HUDTime : MonoBehaviour
{
    private RaceData raceData;
    private TextMeshProUGUI textTime;

    private void Awake()
    {
        //raceData = GameManager.Instance._raceData;
        textTime = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        //GameManager.Instance.changeData.AddListener(FixRaceData);
    }
    void Start()
    {
        raceData = GameManager.Instance._raceData;
    }

    void Update()
    {
        if (GameManager.Instance.isRacing == true)
        { 
            float etime = raceData.elapsedTime;
            //int h = (int)(etime / 3600);
            int m = (int)(etime / 60) % 60;
            int s = (int)(etime % 60);
            int ms = (int)(etime * 100) % 100;

            textTime.text = $"TIME {m:00}:{s:00}:{ms:00}";
        }
    }
    private void OnDisable()
    {
        //GameManager.Instance.changeData.RemoveListener(FixRaceData);
    }
    //private void FixRaceData()
    //{
    //    raceData = GameManager.Instance._raceData; // Renew raceData and re-add event
    //}
}
