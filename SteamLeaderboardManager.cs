using Steamworks;
using UnityEngine;

public class SteamLeaderboardManager : MonoBehaviour
{
    public static SteamLeaderboardManager instance;
    private const string s_leaderboardName = "Top Scores";
    private const ELeaderboardUploadScoreMethod s_leaderboardMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest;
    private static bool s_initialized = false;
    private static SteamLeaderboard_t s_currentLeaderboard;
    private static readonly CallResult<LeaderboardFindResult_t> m_findResult = new CallResult<LeaderboardFindResult_t>();
    private static readonly CallResult<LeaderboardScoreUploaded_t> m_uploadResult = new CallResult<LeaderboardScoreUploaded_t>();

    public int testScore = 0;

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

        SteamAPICall_t hSteamAPICall = SteamUserStats.FindLeaderboard(s_leaderboardName);
        m_findResult.Set(hSteamAPICall, OnLeaderboardFindResult);
    }

    private static void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool failure)
    {
        s_currentLeaderboard = pCallback.m_hSteamLeaderboard;
        s_initialized = true;

        SteamLeaderboardDisplay.GetScores(); // Call to get scores
    }

    private static void OnLeaderboardUploadResult(LeaderboardScoreUploaded_t pCallback, bool failure)
    {
        Debug.Log("STEAM LEADERBOARDS: failure - " + failure + " Completed - " + pCallback.m_bSuccess + " NewScore: " + pCallback.m_nGlobalRankNew + " Score " + pCallback.m_nScore + " HasChanged - " + pCallback.m_bScoreChanged);
    }
}
