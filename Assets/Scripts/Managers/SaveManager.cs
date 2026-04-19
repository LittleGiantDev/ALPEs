using UnityEngine;

public static class SaveManager
{
    private const string HighScore = "HighScore";

    public static void SaveRecord(int score)
    {
        int currentBest = GetRecord();
        if (score > currentBest)
        {
            PlayerPrefs.SetInt(HighScore, score);
            PlayerPrefs.Save();
        }
    }

    public static int GetRecord()
    {
        return PlayerPrefs.GetInt(HighScore, 0);
    }
}