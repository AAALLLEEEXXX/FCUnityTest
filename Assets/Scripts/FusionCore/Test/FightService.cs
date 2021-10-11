using System.Collections.Generic;
using FusionCore.Test.Models;
using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test
{
    public class FightService
    {
        public List<Character> SpawnCharacters { get; set; }
        
        // public bool TryGetNearestAliveEnemy(Character character, out Character target)
        // {
        //     if (TryGetTeam(character, out Team team))
        //     {
        //         Character nearestEnemy = null;
        //         var nearestDistance = float.MaxValue;
        //
        //         foreach (var enemy in SpawnCharacters)
        //         {
        //             if (team == Team.Enemy && enemy.IsAlive)
        //             {
        //                 float distance = Vector3.Distance(character.Position, enemy.Position);
        //                 if (distance < nearestDistance)
        //                 {
        //                     nearestDistance = distance;
        //                     nearestEnemy = enemy;
        //                 }
        //             }
        //         }
        //         target = nearestEnemy;
        //         return target != null;
        //     }
        //     target = default;
        //     return false;
        // }
        //
        // private bool TryGetTeam(Character target, out Team team)
        // {
        //     foreach (var charactersPair in SpawnCharacters)
        //     {
        //         if (charactersPair == target)
        //         {
        //             team = charactersPair.Team;
        //             return true;
        //         }
        //     }
        //     team = default;
        //     return false;
        // }
    }
}
