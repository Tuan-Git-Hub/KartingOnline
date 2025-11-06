using System;
using UnityEngine;

public class RaceData
{
    public int totalLaps;
    public int currentLap;
    public float elapsedTime;
    public float progressKart;
    public int currentRank;

    public RaceData(InitialData initialData)
    {
        totalLaps = initialData.numberOfLaps;
        currentLap = 0;
        elapsedTime = 0f;
        progressKart = 0f;
        currentRank = 1;
    }

    public event Action<int> AddLap;
    public void FinishLap()
    {
        currentLap++;
        AddLap?.Invoke(currentLap);
    }

    public event Action<int> ChangedRank;
    public void ChangeDataRank(int rank)
    {
        currentRank = rank;
        ChangedRank?.Invoke(currentRank);
    }
}
