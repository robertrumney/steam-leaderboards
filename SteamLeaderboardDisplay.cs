using Steamworks;

using UnityEngine;
using UnityEngine.UI;

public class SteamLeaderboardDisplay : MonoBehaviour
{
    public Text info;
    public Text scores;

    [HideInInspector]
    public SteamLeaderboardEntries_t m_SteamLeaderboardEntries;
    private static readonly CallResult<LeaderboardScoresDownloaded_t> m_scoresDownloadedResult = new CallResult<LeaderboardScoresDownloaded_t>();

    private void OnEnable()
    {
        if (info)
        {
            info.gameObject.SetActive(true);
            info.text = "LOADING SCORES...";
        }
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
            SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(SteamLeaderboardManager.s_currentLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 10); // Maximum of 10 entries
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

        scores.text = "";
        scores.gameObject.SetActive(true);
        
        int numEntries = pCallback.m_cEntryCount;
        m_SteamLeaderboardEntries = pCallback.m_hSteamLeaderboardEntries;

        int rank = 1;

        // Process each leaderboard entry
        for (int index = 0; index < numEntries; index++)
        {
            SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, index, out LeaderboardEntry_t leaderboardEntry, null, 0);
            string username = SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser);

            // Handle rare unknown username issue and reset leaderboard
            if (username.ToUpper() == "[UNKNOWN]")
            {
                if (info)
                {
                    // Display loading message
                    info.gameObject.SetActive(true);
                    info.text = "LOADING SCORES...";

                    scores.gameObject.SetActive(false);
                }

                SteamLeaderboardManager.Init();
                return;
            }

            scores.text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=white> " + leaderboardEntry.m_nScore.ToString("n0") + "</color>\n";

            rank++;
        }

        // Update the "info" text field with additional information
        info.text += "\n\nPRESS ANY KEY TO RETURN";
    }
}
