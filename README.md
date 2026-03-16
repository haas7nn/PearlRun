# 🏃 PearlRun (بيرل رن)

A 2.5D side-scrolling action-adventure platformer set in modern-day Bahrain.

Built with **Unity 6** | Course: **IT8101 – Games Development** | Team: **PearlBytes**

---

## 🎮 About The Game

Awal is a young Bahraini running late for a traditional pearl diving competition. What starts as a simple rush quickly escalates into a wild chase across the entire country as he gets caught up in chaotic situations at each location. Along the way Awal collects pearls scattered across Bahrain. The player must avoid obstacles, jump over barriers, slide under blockages, and react quickly to keep progressing through the levels.

The gameplay is fast-paced, exciting, and filled with Bahraini cultural references, local humor, and recognizable landmarks that make every level feel fun and authentic.

---

## 🗺️ Levels

| Level | Location | Theme |
|-------|----------|-------|
| 1 | Muharraq Streets | Tutorial – Market obstacles and rooftops |
| 2 | Manama City | Urban platforming and construction sites |
| 3 | Qarqaoun Neighbourhood | Festival chaos at night |
| 4 | Desert & Tree of Life | Desert survival with sandstorms |
| 5 | Amwaj Islands | Coastal platforming and boat hopping |
| 6 | Bahrain International Circuit | Final challenge with race cars |

---

## 🎯 Features

### Basic Features
- Main Menu with New Game, Level Select, Instructions, Settings, Credits, Quit
- 6 distinct playable levels with unique environments and obstacles
- Animated player character with multiple states (run, jump, slide, attack, hurt, die)
- Side-scrolling follow camera with look-ahead and screen shake
- Keyboard controls consistent across all levels
- Pearl collection reward system with HUD display
- Sound effects, music, and ambient audio per level with Audio Manager
- Particle effects and post-processing with unique color grading per level
- Bug-free polished gameplay
- Windows PC build

### Custom Features
- **Advanced: Dynamic Obstacle and Enemy AI System** – Finite State Machine AI with patrol, detect, chase, and attack states. Multiple enemy types per level. Timed car spawning system for Level 6
- **Power-Up System** – Shield, Magnet, Slow Motion, Double Points
- **Cutscene and Dialogue System** – Story transitions between levels with character dialogue
- **Checkpoint and Save System** – Auto-save at checkpoints, respawn on death, level progress saved
- **Dynamic Weather and Visual Effects** – Sandstorms, festive lights, water effects, unique post-processing per level
- **Progress Bar and HUD System** – Real-time progress bar, score, lives, power-up timers

---

## 🕹️ Controls

| Key | Action |
|-----|--------|
| A / Left Arrow | Move left |
| D / Right Arrow | Move right |
| Space | Jump |
| Space (in air) | Double Jump |
| S / Down Arrow | Slide |
| F / Left Click | Punch Attack |
| Left Shift | Sprint Burst |
| Escape | Pause Menu |

---

## 👥 Team PearlBytes

| Name | Role | Level |
|------|------|-------|
| Hasan | Scrum Master + Lead Programmer | Level 1: Muharraq |
| Ameena | AI & Combat Programmer | Level 2: Manama |
| Ruqaya | Systems Programmer | Level 3: Qarqaoun |
| Adil | Level Designer | Level 4: Desert |
| Rana | Animator & UI Designer | Level 5: Amwaj |
| Samana | Audio & QA Tester | Level 6: Circuit |

---

## 🛠️ Tech Stack

- **Engine:** Unity 6
- **Language:** C#
- **Art:** Mixamo (character), Unity Asset Store (environments)
- **Audio:** Freesound.org, Pixabay, Mixkit
- **Platform:** PC (Windows)

---

## 📁 Project Structure
Assets/
├── Animations/ # Character and environment animations
├── Audio/
│ ├── Music/ # Background music per level
│ └── SFX/ # Sound effects
├── Materials/ # All materials and textures
├── Prefabs/
│ ├── Collectibles/ # Pearl prefabs
│ ├── Enemies/ # Enemy prefabs
│ ├── Obstacles/ # Obstacle prefabs
│ └── PowerUps/ # Power-up prefabs
├── Scenes/
│ ├── MainMenu
│ ├── Level1_Muharraq
│ ├── Level2_Manama
│ ├── Level3_Qarqaoun
│ ├── Level4_Desert
│ ├── Level5_Amwaj
│ ├── Level6_Circuit
│ └── Victory
├── Scripts/
│ ├── Player/ # Player controller, collision, animation
│ ├── Enemies/ # Enemy AI scripts
│ ├── Systems/ # Game manager, camera, audio
│ └── UI/ # Menu, HUD, popup scripts
└── UI/ # UI sprites and assets

---

## 🏗️ Build Instructions

1. Open the project in Unity 6
2. Go to File > Build Settings
3. Make sure all scenes are added in the correct order (MainMenu first)
4. Select Platform: Windows
5. Click Build

---

## 📅 Development Timeline

| Week | Focus |
|------|-------|
| 1 | Project setup, core mechanics, assets |
| 2 | Systems, Level 1 and 2 built |
| 3 | All 6 levels built |
| 4 | Integration, polish, testing |
| 5 | Bug fixing, final build, submission |

---

## 📝 License

This project is created for educational purposes as part of the IT8101 Games Development course at Bahrain Polytechnic.
