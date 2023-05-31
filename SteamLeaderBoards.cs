using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLeaderBoards : MonoBehaviour
{
    public static SteamLeaderBoards instance;

    private SteamLeaderboardEntries_t m_SteamLeaderboardEntries;

    private const string s_leaderboardName = "Top Scores";
    private static bool s_initialized = false;

    private const ELeaderboardUploadScoreMethod s_leaderboardMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest;
    private static SteamLeaderboard_t s_currentLeaderboard;

    private static readonly CallResult<LeaderboardFindResult_t> m_findResult = new CallResult<LeaderboardFindResult_t>();
    private static readonly CallResult<LeaderboardScoreUploaded_t> m_uploadResult = new CallResult<LeaderboardScoreUploaded_t>();
    private static readonly CallResult<LeaderboardScoresDownloaded_t> m_scoresDownloadedResult = new CallResult<LeaderboardScoresDownloaded_t>();

    public bool getScores;
    public int testScore = 0;

    public Text scores;
    public Text[] actualScores;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        if (scores)
        {
            scores.text = "LOADING SCORES...";
        }
    }

    private void OnEnable()
    {
        Init();
    }

    public static void UpdateScore(int score)
    {
        if (!SteamManager.Initialized)
            return;

        if (instance.testScore > 0)
        {
            score += instance.testScore;
            instance.testScore = 0;
        }

        if (!s_initialized)
        {
            Init();
            Debug.Log("Can't upload to the leaderboard because it isn't loaded yet");
        }
        else
        {
            // Upload the score to the Steam leaderboard
            SteamAPICall_t hSteamAPICall = SteamUserStats.UploadLeaderboardScore(s_currentLeaderboard, s_leaderboardMethod, score, null, 0);
            m_uploadResult.Set(hSteamAPICall, OnLeaderboardUploadResult);
            SteamAPI.RunCallbacks();

            if (score > 40000)
            {
                Game.instance.Achieve("jackpot"); // Triggers an in-game achievement
            }
        }
    }

    public static void Init()
    {
        if (!SteamManager.Initialized)
            return;

        // Find the Steam leaderboard by name
        SteamAPICall_t hSteamAPICall = SteamUserStats.FindLeaderboard(s_leaderboardName);
        m_findResult.Set(hSteamAPICall, OnLeaderboardFindResult);
    }

    private static void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool failure)
    {
        s_currentLeaderboard = pCallback.m_hSteamLeaderboard;
        s_initialized = true;

        if (instance.getScores)
            GetScores();
    }

    private static void OnLeaderboardUploadResult(LeaderboardScoreUploaded_t pCallback, bool failure)
    {
        Debug.Log("STEAM LEADERBOARDS: failure - " + failure + " Completed - " + pCallback.m_bSuccess + " NewScore: " + pCallback.m_nGlobalRankNew + " Score " + pCallback.m_nScore + " HasChanged - " + pCallback.m_bScoreChanged);
    }

    public static void GetScores()
    {
        if (!s_initialized)
        {
            Debug.Log("Can't fetch leaderboard because it isn't loaded yet");
        }
        else
        {
            // Download leaderboard entries
            SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(s_currentLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 100); // Maximum of 100 entries
            m_scoresDownloadedResult.Set(handle, OnLeaderboardScoresDownloaded);
        }
    }

    private static void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        instance.scores.gameObject.SetActive(false);

        // Reset text fields
        foreach (Text actualScore in instance.actualScores)
        {
            actualScore.text = "";
            actualScore.gameObject.SetActive(true);
        }

        int numEntries = pCallback.m_cEntryCount;
        instance.m_SteamLeaderboardEntries = pCallback.m_hSteamLeaderboardEntries;

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
                if (instance.scores)
                {
                    // Display loading message
                    instance.scores.gameObject.SetActive(true);
                    instance.scores.text = "LOADING SCORES...";
                    foreach (Text actualScore in instance.actualScores)
                    {
                        actualScore.gameObject.SetActive(false);
                    }
                }

                Init();
                return;
            }

            // Display leaderboard entry in the appropriate UI element based on rank
            if (rank == 1)
            {
                instance.actualScores[0].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else if (rank > 1 && rank < 35)
            {
                instance.actualScores[1].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else if (rank > 34 && rank < 68)
            {
                instance.actualScores[2].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }
            else
            {
                instance.actualScores[3].text += "#" + rank.ToString() + ". " + username.ToUpper() + "  : <color=#E50000> " + leaderboardEntry.m_nScore.ToString("n2") + "</color>\n";
            }

            rank++;
        }

        // Update the "scores" text field with additional information
        instance.scores.text += "\n\nPRESS ANY KEY TO RETURN";
    }
}
