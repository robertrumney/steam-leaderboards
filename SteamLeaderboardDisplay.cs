using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLeaderboardDisplay : MonoBehaviour
{
    public SteamLeaderboardEntries_t m_SteamLeaderboardEntries;
    public bool getScores;
    public Text scores;
    public Text[] actualScores;
    private static readonly CallResult<LeaderboardScoresDownloaded_t> m_scoresDownloadedResult = new CallResult<LeaderboardScoresDownloaded_t>();

    private void OnEnable()
    {
        if (scores)
        {
            scores.text = "LOADING SCORES...";
        }
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
        scores.gameObject.SetActive(false);

        // Reset text fields
        foreach (Text actualScore in actualScores)
        {
            actualScore.text = "";
            actualScore.gameObject.SetActive(true);
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

            // Handle unknown usernames
            if (username.ToUpper() == "[UNKNOWN]")
            {
                if (scores)
                {
                    // Display loading message
                    scores.gameObject.SetActive(true);
                    scores.text = "LOADING SCORES...";
                    foreach (Text actualScore in actualScores)
                    {
                        actualScore.gameObject.SetActive(false);
                    }
                }

                SteamLeaderboardManager.Init();
                return;
            }

            // Display leaderboard entry in the appropriate UI element based on rank
            if (rank == 1)
            {
                actualScores[0].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else if (rank > 1 && rank < 35)
            {
                actualScores[1].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else if (rank > 34 && rank < 68)
            {
                actualScores[2].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else
            {
                actualScores[3].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }

            rank++;
        }

        // Update the "scores" text field with additional information
        scores.text += "\n\nPRESS ANY KEY TO RETURN";
    }
}
