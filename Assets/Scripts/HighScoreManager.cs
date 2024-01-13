using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    private const string PlayerPrefsKey = "HighScores";

    // Function to save the high scores
    public static void SaveHighScores(List<HighScoreEntry> highScores)
    {
        // Convert the list to a JSON string
        string json = JsonUtility.ToJson(new HighScoreList(highScores));

        PlayerPrefs.DeleteAll();

        // Save the JSON string to PlayerPrefs
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    // Function to load the high scores
    public static List<HighScoreEntry> LoadHighScores()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            // Retrieve the JSON string from PlayerPrefs
            string json = PlayerPrefs.GetString(PlayerPrefsKey);

            // If the string is empty, return an empty list
            if (string.IsNullOrEmpty(json))
            {
                return new List<HighScoreEntry>();
            }

            // Parse the JSON string and return the list of high scores
            HighScoreList highScoreList = JsonUtility.FromJson<HighScoreList>(json);
            return highScoreList.highScores;
        }
        else
        {
            return new List<HighScoreEntry>();
        }
    }
}

[System.Serializable]
public class HighScoreEntry
{
    public string playerName;
    public int score;
    public string date;
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScoreEntry> highScores;

    public HighScoreList(List<HighScoreEntry> scores)
    {
        highScores = scores;
    }
}
