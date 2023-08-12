# Steam Leaderboard Integration for Unity

This project consists of two main classes designed to interact with Steam's leaderboard system within Unity. These classes enable developers to update and display the leaderboards for their games using Steamworks.

## Classes

### 1. `SteamLeaderboardManager.cs`

This class is responsible for initializing the Steam leaderboard, uploading player scores, and managing any related achievements. It uses Steam's API to handle these tasks and ensures that the correct leaderboard is referenced.

### 2. `SteamLeaderboardDisplay.cs`

This class is in charge of displaying the leaderboard scores within the game's UI. It downloads the scores from the Steam leaderboard and renders them appropriately on the screen.

## Requirements

- Steamworks SDK
- Unity

## Usage

### Setup

1. Import the Steamworks SDK into your Unity project.
2. Place the `SteamLeaderboardManager.cs` and `SteamLeaderboardDisplay.cs` scripts into your project.

### Updating Scores

To update a player's score, simply call the `UpdateScore(int score)` method from the `SteamLeaderboardManager` class:

```csharp
SteamLeaderboardManager.UpdateScore(playerScore);
```

### Displaying Scores

Add the `SteamLeaderboardDisplay` component to a UI GameObject where you want to display the scores. You can customize the `Text` fields for the scores as required.

### Initialization

Ensure that the leaderboard name is correctly set in the `SteamLeaderboardManager` class, and that the Steamworks SDK is initialized before trying to interact with the leaderboard.

## License

This project is available under the MIT license. See the LICENSE file for more info.

## Contributing

If you find a bug or have an idea for an additional feature, please open an issue or submit a pull request.
