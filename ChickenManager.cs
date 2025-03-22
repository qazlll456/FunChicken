using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using CounterStrikeSharp.API;
using System.Linq;

namespace FunChicken
{
    public class ChickenManager
    {
        private readonly Config _config;
        private readonly Dictionary<CCSPlayerController, CBaseEntity> _followingPets = new();
        private readonly List<CBaseEntity> _normalChickens = new();
        private readonly Dictionary<CCSPlayerController, List<CBaseEntity>> _fightChickens = new();
        public CCSPlayerController? ActiveFightPlayer { get; set; }
        private const int MAX_NORMAL_CHICKENS = 100;

        public ChickenManager(Config config)
        {
            _config = config;
        }

        public CBaseEntity? SpawnChicken(float x, float y, float z)
        {
            var chicken = Utilities.CreateEntityByName<CBaseEntity>("chicken");
            if (chicken == null || chicken.AbsOrigin == null) return null;

            chicken.AbsOrigin.X = x;
            chicken.AbsOrigin.Y = y;
            chicken.AbsOrigin.Z = z;
            chicken.DispatchSpawn();
            return chicken;
        }

        public void UpdatePetPosition(CCSPlayerController player, CBaseEntity pet)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn?.AbsOrigin == null || pet.AbsOrigin == null) return;

            float targetX = pawn.AbsOrigin.X - 40.0f;
            float targetY = pawn.AbsOrigin.Y;
            float baseZ = pawn.AbsOrigin.Z; // Base height is player's ground level
            float distance = VectorDistance(pet.AbsOrigin, targetX, targetY, baseZ);

            if (distance > _config.PetMaxDistance)
            {
                pet.AbsOrigin.X = targetX;
                pet.AbsOrigin.Y = targetY;
                pet.AbsOrigin.Z = baseZ + 20.0f; // Reset to base height
                // Debug log removed: Server.PrintToConsole($"[ChickenPet] Teleported pet for {player.PlayerName} due to distance");
            }
            else if (distance > 50.0f) // Extended range
            {
                float speed = 300.0f * (8 / 64.0f);
                float moveDistance = Math.Min(distance, speed);
                float deltaX = targetX - pet.AbsOrigin.X;
                float deltaY = targetY - pet.AbsOrigin.Y;
                pet.AbsOrigin.X += (deltaX / distance) * moveDistance;
                pet.AbsOrigin.Y += (deltaY / distance) * moveDistance;
                // Add jumping while moving
                float time = (float)Server.CurrentTime;
                float jumpHeight = 20.0f; // Max height of jump
                float jumpFrequency = 1.5f; // How fast it jumps
                pet.AbsOrigin.Z = baseZ + jumpHeight * (float)Math.Sin(time * jumpFrequency);
            }
            else
            {
                // When close to the player, still apply jumping
                float time = (float)Server.CurrentTime;
                float jumpHeight = 20.0f;
                float jumpFrequency = 1.5f;
                pet.AbsOrigin.Z = baseZ + jumpHeight * (float)Math.Sin(time * jumpFrequency);
            }
        }

        private float VectorDistance(Vector vec, float x, float y, float z) =>
            (float)Math.Sqrt(Math.Pow(vec.X - x, 2) + Math.Pow(vec.Y - y, 2));

        public void CleanupChickens()
        {
            _normalChickens.RemoveAll(c => !c.IsValid);
            var invalidPets = _followingPets.Where(kvp => !kvp.Key.IsValid || !kvp.Value.IsValid).ToList();
            foreach (var kvp in invalidPets) _followingPets.Remove(kvp.Key);
            foreach (var fight in _fightChickens)
                fight.Value.RemoveAll(c => !c.IsValid);
        }

        public void DespawnAll()
        {
            foreach (var pet in _followingPets.Values)
                pet?.Remove();
            _followingPets.Clear();

            foreach (var chicken in _normalChickens)
                chicken?.Remove();
            _normalChickens.Clear();

            foreach (var fight in _fightChickens.Values)
                foreach (var chicken in fight)
                    chicken?.Remove();
            _fightChickens.Clear();

            ActiveFightPlayer = null;
        }

        public Dictionary<CCSPlayerController, CBaseEntity> FollowingPets => _followingPets;
        public List<CBaseEntity> NormalChickens => _normalChickens;
        public Dictionary<CCSPlayerController, List<CBaseEntity>> FightChickens => _fightChickens;
        public int MaxNormalChickens => MAX_NORMAL_CHICKENS;
    }
}