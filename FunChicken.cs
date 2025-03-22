using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System;
using System.IO;
using System.Text.Json;

namespace FunChicken
{
    public class FunChicken : BasePlugin
    {
        public override string ModuleName => "FunChicken";
        public override string ModuleAuthor => "qazlll456 from HK with xAI assistance";
        public override string ModuleVersion => "1.0";
        public override string ModuleDescription => "A fun plugin for chicken lovers with permissions.";

        private Config? _config;
        private PermissionSystem? _permissions;
        private ChickenManager? _chickenManager;
        private CommandHandler? _commandHandler;
        private int _tickCounter = 0;
        private const int FOLLOW_INTERVAL_TICKS = 8;

        public override void Load(bool hotReload)
        {
            string configDir = Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "configs", "plugins", "funchicken");
            string configPath = Path.Combine(configDir, "funchicken.json");
            Directory.CreateDirectory(configDir);

            _config = File.Exists(configPath) 
                ? JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath)) 
                : new Config();
            ValidateConfig();
            File.WriteAllText(configPath, JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true }));

            _permissions = new PermissionSystem(_config!);
            _chickenManager = new ChickenManager(_config!);
            _commandHandler = new CommandHandler(_config!, _permissions, _chickenManager, this);

            AddCommand("css_spawnpet", "Spawns a following chicken pet", (player, info) => 
                _commandHandler!.SpawnPet(player ?? throw new ArgumentNullException(nameof(player))));
            AddCommand("css_despawnpet", "Despawns the player's pets", (player, info) => 
                _commandHandler!.DespawnPet(player ?? throw new ArgumentNullException(nameof(player))));
            AddCommand("css_petjump", "Makes the pet jump", (player, info) => 
                _commandHandler!.PetJump(player ?? throw new ArgumentNullException(nameof(player))));
            AddCommand("css_spawnstatic", "Spawns a normal chicken", (player, info) => 
                _commandHandler!.SpawnNormalChicken(player ?? throw new ArgumentNullException(nameof(player))));
            AddCommand("css_chickenfight", "Spawns chickens for a fight", (player, info) => 
                _commandHandler!.ChickenFight(player ?? throw new ArgumentNullException(nameof(player))));
            AddCommand("css_help", "Shows available commands", (player, info) => 
                _commandHandler!.ShowHelp(player ?? throw new ArgumentNullException(nameof(player))));
            AddCommand("css_curses", "Shows the curse text", (player, info) => 
                _commandHandler!.ShowCurses(player ?? throw new ArgumentNullException(nameof(player))));
            AddCommandListener("say", OnSayCommand);

            RegisterListener<Listeners.OnTick>(OnTick);
        }

        private void ValidateConfig()
        {
            foreach (var cmd in _config!.Commands)
            {
                if (cmd.Value.CooldownSeconds < 0) cmd.Value.CooldownSeconds = 1.0f;
            }
            if (_config!.PetMaxDistance < 0) _config.PetMaxDistance = 80.0f;
            if (!new[] { "@all", "@blacklist", "@whitelist" }.Contains(_config!.Permissions.Mode))
                _config.Permissions.Mode = "@all";
        }

        private void OnTick()
        {
            if (++_tickCounter >= FOLLOW_INTERVAL_TICKS)
            {
                if (_chickenManager != null)
                {
                    _chickenManager.CleanupChickens();
                    foreach (var kvp in _chickenManager.FollowingPets)
                        if (kvp.Key.IsValid && kvp.Value.IsValid)
                            _chickenManager.UpdatePetPosition(kvp.Key, kvp.Value);
                }
                _tickCounter = 0;
            }
        }

        private HookResult OnSayCommand(CCSPlayerController? player, CommandInfo info)
        {
            if (player == null || !player.IsValid) return HookResult.Continue;

            string message = info.GetArg(1).ToLower();
            if (_config!.Commands["SpawnChickenPet"].Aliases.Contains(message)) { _commandHandler!.SpawnPet(player); return HookResult.Stop; }
            if (_config!.Commands["SpawnNormalChicken"].Aliases.Contains(message)) { _commandHandler!.SpawnNormalChicken(player); return HookResult.Stop; }
            if (_config!.Commands["KillChicken"].Aliases.Contains(message)) { _commandHandler!.DespawnPet(player); return HookResult.Stop; }
            if (_config!.Commands["PetJump"].Aliases.Contains(message)) { _commandHandler!.PetJump(player); return HookResult.Stop; }
            if (_config!.Commands["ChickenFight"].Aliases.Contains(message)) { _commandHandler!.ChickenFight(player); return HookResult.Stop; }
            if (_config!.Commands["Help"].Aliases.Contains(message)) { _commandHandler!.ShowHelp(player); return HookResult.Stop; }
            if (_config!.Commands["Curses"].Aliases.Contains(message)) { _commandHandler!.ShowCurses(player); return HookResult.Stop; }
            return HookResult.Continue;
        }

        public override void Unload(bool hotReload)
        {
            _chickenManager?.DespawnAll();
        }
    }
}