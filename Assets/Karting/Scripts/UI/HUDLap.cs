using TMPro;
using UnityEngine;

public class HUDLap : MonoBehaviour
{
    private RaceData raceData;
    private TextMeshProUGUI textLap;
    private void Awake()
    {
        raceData = GameManager.Instance._raceData;
        textLap = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        GameManager.Instance.changeData.AddListener(FixRaceData);
        raceData.AddLap += FixHUDLap;
    }
    void Start()
    {
        textLap.text = "LAP 1" + "/" + raceData.totalLaps;
    }
 
    void Update()
    {
        
    }

    private void OnDisable()
    {
        GameManager.Instance.changeData.RemoveListener(FixRaceData);
        raceData.AddLap -= FixHUDLap;
    }

    private void FixRaceData()
    {
        if (raceData != null)
        {
            raceData.AddLap -= FixHUDLap; // Remove old envent 
        }
        raceData = GameManager.Instance._raceData; // Renew raceData and re-add event
        raceData.AddLap += FixHUDLap;
    }

    private void FixHUDLap(int currentLap)
    {
        textLap.text = "LAP " + currentLap + "/" + raceData.totalLaps;
    }    
}
