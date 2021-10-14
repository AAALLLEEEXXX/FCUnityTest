using System.Collections.Generic;
using FusionCore.Test.Models;

namespace FusionCore.Test
{
    public class FightService
    {
        private List<Character> _spawnCharacters;
        
        public void UpdateSpawnCharacter(List<Character> spawnCharacters)
        {
            _spawnCharacters = spawnCharacters;
        }

        public bool TryGetNearestAliveEnemyNew(CharacterModel character, out CharacterModel target)
        {
            foreach (var spawnCharacter in _spawnCharacters)
            {
                if (character.Team != spawnCharacter.Model.Team && spawnCharacter.Model.IsAlive)
                {
                    target = spawnCharacter.Model;
                    return true;
                }
            }
            
            target = null;
            return false;
        }
    }
}
