using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;


public class ScoreManager : MonoBehaviourPunCallbacks
{
    public bool gameOn = true;

    private TMP_Text scoreBoard;
    private GameObject gameEndPanal;
    private TMP_Text gameEndText;

    static string nameTemp;

    private string scoresString = ""; // "name1,\t\t12 \n name2,\t\t19";
    private float timer = 120f;

    private GameObject gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManagerObject");
        scoreBoard = gameManager.GetComponent<GameManager>().scoreboard;
        scoresString = "";
        gameEndText = gameManager.GetComponent<GameManager>().gameEndText;
        gameEndPanal = gameManager.GetComponent<GameManager>().gameEndPanal;
        if (photonView.IsMine)
        {
            gameEndPanal.SetActive(false);
            //gameEndText = GameObject.Find("GameEndText(TMP)").GetComponent<TMP_Text>();  
        }
    }
    void Update()
    {
        if (timer > 0f)
        {
            // Update the timer countdown
            timer -= Time.deltaTime;

            // Check if the timer has reached zero
            if (timer <= 0f)
            {
                TimerExpired();
            }
        }
    }

    void TimerExpired()
    {
        gameOn = false;
        Debug.Log("Timer expired!");

        // Example string containing player names and scores
        string playerScoresString = scoreBoard.text;

        // Parse the string and find the high score
        string[] playerScoresLines = playerScoresString.Split('\n');
        Dictionary<string, int> playerScores = new Dictionary<string, int>();

        foreach (string line in playerScoresLines)
        {
            string[] parts = line.Trim().Split("\t\t");
            if (parts.Length == 2)
            {
                string playerName = parts[0].Trim();
                int score;

                if (int.TryParse(parts[1].Trim(), out score))
                {
                    playerScores[playerName] = score;
                }
                else
                {
                    Debug.LogError($"Invalid score format for player {playerName}");
                }
            }
            else
            {
                Debug.LogError($"Invalid line format: {line}");
            }
        }
        
        // Find the player with the highest score
        KeyValuePair<string, int> highScorePlayer = FindHighScorePlayer(playerScores);
        Debug.Log($"Player with the highest score: {highScorePlayer.Key}, Score: {highScorePlayer.Value}");

        gameEndPanal.SetActive(true);
        gameEndText.text = $"Player {highScorePlayer.Key} won \n Score: {highScorePlayer.Value}";
        // Print the result
        
    }

    KeyValuePair<string, int> FindHighScorePlayer(Dictionary<string, int> playerScores)
    {
        KeyValuePair<string, int> highScorePlayer = new KeyValuePair<string, int>();

        foreach (KeyValuePair<string, int> kvp in playerScores)
        {
            if (kvp.Value > highScorePlayer.Value)
            {
                highScorePlayer = kvp;
            }
        }

        return highScorePlayer;
    }


    public void GetDetailsToUpdateScore(string playerName, int newScore)
    {
        GetPlayerNames(playerName, newScore);
        //this.GetComponent<PhotonView>().RPC("GetPlayerNames", RpcTarget.AllBuffered, playerName, newScore);
    }

    private void GetPlayerNames(string playerName, int newScore)
    {
        scoresString = scoreBoard.text;
        if (PhotonNetwork.IsConnected) // Make sure to check if you are connected to the Photon network
        {
            // Iterate through the list of players in the room
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (!scoresString.Contains(player.NickName))
                {
                    scoresString += player.NickName + ",\t\t" + 0 + "\n";
                }
            }
            scoresString.Remove(scoresString.Length - 2);
        }

        UpdateScore(playerName, newScore);
    }



    // Update score by name
    public void UpdateScore(string playerName, int newScore)
    {
        // Parse the existing scores string into a dictionary
        Dictionary<string, int> scores = ParseScores(scoresString);
        //playerName = playerName.Replace(",", "");
        string playerN = playerName;
        // Update the score for the specified player
        if (scores.ContainsKey(playerN))
        {
            scores[playerN] += newScore;
        }
        else
        {
            // Add a new entry if the player name doesn't exist
            scores.Add(playerName, newScore);
        }

        // Reconstruct the scores string
        scoresString = BuildScoresString(scores);
        //print("result "+scoresString);
        string temp = scoresString.Replace(",", "");
        scoreBoard.text = " " + temp;
        scoresString = " " + temp;

        //this.photonView.RPC("SendScoreboard", RpcTarget.All, scoreBoard.text);
        this.GetComponent<PhotonView>().RPC("SendScoreboard", RpcTarget.AllBuffered, " " + scoreBoard.text, timer);
    }

    // Parse scores string into a dictionary
    private static Dictionary<string, int> ParseScores(string scoresString)
    {
        Dictionary<string, int> scores = new Dictionary<string, int>();

        // Split the input string by newline and then by tab
        string[] scoreEntries = scoresString.Split('\n');
        foreach (string entry in scoreEntries)
        {
            string[] parts = entry.Trim().Split("\t\t");

            if (parts.Length == 2)
            {
                string playerName = parts[0].Trim();
                playerName = playerName.Replace(",", "");
                int score;

                if (int.TryParse(parts[1].Trim(), out score))
                {
                    scores[playerName] = score;
                }
            }
        }
        return scores;
    }

    // Reconstruct scores string from dictionary
    private static string BuildScoresString(Dictionary<string, int> scores)
    {
        // Join the dictionary entries into a string with tabs
        return string.Join(" \n ", scores.Select(kv => $"{kv.Key},\t\t{kv.Value}"));
        //return string.Join(" \n ", scores.Select(kv => $"{kv.Key},\t\t{kv.Value}"));
    }

    [PunRPC]
    private void SendScoreboard(string scores, float timefor)
    {
        print("Timer RPC  " + timefor);
        scoreBoard.text = scores;
        scoresString = scores;
        timer = timefor;
    }
}
