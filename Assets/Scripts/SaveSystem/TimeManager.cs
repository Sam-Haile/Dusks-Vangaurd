using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    private TimeSpan totalTimePlayed;
    private DateTime lastSessionStart;


    void Start()
    {
        LoadTotalTimePlayed();
        StartSession();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            EndSession();
        else
            StartSession();
    }

    void OnApplicationQuit()
    {
        EndSession();
    }

    private void StartSession()
    {
        lastSessionStart = DateTime.Now;
    }

    private void EndSession()
    {
        TimeSpan sessionLength = DateTime.Now - lastSessionStart;
        totalTimePlayed += sessionLength;
        SaveTotalTimePlayed();
    }

    private void SaveTotalTimePlayed()
    {
        // Save totalTimePlayed to PlayerPrefs or a file
        PlayerPrefs.SetString("TotalTimePlayed", totalTimePlayed.ToString());
    }

    private void LoadTotalTimePlayed()
    {
        // Load totalTimePlayed from PlayerPrefs or a file
        string savedTime = PlayerPrefs.GetString("TotalTimePlayed", "00:00:00");
        totalTimePlayed = TimeSpan.Parse(savedTime);
    }

    public TimeSpan GetTotalTimePlayed()
    {
        return totalTimePlayed;
    }
}
