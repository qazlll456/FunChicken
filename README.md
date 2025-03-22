# FunChicken - CS2 Plugin

A whimsical **Counter-Strike 2** plugin that adds chicken-themed chaos to your server! Spawn personal pet chickens that follow you, create static chickens, or engage in a wild chicken fight—all powered by [CounterStrikeSharp (CSSHARP)](https://github.com/roflmuffin/CounterStrikeSharp). Features a robust permission system, cooldowns, extensive configurability, and a fiery curse against the infamous "Furious"!

## Features

- **Following Chicken Pets**: Spawn a personal chicken that follows players with smooth movement and periodic jumping (teleports if too far).
- **Normal Chickens**: Spawn static chickens with a server-wide cap (default: 100).
- **Chicken Fight Mode**: Challenge yourself to kill a swarm of chickens in a timed event (one player at a time).
- **Command System**: Chat and console commands with aliases, permissions, and cooldowns.
- **Permissions**: Configurable modes (`@all`, `@blacklist`, `@whitelist`) based on SteamIDs.
- **Cleanup**: Automatically removes invalid chickens and despawns all on plugin unload.
- **Configurable**: JSON-based settings for distances, counts, cooldowns, and more.
- **Lore Element**: Includes Qazlll456’s dramatic curse against "Furious" (optional).

## Installation

### Prerequisites
- A CS2 server with [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) installed.
- Admin access to the server.

### Steps
1. Download the latest release from [Releases](https://github.com/[your-username]/FunChicken/releases) or clone the repo:  
   > git clone https://github.com/[your-username]/FunChicken.git  
2. Copy the `FunChicken` folder to `csgo/addons/counterstrikesharp/plugins/`.  
3. Start/restart your server or load manually:  
   > css_plugins load FunChicken  
4. A `funchicken.json` config file will be created in `csgo/addons/counterstrikesharp/configs/plugins/funchicken/`.

## Commands

| Command            | Aliases                  | Description                                                                 |
|--------------------|--------------------------|-----------------------------------------------------------------------------|
| `!spawnchickenpet` | `!sc`                   | Spawns a personal chicken pet that follows you (1 per player).             |
| `!iwantachicken`   | `!ic`                   | Spawns a normal chicken (server limit: 100).                               |
| `!killchicken`     | `!kc`                   | Removes your following chicken pet.                                        |
| `!petjump`         |                         | Makes your chicken pet jump *(Note: Unstable function, may not work consistently)*. |
| `!chickenfight`    | `!cf`                   | Spawns chickens for a timed fight challenge (1 player at a time).          |
| `!funchicken`      |                         | Displays all available commands.                                           |
| `!curses`          |                         | Shows Qazlll456’s curse against Furious.                                   |

**Console Commands**: Use `css_` prefix (e.g., `css_spawnpet`).

## Configuration

The plugin generates a `funchicken.json` file with these key sections:

### PluginInfo
- **Name**: `FunChicken`
- **Author**: `qazlll456 from HK with xAI assistance`
- **Version**: `1.0`
- **Description**: `A fun plugin for chicken lovers in CS2 with permissions and cooldowns`

### Commands
Customize each command’s settings:  
- `Enabled`: Toggle on/off.  
- `Aliases`: Chat triggers (e.g., `["!sc"]`).  
- `Description`: Help text.  
- `CooldownSeconds`: Time between uses (default: 1.0s).  
- Specific options:  
  - `JumpHeight` (PetJump): Height in units (default: 50.0).  
  - `ChickenCount` (ChickenFight): Number of chickens (default: 100).  
  - `DurationSeconds` (ChickenFight): Fight duration (default: 10.0s).  
  - `ShowLogs`: Log actions to console.

### Permissions
- **Mode**: `@all` (everyone), `@blacklist` (exclude listed SteamIDs), or `@whitelist` (only listed SteamIDs).  
- **Blacklist/Whitelist**: Arrays of SteamIDs (e.g., `["76561198000000000"]`).  
- **Info**: Explains permission modes.

### Other Settings
- **PetMaxDistance**: Max distance before pet teleports (default: 80.0).  
- **MyCurses**: Editable curse text (see below).  
- **Notes**: Configuration tips.

#### Example Config

```json
> {  
>   "PluginInfo": {  
>     "Name": "FunChicken",  
>     "Author": "qazlll456 from HK with xAI assistance",  
>     "Version": "1.0",  
>     "Description": "A fun plugin for chicken lovers in CS2 with permissions and cooldowns"  
>   },  
>   "Commands": {  
>     "PetJump": {  
>       "Enabled": true,  
>       "Aliases": ["!petjump"],  
>       "Description": "Makes your following chicken jump (Note: Unstable function, may not work consistently)",  
>       "JumpHeight": 50.0,  
>       "ShowLogs": true,  
>       "CooldownSeconds": 1.0  
>     },  
>     "ChickenFight": {  
>       "Enabled": true,  
>       "Aliases": ["!cf", "!chickenfight"],  
>       "Description": "Spawns chickens for a fight challenge (only 1 player at a time)",  
>       "ChickenCount": 100,  
>       "DurationSeconds": 10.0,  
>       "ShowLogs": true,  
>       "CooldownSeconds": 1.0  
>     }  
>   },  
>   "Permissions": {  
>     "Mode": "@all",  
>     "Blacklist": [],  
>     "Whitelist": [],  
>     "Info": "Permission Modes:\n@all: Everyone\n@blacklist: All except Blacklist\n@whitelist: Only Whitelist"  
>   },  
>   "PetMaxDistance": 80.0,  
>   "MyCurses": "Qazlll456 curses Furious, that vile fiend who dared disrespect me...",  
>   "Notes": "Edit this file to configure permissions, cooldowns, pet distance, and more. Reload plugin to apply."  
> }

```

Reload after edits:  
> css_plugins reload FunChicken

## Technical Details

### Classes
- **ChickenManager**: Manages chicken spawning, movement, and cleanup.  
  - Spawns chickens at specified coordinates.  
  - Updates pet positions with jumping animations.  
  - Caps normal chickens at 100.  
  - Tracks following pets and fight chickens.  
- **CommandHandler**: Processes commands with permission/cooldown checks.  
- **PermissionSystem**: Validates access and applies cooldowns per player.  
- **FunChicken**: Main plugin class, handles loading, ticking, and unloading.  
- **Config**: JSON-based configuration management.

### Mechanics
- **Pet Movement**: Updates every 8 ticks, teleports if beyond `PetMaxDistance`, includes sinusoidal jumping.  
- **Cleanup**: Removes invalid entities periodically and on unload.  
- **Fight Mode**: Spawns chickens in a circle, ends after a timer with kill count feedback.

## Contributing
1. Fork the repo:  
   > git clone https://github.com/[your-username]/FunChicken.git  
2. Edit in a C# IDE (e.g., Visual Studio).  
3. Test on a local CS2 server.  
4. Submit a pull request with details.

## Notes
- **Unstable Feature**: `!petjump` is experimental and may fail due to timing or engine issues—use cautiously.  
- **Performance**: High chicken counts (e.g., 100 in `!chickenfight`) may lag on weaker servers.  
- **Curse Text**: A creative rant by qazlll456 against "Furious"—keep it for fun or remove it.

## Credits
- **Author**: qazlll456 from HK  
- **Assistance**: xAI’s Grok  
- **Framework**: [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

## License
[MIT License](LICENSE)

## Support
If you enjoy this plugin, consider supporting my work!  
[Money, Steam game or any valuable is also welcome](https://streamlabs.com/BKCqazlll456/tip)

---
*Generated on March 22, 2025*