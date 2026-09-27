# 🎭 LIAR VR

## 📖 Overview

A two-player multiplayer VR card game inspired by **Liar's Bar**, centred on bluffing, deception, and strategic challenges.

Built in Unity, the experience combines turn-based card gameplay with immersive VR interaction, allowing two players to compete against each other in a shared virtual environment. Players must decide whether to trust their opponent or challenge their claims while progressing through each round.

<br>

## ✨ Key Features

### 🃏 Bluffing & Card Gameplay

- **Turn-Based Play:** Players take turns placing cards and making strategic decisions
- **Bluffing Mechanic:** Players can attempt to deceive their opponent about the cards they play
- **Challenge System:** Opponents can challenge a player's claim when they suspect a bluff
- **Competitive Gameplay:** Each decision can influence the outcome of the match

### 🌐 Multiplayer VR Experience

- **Two-Player Multiplayer:** Designed for two players competing in the same VR game session
- **Networked Gameplay:** Player actions and game events are synchronised between both players
- **Shared Virtual Environment:** Both players interact within the same multiplayer game space

### 🥽 VR Interaction & Player Presence

- **Hand-Based Interaction:** Players interact with cards and game elements using VR controls
- **Body-Tracked Avatars:** Player movement is represented through virtual character avatars
- **Immersive Card Interaction:** Physical-style card handling brings traditional card gameplay into VR

### 🎮 In-Game Controls & Communication

- **Voice Communication Buttons:** Players can trigger short pre-recorded voice messages during the match
  - **“Hurry up, hurry up”** — prompts the opponent to make their move
  - **“Is it lag or just you?”** — allows the player to question whether a delay is caused by network lag or the opponent taking time to play
- **Challenge Button:** Allows a player to challenge the opponent when they believe the opponent is bluffing
- **How-to-Play Board:** An instruction board positioned beside the player's seating area remains available throughout the match, allowing players to review the game rules and interaction instructions whenever needed

### 🏆 Match Outcomes

- **Victory Sequence:** Winning players are presented with a dedicated victory animation and visual effects
- **Defeat Sequence:** Losing players experience a separate defeat animation and visual sequence
- **Outcome Environment:** Match results are reinforced through environmental effects and dedicated end-game presentation

<br>

## 🛠️ Technical Specifications

- **Game Engine:** Unity 6.0 (`6000.0.59f2`)
- **Programming Language:** C#
- **VR Framework:** XR Interaction Toolkit
- **Multiplayer Networking:** Photon Unity Networking (PUN)
- **Target Platform:** Meta Quest
- **Interaction:** VR controller and hand-based interaction
- **Gameplay Type:** Two-player networked VR card game
- **Core Systems:** Turn management, card interaction, bluff/challenge mechanics, multiplayer synchronisation and match outcomes

<br>

## 📋 Requirements

### Hardware Requirements

- **VR Headset:** Meta Quest headset
- **Controllers:** Meta Quest Touch controllers
- **Internet Connection:** Required for two-player multiplayer gameplay
- **Development PC:** A system capable of running Unity and building applications for Meta Quest
- **USB Connection:** Required when deploying the application directly from Unity to the headset

### Software Requirements

- **Unity Hub**
- **Unity Editor:** Unity 6.0 (`6000.0.59f2`)
- **Android Build Support** with SDK, NDK and OpenJDK
- **Photon Unity Networking (PUN)** for multiplayer functionality

<br>

## 🚀 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR-USERNAME/LiarVR-VRGame.git
cd LiarVR-VRGame
```

### 2. Open the Project

- Open **Unity Hub**
- Select **Add → Add project from disk**
- Choose the cloned project folder
- Open the project using **Unity 6.0 (`6000.0.59f2`)**
- Allow Unity to restore the required packages and regenerate the `Library` folder

### 3. Prepare the Meta Quest Headset

- Enable **Developer Mode** on the Meta Quest headset
- Connect the headset to the development PC using USB
- Allow **USB debugging** when prompted on the headset
- Ensure the headset is recognised by Unity for Android deployment

### 4. Configure Multiplayer

- Ensure the project's **Photon Unity Networking (PUN)** configuration is available
- A valid Photon configuration is required for networked multiplayer functionality
- Both players require an internet connection to participate in the multiplayer session

### 5. Build & Run

- Open the main game scene from the `Assets/Scenes/` folder
- Open **File → Build Profiles**
- Select **Android**
- Switch to the Android platform if required
- Select the connected Meta Quest headset
- Build and run the application

<br>

## 🎮 How to Play

### Getting Started

1. **Launch the Game:** Start LIAR VR on both players' Meta Quest headsets.
2. **Join the Multiplayer Session:** Connect both players to the same game session.
3. **Enter the Game:** Each player is represented within the shared virtual environment.
4. **Check the Instructions:** A How-to-Play board is positioned beside the player's seating area and remains accessible throughout the match.
5. **Begin the Match:** Players take turns playing cards and attempting to deceive or challenge their opponent.

### 🃏 Card Gameplay

1. **Play Your Turn:** Select and place cards when it is your turn.
2. **Make Your Claim:** Present your play to the opponent.
3. **Bluff Strategically:** Attempt to deceive the opponent when appropriate.
4. **Judge the Opponent:** Decide whether you believe the opponent's play.
5. **Challenge a Bluff:** Use the Challenge button when you believe the opponent is lying.
6. **Continue the Match:** Players alternate turns until the game reaches its outcome.

### 🎮 In-Game Buttons

Three interactive buttons are available during gameplay:

- **“Hurry up, hurry up”** — plays a pre-recorded voice message encouraging the opponent to make their move
- **“Is it lag or just you?”** — plays a pre-recorded voice message when the opponent is taking longer than expected, allowing the player to question whether the delay is caused by network lag or the opponent
- **Challenge Button** — used when a player believes the opponent is bluffing and wants to challenge their play

### 📖 In-Game Guidance

A **How-to-Play instruction board** is positioned beside the player's seating area and remains available throughout the game.

Players can refer to the board whenever needed to review the gameplay rules and interaction instructions without leaving the VR experience.

### 🥽 VR Interaction

- Interact with cards and game elements using VR controls
- Select and place cards during your turn
- Use the interactive buttons positioned within the game environment
- View and interact with the opponent through the shared multiplayer environment
- Player movement is represented through virtual avatars

### 🏆 Match Outcome

At the end of the match, players are presented with different outcome sequences depending on the result:

- **Victory:** The winning player experiences a dedicated victory animation and visual effects
- **Defeat:** The losing player experiences a separate defeat animation and visual sequence

These sequences provide a clear visual conclusion to the multiplayer match.

<br>

## 🎯 Core Systems & Interactions

### 🌐 Multiplayer System

The game uses **Photon Unity Networking (PUN)** to support two-player networked gameplay within a shared VR environment.

- Synchronises both players within the multiplayer session
- Communicates gameplay actions and events between players
- Supports turn-based interaction between the two participants
- Maintains the shared multiplayer game experience

### 🃏 Card & Turn System

The core gameplay revolves around alternating turns, card interaction, bluffing, and deciding whether to trust the opponent.

- Manages player turns
- Allows players to select and play cards
- Supports bluffing-based gameplay
- Updates the game state as players complete their actions

### 🚨 Challenge System

Players can use the dedicated **Challenge button** when they believe their opponent is bluffing.

- Allows an opponent's play to be challenged
- Connects directly with the bluffing mechanic
- Influences the progression and outcome of the match

### 🔊 Voice Communication System

Two interactive buttons provide simple pre-recorded communication between players during the multiplayer match.

- **“Hurry up, hurry up”** — encourages the opponent to make their move
- **“Is it lag or just you?”** — allows a player to question a delay in the opponent's response

These provide lightweight communication without requiring players to leave the VR experience.

### 📖 In-Game Guidance

A **How-to-Play instruction board** is positioned beside the player's seating area and remains available throughout the match.

This allows players to review the game rules and interaction instructions whenever required without interrupting gameplay.

### 🥽 VR Interaction & Player Presence

- VR-based interaction with cards and game elements
- Interactive buttons positioned within the virtual environment
- Virtual avatars represent both players within the shared game space
- Player actions are reflected within the multiplayer experience

### 🏆 Victory & Defeat System

The game provides dedicated outcome experiences after the result of a match is determined.

- **Victory Sequence:** Presents the winning player with a dedicated victory animation and visual effects
- **Defeat Sequence:** Presents the losing player with a separate defeat animation and visual sequence
- **Environmental Feedback:** Visual effects reinforce the outcome and provide a clear conclusion to the match

<br>

## 📝 Credits

### Development Team

- Pranavv Jothinathan
- Yifei Liu
- Lintao Guo

Developed as part of the MSc Immersive Technologies programme at the University of Bristol.

### Third-Party Tools & Assets

The project was developed using technologies and packages including:

- Unity
- XR Interaction Toolkit
- Photon Unity Networking (PUN)
- TextMesh Pro

Additional third-party 3D models, textures, audio, animations, and other assets used within the project remain subject to their respective licences and terms of use.

<br>

## 🔮 Future Enhancements

Potential future improvements include:

- Expand the current two-player game to support larger multiplayer sessions
- Add in-game voice communication option between players
- Expand player avatar customisation
- Improve visual and environmental feedback during gameplay
- Introduce additional game environments or table themes
- Further improve multiplayer synchronisation and network handling

<br>

## 🎥 Demo Video

A gameplay demonstration of **LIAR VR** VR game, showcasing multiplayer card gameplay, bluffing and challenge mechanics, VR interaction, in-game communication, and victory/defeat sequences.

▶️ [View / Download the LIAR VR Demo Video](https://github.com/pranavv-jothinathan/LiarVR-VRGame/releases/tag/demoVideo-v1)

