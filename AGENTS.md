# Agent Instructions

Call the user Zozi.

## Project

- Unity version: `2020.3.32f1`.
- Main entry scene: `Assets/_Project/Scenes/Menu/Menu.unity`.
- Gameplay round scene: `Assets/_Project/Scenes/Gameplay/Kitchen/Kitchen.unity`.
- Game-specific content belongs under `Assets/_Project/`.
- Imported packages and asset packs belong under `Assets/_ThirdParty/`.

## Unity Workflow

- Preserve `.meta` files when moving Unity assets.
- Do not commit generated folders or files such as `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `.csproj`, or `.sln`.
- Keep scene paths in `ProjectSettings/EditorBuildSettings.asset` valid after scene moves.
- If Unity is open and `Temp/UnityLockfile` exists, do not run a second batch import against the project.

## Gameplay Rules

- The player is the seeker.
- Hidden characters are hiders.
- Kitchen rounds last five minutes.
- Selecting a hider ends the round with a seeker win and logs `found one`.
- Timer expiration ends the round with a hider win.
