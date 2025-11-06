using TMPro;
using UnityEngine;

public class HUDRank : MonoBehaviour
{
    private RaceData raceData;
    private TextMeshProUGUI textRank;
    private void Awake()
    {
        raceData = GameManager.Instance._raceData;
        textRank = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        GameManager.Instance.changeData.AddListener(FixRaceData);
        raceData.ChangedRank += DisplayPosition;
    }

    

    private void OnDisable()
    {
        GameManager.Instance.changeData.RemoveListener(FixRaceData);
        raceData.ChangedRank -= DisplayPosition;
    }

    private void FixRaceData()
    {
        if (raceData != null)
        {
            raceData.ChangedRank -= DisplayPosition; // Remove old event 
        }
        raceData = GameManager.Instance._raceData; // Renew raceData and re-add event
        raceData.ChangedRank += DisplayPosition;
    }

    void DisplayPosition(int rank)
    {
        if (rank == 1)
            textRank.text = "1<size=30%>st</size>";
        else if (rank == 2)
            textRank.text = "2<size=30%>nd</size>";
        else if (rank == 3)
            textRank.text = "3<size=30%>rd</size>";
        else
            textRank.text = $"{rank}<size=30%>th</size>";
    }
}
