using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using System;

namespace FunChicken
{
    public class CommandHandler
    {
        private readonly Config _config;
        private readonly PermissionSystem _permissions;
        private readonly ChickenManager _chickenManager;
        private readonly FunChicken _plugin;

        public CommandHandler(Config config, PermissionSystem permissions, ChickenManager chickenManager, FunChicken plugin)
        {
            _config = config;
            _permissions = permissions;
            _chickenManager = chickenManager;
            _plugin = plugin;
        }

        public bool HandleCommand(CCSPlayerController player, string commandName, Action commandAction)
        {
            if (!_config.Commands.TryGetValue(commandName, out var cmdConfig) || !cmdConfig.Enabled)
                return false;

            if (!_permissions.HasPermission(player, commandName, out string denyReason))
            {
                player.PrintToChat($"[ChickenPet] Access denied: {denyReason}");
                return false;
            }

            commandAction();
            _permissions.ApplyCooldown(player, commandName);
            return true;
        }

        public void SpawnPet(CCSPlayerController player)
        {
            HandleCommand(player, "SpawnChickenPet", () =>
            {
                var pawn = player.PlayerPawn.Value;
                if (pawn?.AbsOrigin == null) return;

                if (_chickenManager.FollowingPets.ContainsKey(player))
                {
                    _chickenManager.FollowingPets[player].Remove();
                    _chickenManager.FollowingPets.Remove(player);
                }

                var chicken = _chickenManager.SpawnChicken(pawn.AbsOrigin.X + 50, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + 20);
                if (chicken != null)
                {
                    _chickenManager.FollowingPets[player] = chicken;
                    player.PrintToChat("[ChickenPet] Spawned your following chicken pet!");
                }
            });
        }

        public void SpawnNormalChicken(CCSPlayerController player)
        {
            HandleCommand(player, "SpawnNormalChicken", () =>
            {
                if (_chickenManager.NormalChickens.Count >= _chickenManager.MaxNormalChickens)
                {
                    player.PrintToChat($"[ChickenPet] Max chickens ({_chickenManager.MaxNormalChickens}) reached!");
                    return;
                }

                var pawn = player.PlayerPawn.Value;
                if (pawn?.AbsOrigin == null) return;

                var chicken = _chickenManager.SpawnChicken(pawn.AbsOrigin.X + 50, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + 10);
                if (chicken != null)
                {
                    _chickenManager.NormalChickens.Add(chicken);
                    player.PrintToChat($"[ChickenPet] Spawned a normal chicken! ({_chickenManager.NormalChickens.Count}/{_chickenManager.MaxNormalChickens})");
                }
            });
        }

        public void DespawnPet(CCSPlayerController player)
        {
            HandleCommand(player, "KillChicken", () =>
            {
                if (_chickenManager.FollowingPets.ContainsKey(player))
                {
                    _chickenManager.FollowingPets[player].Remove();
                    _chickenManager.FollowingPets.Remove(player);
                    player.PrintToChat("[ChickenPet] Your following chicken has been despawned!");
                }
                else
                {
                    player.PrintToChat("[ChickenPet] You have no following chicken to despawn!");
                }
            });
        }

        public void PetJump(CCSPlayerController player)
        {
            HandleCommand(player, "PetJump", () =>
            {
                if (_chickenManager.FollowingPets.TryGetValue(player, out var pet) && pet.IsValid && pet.AbsOrigin != null)
                {
                    float originalZ = pet.AbsOrigin.Z;
                    float jumpHeight = _config.Commands["PetJump"].JumpHeight ?? 100f;
                    pet.AbsOrigin.Z += jumpHeight;
                    player.PrintToChat("[ChickenPet] Your following chicken jumped!");

                    _plugin.AddTimer(0.5f, () =>
                    {
                        if (pet.IsValid && pet.AbsOrigin != null) pet.AbsOrigin.Z = originalZ;
                    });
                }
                else
                {
                    player.PrintToChat("[ChickenPet] You have no following chicken to make jump!");
                }
            });
        }

        public void ChickenFight(CCSPlayerController player)
        {
            HandleCommand(player, "ChickenFight", () =>
            {
                if (_chickenManager.ActiveFightPlayer != null)
                {
                    player.PrintToChat("[ChickenPet] Chicken fight already in progress!");
                    return;
                }

                var pawn = player.PlayerPawn.Value;
                if (pawn?.AbsOrigin == null) return;

                _chickenManager.ActiveFightPlayer = player;
                var chickens = new List<CBaseEntity>();
                int chickenCount = _config.Commands["ChickenFight"].ChickenCount ?? 100;
                float duration = _config.Commands["ChickenFight"].DurationSeconds ?? 10f;

                for (int i = 0; i < chickenCount; i++)
                {
                    float angle = (float)(2 * Math.PI * i / chickenCount);
                    float radius = 100.0f;
                    var chicken = _chickenManager.SpawnChicken(
                        pawn.AbsOrigin.X + radius * (float)Math.Cos(angle),
                        pawn.AbsOrigin.Y + radius * (float)Math.Sin(angle),
                        pawn.AbsOrigin.Z + 10.0f
                    );
                    if (chicken != null) chickens.Add(chicken);
                }

                _chickenManager.FightChickens[player] = chickens;
                player.PrintToChat($"[ChickenPet] Chicken fight started! Kill {chickenCount} chickens in {duration} seconds!");

                _plugin.AddTimer(duration, () =>
                {
                    if (_chickenManager.FightChickens.TryGetValue(player, out var fightChickens))
                    {
                        int remaining = fightChickens.Count(c => c.IsValid);
                        int killed = chickenCount - remaining;
                        foreach (var chicken in fightChickens)
                            if (chicken.IsValid) chicken.Remove();
                        _chickenManager.FightChickens.Remove(player);
                        _chickenManager.ActiveFightPlayer = null;

                        player.PrintToChat($"[ChickenPet] Fight ended! You killed {killed} chickens!");
                    }
                });
            });
        }

        public void ShowHelp(CCSPlayerController player)
        {
            HandleCommand(player, "Help", () =>
            {
                player.PrintToChat("[FunChicken] Available Commands:");
                foreach (var cmd in _config.Commands)
                {
                    if (cmd.Value.Enabled)
                    {
                        string aliases = string.Join(", ", cmd.Value.Aliases);
                        player.PrintToChat($"{aliases} - {cmd.Value.Description}");
                    }
                }
            });
        }

        public void ShowCurses(CCSPlayerController player)
        {
            HandleCommand(player, "Curses", () =>
            {
                string[] curseLines = _config.MyCurses.Split('\n');
                foreach (var line in curseLines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        player.PrintToChat($"[ChickenPet] {line}");
                }
            });
        }
    }
}