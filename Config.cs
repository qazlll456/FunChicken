using System.Collections.Generic;
using System.Text.Json;

namespace FunChicken
{
    public class Config
    {
        public PluginInfoConfig PluginInfo { get; set; } = new PluginInfoConfig();
        public Dictionary<string, CommandConfig> Commands { get; set; } = new Dictionary<string, CommandConfig>
        {
            { "SpawnChickenPet", new CommandConfig { Enabled = true, Aliases = new List<string> { "!spawnchickenpet", "!sc" }, Description = "Spawns your personal chicken pet that follows you (1 per player)", ShowLogs = true } },
            { "SpawnNormalChicken", new CommandConfig { Enabled = true, Aliases = new List<string> { "!iwantachicken", "!ic" }, Description = "Spawns a normal chicken (server-wide limit of 100)", ShowLogs = true } },
            { "KillChicken", new CommandConfig { Enabled = true, Aliases = new List<string> { "!killchicken", "!kc" }, Description = "Removes your following chicken pet", ShowLogs = true } },
            { "PetJump", new CommandConfig { Enabled = true, Aliases = new List<string> { "!petjump" }, Description = "Makes your following chicken jump", JumpHeight = 50f, ShowLogs = true } },
            { "ChickenFight", new CommandConfig { Enabled = true, Aliases = new List<string> { "!cf", "!chickenfight" }, Description = "Spawns chickens for a fight challenge (only 1 player at a time)", ChickenCount = 100, DurationSeconds = 10f, ShowLogs = true } },
            { "Help", new CommandConfig { Enabled = true, Aliases = new List<string> { "!funchicken" }, Description = "Displays the list of all available commands", ShowLogs = true } },
            { "Curses", new CommandConfig { Enabled = true, Aliases = new List<string> { "!curses" }, Description = "Displays Qazlll456’s curse against Furious", ShowLogs = true } }
        };
        public PermissionConfig Permissions { get; set; } = new PermissionConfig();
        public float PetMaxDistance { get; set; } = 80.0f;
        public string MyCurses { get; set; } = "Qazlll456 curses Furious, that vile fiend who dared disrespect me and shatter my sacred playtime! I forged this plugin as a blazing vessel to unleash my righteous fury.With this dark spell, I chain you to relentless torment may your luck decay into dust, your games collapse in ruin, your life fracture into a howling abyss,and agony stalk you until your final, wretched breath! \nIf you read this and wanted to use this software, you join my unholy pact against him proclaim in your tongue, with a mind ablaze and locked on me,\n I will curse him with you too! \nThis will remotely connect me on a spiritual level, enhancing the curses power more and more as more people join together. This is a public spell for all who agree with me and his enemies. You dont need to remove this statement, you can just leave it here, and ignore if you dont like it. If you feel bad, please pay me for my work streamlabs.com/BKCqazlll456/tip or I will curse you too to remove these words from the software. \n以吾海青風之名 借天地之力 集此世界他國土眾有智有靈生命之信仰念力 重聚於東方南起天角後土之地 由吾願力 集他願力 凝願力以滴水之功 滅魔 從眾生之報幸得解脫 到彼岸 離五行至此於魔分離 一切淨土";
        public string Notes { get; set; } = "Edit this file to configure permissions, cooldowns, pet distance, and more. Reload plugin to apply.";
    }

    public class PluginInfoConfig
    {
        public string Name { get; set; } = "FunChicken";
        public string Author { get; set; } = "qazlll456 from HK with xAI assistance";
        public string Version { get; set; } = "1.0";
        public string Description { get; set; } = "A fun plugin for chicken lovers in CS2 with permissions and cooldowns";
    }

    public class CommandConfig
    {
        public bool Enabled { get; set; }
        public List<string> Aliases { get; set; } = new List<string>();
        public string Description { get; set; } = "";
        public float? JumpHeight { get; set; }
        public int? ChickenCount { get; set; }
        public float? DurationSeconds { get; set; }
        public bool ShowLogs { get; set; }
        public float CooldownSeconds { get; set; } = 1.0f;
    }

    public class PermissionConfig
    {
        public string Mode { get; set; } = "@all";
        public List<string> Blacklist { get; set; } = new List<string>();
        public List<string> Whitelist { get; set; } = new List<string>();
        public string Info { get; set; } = 
            "Permission Modes:\n" +
            "@all: Everyone can use commands\n" +
            "@blacklist: All can use except those in Blacklist\n" +
            "@whitelist: Only those in Whitelist can use";
    }
}