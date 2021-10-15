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

		private FightWindowView _fightWindowView;
		private FightService _fightService;

		public FightController(FightWindowView fightWindowView, FightService fightService, GameModel gameModel, 
			SpawnPoint[] spawnPoints, CharacterPreset[] characters)
		{
			_spawnPoints = spawnPoints;
			_characters = characters;
			_gameModel = gameModel;
			_fightWindowView = fightWindowView;
			_fightService = fightService;

			_gameModel.CurrentGameState.SubscribeOnChange(OnChangeGameState);
			OnChangeGameState(GameState.MainMenu);
		}
		
		private void OnChangeGameState(GameState gameState)
		{
			switch (gameState)
			{
				case GameState.MainMenu:
					_fightWindowView.gameObject.SetActive(false);
					ClearBattlefield();
					break;
				
				case GameState.Fight:
					SpawnCharactersInBattlefield(_spawnPoints, _characters);
					
					_fightWindowView.gameObject.SetActive(true);
					RefreshFightView();
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
					
					spawnCharacter.Model.Armor.SubscribeOnChange(_ =>
					{
						RefreshArmorView();

						if (!CheckTeamAlive(spawnCharacter.Model.Team))
							_gameModel.CurrentGameState.Value = GameState.EndFight;
					});
					
					spawnCharacter.Model.Health.SubscribeOnChange(_ =>
					{
						RefreshHealthView();

						if (!CheckTeamAlive(spawnCharacter.Model.Team))
							_gameModel.CurrentGameState.Value = GameState.EndFight;
					});
					
					_spawnCharacters.Add(spawnCharacter);
				}
			}
			
			_fightService.UpdateSpawnCharacter(_spawnCharacters);
		}

		private void RefreshFightView()
		{
			RefreshArmorView();
			RefreshHealthView();
		}

		private void RefreshArmorView()
		{
			var dataEnemy = CalculationAmountAndHealth(Team.Enemy);
			_fightWindowView.IndicatorEnemy.SetDataArmor(dataEnemy.armor);
			
			var dataPlayer = CalculationAmountAndHealth(Team.Player);
			_fightWindowView.IndicatorPlayer.SetDataArmor(dataPlayer.armor);
		}
		
		private void RefreshHealthView()
		{
			var dataEnemy = CalculationAmountAndHealth(Team.Enemy);
			_fightWindowView.IndicatorEnemy.SetDataHealth(dataEnemy.health);
			
			var dataPlayer = CalculationAmountAndHealth(Team.Player);
			_fightWindowView.IndicatorPlayer.SetDataHealth(dataPlayer.health);
		}
		
		private (float armor, float health) CalculationAmountAndHealth(Team team)
		{
			var countArmor = 0.0f;
			var countHealth = 0.0f;
			
			foreach (var character in _spawnCharacters)
			{
				if (character.Model.Team == team)
				{
					countArmor += character.Model.Armor.Value;
					countHealth += character.Model.Health.Value;
				}
			}
			
			return (countArmor, countHealth);
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
				character.Model.Armor.UnSubscriptionOnChange(_ => RefreshArmorView());
				character.Model.Health.UnSubscriptionOnChange(_ => RefreshHealthView());
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