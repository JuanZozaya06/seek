# Seek

Unity third-person prototype built with Unity 2020.3.32f1.

## Project Layout

- `Assets/_Project/` - game-specific content.
  - `Animations/` - player, enemy, and menu animation clips/controllers.
  - `Art/` - project models, rigs, fonts, and materials.
  - `Scenes/` - playable and experimental scenes.
  - `Scripts/` - game scripts grouped by responsibility.
  - `Settings/` - rendering and lighting settings.
  - `Presets/` - Unity import/editor presets.
- `Assets/_ThirdParty/` - imported packages and asset packs kept separate from project code.
- `Packages/` - Unity Package Manager manifest and lock file.
- `ProjectSettings/` - Unity project configuration.

Generated Unity folders such as `Library/`, `Temp/`, `Logs/`, `UserSettings/`, and IDE files are ignored by Git.

## Scenes

The build settings include:

1. `Assets/_Project/Scenes/Menu/Menu.unity`
2. `Assets/_Project/Scenes/Gameplay/Kitchen/Kitchen.unity`

Experimental scenes live under `Assets/_Project/Scenes/Experimental/`.

## Development

Open the project through Unity Hub with Unity `2020.3.32f1`.

After pulling fresh changes, let Unity reimport assets before running the game. Generated `.csproj` and `.sln` files are intentionally not tracked; Unity recreates them.
