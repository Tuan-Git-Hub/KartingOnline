using UnityEngine;

[CreateAssetMenu(fileName = "InitialData", menuName = "Scriptable Objects/InitialData")]
public class InitialData : ScriptableObject
{
    [Header("Track One")]
    [Header("Player")]
    [Tooltip("Total number of players to race")]
    public int numberOfPlayers = 2;

    [Header("Lap")]
    [Tooltip("Total number of laps to race")]
    public int numberOfLaps = 2;
}
