# Pong Game in MonoGame

This is a recreation of the classic Pong game using MonoGame. The game supports single-player mode with AI and two-player mode. Enjoy the nostalgic feel with retro 8-bit graphics and sound effects!

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Controls](#controls)
- [Gameplay](#gameplay)
- [Credits](#credits)

## Features
- Full-screen gameplay
- Single-player mode with AI
- Two-player mode
- Sound effects and background music
- Score tracking and game-over screen

## Installation
### Prerequisites
- .NET 6.0 SDK or later
- MonoGame Framework ("dotnet new install MonoGame.Templates.CSharp" in Powershell if you are using it for the first time)

### Steps
1. Clone the repository:
    ```sh
    https://github.com/gokturkozkan/Pong-Game.git
    cd Pong-Game
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Build the project:
    ```sh
    dotnet build
    ```

4. Run the game:
    ```sh
    dotnet run
    ```

## Usage
Once the game is running, you will see the title screen and required steps.

## Controls
- **Player 1 (Left Paddle):**
  - Move Up: `W`
  - Move Down: `S`
- **Player 2 (Right Paddle):**
  - Move Up: `Up Arrow`
  - Move Down: `Down Arrow`
- **Start Game / Restart Game:** `Spacebar`
- **Toggle AI for Player 2:** `X`
- **Exit Game:** `Escape`

## Gameplay
- The objective is to score points by hitting the ball past the opponent's paddle.
- The first player to score 3 points wins the game.
- The game starts with a title screen and a prompt to press `Spacebar` to start or `X` to enable AI for the second player.
- The game will display a countdown before the ball is served.
- If a player scores, a countdown will precede the next serve.
- The game ends when a player scores 3 points, displaying a winner message.

## Credits
- **Development:** gokturkozkan
- **Sound Effects:** `ping_pong_8bit_plop` and `ping_pong_8bit_peeeeeep` from opengameart.org
- **Music:** `pong` from opengameart.org
- **Framework:** MonoGame
- **Font:** from www.sweeep.fr by Ansimuz

Feel free to contribute to this project by forking the repository and submitting pull requests. If you encounter any issues, please report them on the [issue tracker](https://github.com/gokturkozkan/Pong-Game/issues).

Enjoy playing Pong!
