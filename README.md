# DungeonExplorer

A modern C# WPF re-imagining of a Python console dungeon crawler, redesigned with MVVM, pathfinding algorithms, and SQLite persistence.

## Overview

DungeonExplorer is a fantasy-themed grid-navigation game where the player explores a castle, collects legendary items, and ultimately confronts the final Dungeon boss. This project is a complete enhancement of an earlier Python console application, rebuilt in C#, WPF, and MVVM, with major upgrades to architecture, gameplay systems, UI polish, and data persistence.

The project serves as the capstone artifact for SNHU CS-499, demonstrating software engineering practices, algorithmic implementation, UI/UX design, and database integration.

## Key Features
### Modern MVVM Architecture

The entire application is structured using the Model–View–ViewModel pattern:

Clear separation of concerns

Testable and extendable game logic

Clean binding between UI and state machine

### Enhanced Gameplay Logic

Explore interconnected rooms

Collect all six legendary items

Enter the Dungeon for a final battle

Dynamic status messages and inventory updates

Win/Loss outcome handling with scoring summary

### Algorithms & Scoring System

The original game’s simple scoring was replaced with a more modern system:

Breadth-First Search (BFS) computes shortest distances between rooms

Held–Karp dynamic programming creates an optimal route visiting all item rooms

A computed par score penalizes inefficient routes

Final score reflects performance + strategic planning

### SQLite Leaderboard with EF Core

A permanent leaderboard tracks all completed runs:

Player table stores unique player names

Score table stores points, moves, par, win/loss, items collected

EF Core handles schema, context, and queries

Filters include:

“Show only my scores”

“Wins only”

Data is displayed live in the overlay menu

### Dungeon-Themed UI

Custom color palette (stone gray, parchment text, bronze accents)

Semi-transparent fog overlay for menu/game over

Clear visual hierarchy and polished layout

Keyboard controls (WASD + arrows) and clickable movement buttons

## How to Play
### Objective

Explore the castle, collect all six legendary items, and defeat the Dungeon boss.

### Controls

W / A / S / D or Arrow Keys → Move between rooms

Pick Up Item → Collect item in current room

Movement buttons available on-screen as well

### Scoring

+100 points per item collected

Par score is computed via BFS + Held–Karp

Moves over par incur penalties

Final score appears after battle resolution

### Leaderboard & Persistence

All completed runs are saved permanently to dungeon_scores.db using SQLite and EF Core.

Each score entry includes:

Player name

Win/Loss result

Items collected

Moves taken + Par

Final score

Timestamp

Filter options:

Show only my scores

Wins only

The leaderboard appears both on the main menu overlay and the game over screen.

## Project Structure
DungeonExplorer/
│
├── Models/
│   ├── Player.cs
│   ├── Room.cs
│   ├── GameState.cs
│   └── ...
│

├── ViewModels/
│   └── GameViewModel.cs
│

├── Views/
│   └── MainWindow.xaml
│

├── Services/
│   ├── GameService.cs
│   └── LeaderboardService.cs
│

├── Data/
│   ├── GameDbContext.cs
│   ├── PlayerEntity.cs
│   └── ScoreEntity.cs
│

├── Converters/
│   └── BooleanToVisibilityConverter.cs
│

└── dungeon_scores.db

## Technologies Used

C# (.NET 8)

WPF (Windows Presentation Foundation)

SQLite

Entity Framework Core

MVVM architecture

BFS + Held–Karp algorithms

## Getting Started
Prerequisites

Visual Studio 2022

.NET 8 SDK

SQLite (included automatically with EF Core provider)

Clone the Repository
git clone https://github.com/dexmel1/SNHU-CS-499.git

Run the Application

Open the solution in Visual Studio

Build the project

Run it

A new dungeon_scores.db will be created on first launch

# Screenshots

(Add your screenshots here for your ePortfolio and capstone submission)

Recommended images:

<img width="787" height="872" alt="Screenshot 2025-12-05 085210" src="https://github.com/user-attachments/assets/6726bc97-79c9-4d90-9601-bc4e0ac71335" />


<img width="785" height="874" alt="Screenshot 2025-12-05 085305" src="https://github.com/user-attachments/assets/08a4ea97-5fd4-47ad-b4a7-52c5751fb169" />


<img width="788" height="875" alt="Screenshot 2025-12-05 085406" src="https://github.com/user-attachments/assets/92e8391a-7acf-454f-b4dd-d7b821f4d90d" />


## Future Improvements (Optional Ideas)

Animated transitions between rooms

Multiple difficulty levels

More items and room event types

Achievements system (stored in the database)

Sound and ambient dungeon music

## License

This project is part of the SNHU CS-499 Capstone and is intended for academic and portfolio use.
