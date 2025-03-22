using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;

namespace FunChicken
{
    public class PermissionSystem
    {
        private readonly Config _config;
        private readonly Dictionary<CCSPlayerController, Dictionary<string, DateTime>> _commandCooldowns = new();

        public PermissionSystem(Config config)
        {
            _config = config;
        }

        public bool HasPermission(CCSPlayerController player, string commandName, out string denyReason)
        {
            denyReason = "";
            if (!player.IsValid || player.SteamID == 0)
            {
                denyReason = "Invalid player";
                return false;
            }

            if (_commandCooldowns.TryGetValue(player, out var cooldowns) && 
                cooldowns.TryGetValue(commandName, out var nextUse) && 
                nextUse > DateTime.Now)
            {
                denyReason = $"Command on cooldown ({(nextUse - DateTime.Now).TotalSeconds:F1}s remaining)";
                return false;
            }

            string steamId = player.SteamID.ToString();
            switch (_config.Permissions.Mode.ToLower())
            {
                case "@all":
                    return true;
                case "@blacklist":
                    if (_config.Permissions.Blacklist.Contains(steamId))
                    {
                        denyReason = "Player is blacklisted";
                        return false;
                    }
                    return true;
                case "@whitelist":
                    if (!_config.Permissions.Whitelist.Contains(steamId))
                    {
                        denyReason = "Player not in whitelist";
                        return false;
                    }
                    return true;
                default:
                    denyReason = "Invalid permission mode in config";
                    return false;
            }
        }

        public void ApplyCooldown(CCSPlayerController player, string commandName)
        {
            if (!_config.Commands.TryGetValue(commandName, out var cmdConfig)) return;
            float cooldown = cmdConfig.CooldownSeconds;
            if (!_commandCooldowns.ContainsKey(player))
                _commandCooldowns[player] = new Dictionary<string, DateTime>();
            _commandCooldowns[player][commandName] = DateTime.Now.AddSeconds(cooldown);
        }
    }
}