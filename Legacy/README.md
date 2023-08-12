# Steam Leaderboards Integration [Legacy]

This repository contains a script that enables integration with Steam Leaderboards in a Unity game. It utilizes the Steamworks API to interact with Steam's leaderboard system, allowing players to upload and retrieve scores from the leaderboard.

# Note

This folder contains the older version of the script, which handles score updating as well as the leaderboards within a single script. Please rather use the updated components in the repo root.

## Prerequisites

- Unity 2020+
- Steamworks SDK

## Getting Started

1. Ensure you have the Steamworks SDK integrated into your Unity project.
2. Copy the `SteamLeaderBoards.cs` script into your project's scripts folder.
3. Attach the `SteamLeaderBoards.cs` script to a game object in your scene.
4. Configure the `s_leaderboardName` constant to match the name of your leaderboard in the Steamworks backend.
5. Set up the UI elements (such as `scores` and `actualScores`) to display the leaderboard scores.
6. Assign your Steam Leaderboard ID name to the `s_leaderboardName` variable. 
7. Build and run your game with Steam enabled.

## Usage

### Uploading Scores

To upload a score to the leaderboard, use the `UpdateScore` method in the `SteamLeaderBoards` script. The method takes an `int` parameter representing the score to be uploaded. Here's an example usage:

```csharp
SteamLeaderBoards.UpdateScore(5000);
```

## UI Integration

Make sure to set up the necessary UI elements to display the leaderboard scores. The `scores` text component is used to display additional information, while the `actualScores` array of text components displays the individual scores. Customize the UI elements based on your game's design and requirements.

## Notes

- The script relies on the Steamworks API and the SteamManager class to initialize Steam. Ensure that the Steamworks SDK is properly set up in your Unity project.
- The script includes placeholders for handling unknown usernames and triggering in-game achievements. Customize these parts based on your game's logic and requirements.
- Feel free to modify the script and adapt it to your specific needs. Remember to follow the Steamworks API documentation for proper usage.

## License

This project is licensed under the MIT License.
