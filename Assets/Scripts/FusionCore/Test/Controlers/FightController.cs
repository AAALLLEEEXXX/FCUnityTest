using System;
using System.Collections.Generic;
using FusionCore.Test.Data;
using FusionCore.Test.Views;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FusionCore.Test.Models
{
    public class FightController : IDisposable
    {
        private readonly CharacterPreset[] _characters;

        private readonly FightService _fightService;

        private readonly GameModel _gameModel;

        private readonly ModifierCharacterPreset _modifierCharacterPreset;
        private readonly ModifierWeaponPreset _modifierWeaponPreset;
        private readonly List<Character> _spawnCharacters = new List<Character>();

        private readonly SpawnPoint[] _spawnPoints;

        public FightController(FightService fightService, GameModel gameModel,
            SpawnPoint[] spawnPoints, CharacterPreset[] characters,
            ModifierCharacterPreset modifierCharacterPreset, ModifierWeaponPreset modifierWeaponPreset)
        {
            _spawnPoints = spawnPoints;
            _characters = characters;
            _gameModel = gameModel;
            _fightService = fightService;
            _modifierCharacterPreset = modifierCharacterPreset;
            _modifierWeaponPreset = modifierWeaponPreset;

            _gameModel.CurrentGameState.SubscribeOnChange(OnChangeGameState);
            OnChangeGameState(GameState.MainMenu);
        }

        public void Dispose()
        {
            _gameModel.CurrentGameState.UnSubscriptionOnChange(OnChangeGameState);
        }

        private void OnChangeGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    ClearBattlefield();
                    break;

                case GameState.Fight:
                    SpawnCharactersInBattlefield(_spawnPoints, _characters);
                    break;

                case GameState.EndFight:
                    break;
            }
        }

        public void Update()
        {
            if (_gameModel.CurrentGameState.Value != GameState.Fight)
                return;

            for (var i = 0; i < _spawnCharacters.Count; i++)
                _spawnCharacters[i].Update();
        }

        private void SpawnCharactersInBattlefield(SpawnPoint[] spawnPoints, CharacterPreset[] characters)
        {
            foreach (var positionsPair in spawnPoints)
            {
                foreach (var spawn in positionsPair.PointsView)
                {
                    var index = Random.Range(0, characters.Length);
                    var spawnCharacter = CreateCharacter(characters[index], spawn.transform.position, positionsPair.Team);
                    RefreshColorHealthBar(spawnCharacter.Model);

                    spawnCharacter.Model.Armor.SubscribeOnChange(armor =>
                    {
                        RefreshArmorView(spawnCharacter, armor);

                        if (!CheckTeamAlive(spawnCharacter.Model.Team))
                            _gameModel.CurrentGameState.Value = GameState.EndFight;
                    });

                    spawnCharacter.Model.Health.SubscribeOnChange(health =>
                    {
                        RefreshHealthView(spawnCharacter, health);

                        if (!CheckTeamAlive(spawnCharacter.Model.Team))
                            _gameModel.CurrentGameState.Value = GameState.EndFight;
                    });

                    _spawnCharacters.Add(spawnCharacter);
                }
            }
            
            _fightService.UpdateSpawnCharacter(_spawnCharacters);
        }

        private void RefreshArmorView(Character character, float armor)
        {
            var maxArmor = character.Model.MaxArmor;
            character.Model.CharacterView.CharacterUiView.FillArmor = armor / maxArmor;
        }

        private void RefreshHealthView(Character character, float health)
        {
            var maxHealth = character.Model.MaxHealth;
            character.Model.CharacterView.CharacterUiView.FillHealth = health / maxHealth;
        }

        private bool CheckTeamAlive(Team team)
        {
            var teamAlive = false;

            foreach (var character in _spawnCharacters)
            {
                if (character.Model.Team != team)
                    continue;

                if (character.Model.IsAlive)
                    teamAlive = true;
            }

            return teamAlive;
        }

        private void ClearBattlefield()
        {
            foreach (var character in _spawnCharacters)
            {
                character.Model.Armor.UnSubscriptionOnChange(armor => RefreshArmorView(character, armor));
                character.Model.Health.UnSubscriptionOnChange(health => RefreshHealthView(character, health));
                character.Dispose();

                Object.Destroy(character.Model.CharacterView.gameObject);
            }

            _spawnCharacters.Clear();
        }

        private Character CreateCharacter(CharacterPreset preset, Vector3 position, Team team)
        {
            var character = Object.Instantiate(preset.CharacterView, position, Quaternion.identity);
            var characterModifierController = new CharacterModifierController(_modifierCharacterPreset, preset);
            var characterModel = new CharacterModel(characterModifierController, character, team);

            var weaponController = new WeaponController(character.WeaponView, _modifierWeaponPreset);

            return new Character(characterModel, weaponController, _fightService);
        }

        private void RefreshColorHealthBar(ICharacterModel model)
        {
            model.CharacterView.CharacterUiView.ColorHealthBar = model.Team == Team.Player ? Color.green : Color.red;
        }
    }
}