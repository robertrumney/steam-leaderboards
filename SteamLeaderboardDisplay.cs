using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLeaderboardDisplay : MonoBehaviour
{
    public bool getScores = true;
    
    public Text info;
    public Text[] scores;

    [HideInInspector]
    public SteamLeaderboardEntries_t m_SteamLeaderboardEntries;
    private static readonly CallResult<LeaderboardScoresDownloaded_t> m_scoresDownloadedResult = new CallResult<LeaderboardScoresDownloaded_t>();

    private void OnEnable()
    {
        if (info)
            info.text = "LOADING SCORES...";
        
        if (getScores)
            GetScores();
    }

    public static void GetScores()
    {
        if (!SteamLeaderboardManager.s_initialized)
        {
            Debug.Log("Can't fetch leaderboard because it isn't loaded yet");
        }
        else
        {
            // Download leaderboard entries
            SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(SteamLeaderboardManager.s_currentLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 100); // Maximum of 100 entries
            m_scoresDownloadedResult.Set(handle, OnLeaderboardScoresDownloaded);
        }
    }

    private static void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        SteamLeaderboardDisplay instance = FindObjectOfType<SteamLeaderboardDisplay>();
        instance.ProcessDownloadedScores(pCallback);
    }

    private void ProcessDownloadedScores(LeaderboardScoresDownloaded_t pCallback)
    {
        info.gameObject.SetActive(false);

        // Reset text fields
        foreach (Text score in scores)
        {
            score.text = "";
            score.gameObject.SetActive(true);
        }

        int numEntries = pCallback.m_cEntryCount;
        m_SteamLeaderboardEntries = pCallback.m_hSteamLeaderboardEntries;

        int rank = 1;

        // Process each leaderboard entry
        for (int index = 0; index < numEntries; index++)
        {
            LeaderboardEntry_t leaderboardEntry;
            SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, index, out leaderboardEntry, null, 0);
            string username = SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser);

            // Handle rare unknown username issue and reset leaderboard
            if (username.ToUpper() == "[UNKNOWN]")
            {
                if (info)
                {
                    // Display loading message
                    info.gameObject.SetActive(true);
                    info.text = "LOADING SCORES...";
                    
                    foreach (Text score in scores)
                    {
                        score.gameObject.SetActive(false);
                    }
                }

                SteamLeaderboardManager.Init();
                return;
            }

            // Display leaderboard entry in the appropriate UI element based on rank - here we use 4 columns to fit up to 100 scores with 33 scores in each column, and a single score in column one, adjust to your needs.
            if (rank == 1)
            {
                scores[0].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else if (rank > 1 && rank < 35)
            {
                scores[1].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else if (rank > 34 && rank < 68)
            {
                scores[2].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else
            {
                scores[3].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }

            rank++;
        }

        // Update the "info" text field with additional information
        info.text += "\n\nPRESS ANY KEY TO RETURN";
    }
}
