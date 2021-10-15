using System;
using System.Collections.Generic;
using FusionCore.Test.Data;
using FusionCore.Ui;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FusionCore.Test.Models
{
	public class FightController : IDisposable
	{
		private readonly CompositeDisposable _disposables = new CompositeDisposable();
		
		private List<Character> _spawnCharacters = new List<Character>();

		private SpawnPoint[] _spawnPoints;
		private CharacterPreset[] _characters;
		
		private GameModel _gameModel;
		
		private FightService _fightService;

		public FightController(FightService fightService, GameModel gameModel, 
			SpawnPoint[] spawnPoints, CharacterPreset[] characters)
		{
			_spawnPoints = spawnPoints;
			_characters = characters;
			_gameModel = gameModel;
			_fightService = fightService;

			_gameModel.CurrentGameState.SubscribeOnChange(OnChangeGameState);
			OnChangeGameState(GameState.MainMenu);
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
			var maxArmor = character.Model.CharacterPreset.MaxArmor;
			character.Model.CharacterView.CharacterUiView.FillArmor = armor / maxArmor;
		}
		
		private void RefreshHealthView(Character character, float health)
		{
			var maxHealth = character.Model.CharacterPreset.MaxHealth;
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
			var characterModel = new CharacterModel(preset, character, team);
			return new Character(characterModel, new WeaponController(character.WeaponView), _fightService);
		}

		public void Dispose()
		{
			_gameModel.CurrentGameState.UnSubscriptionOnChange(OnChangeGameState);
			_disposables.Clear();
		}
	}
}