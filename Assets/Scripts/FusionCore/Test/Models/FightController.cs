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

			gameModel.CurrentGameState.Subscribe(OnChangeGameState).AddTo(_disposables);
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
					_fightWindowView.gameObject.SetActive(false);
					break;
			}
		}

		private void RefreshFightView()
		{
			var dataEnemy = CalculationAmountAndHealth(Team.Enemy);
			_fightWindowView.IndicatorEnemy.SetData(dataEnemy.health, dataEnemy.armor);
			
			var dataPlayer = CalculationAmountAndHealth(Team.Player);
			_fightWindowView.IndicatorPlayer.SetData(dataPlayer.health, dataPlayer.armor);
		}

		public void Update()
		{
			if (_gameModel.CurrentGameState.Value != GameState.Fight) 
				return;
			
			foreach (var character in _spawnCharacters)
				character.Update();
		}
		
		private void SpawnCharactersInBattlefield(SpawnPoint[] spawnPoints, CharacterPreset[] characters)
		{
			foreach (var positionsPair in spawnPoints)
			{
				foreach (var spawn in positionsPair.PointsView)
				{
					var index = Random.Range(0, characters.Length);
					var spawnCharacter = CreateCharacter(characters[index], spawn.transform.position, positionsPair.Team);
					
					// spawnCharacter.Model.Armor.Subscribe(_ => RefreshFightView()).AddTo(_disposables);
					// spawnCharacter.Model.Health.Subscribe(_ => RefreshFightView()).AddTo(_disposables);
					
					_spawnCharacters.Add(spawnCharacter);
				}
			}
			
			_fightService.UpdateSpawnCharacter(_spawnCharacters);
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
		
		private void ClearBattlefield()
		{
			_disposables.Clear();
			
			foreach (var character in _spawnCharacters)
			{
				character.Dispose();
				Object.Destroy(character.Model.CharacterView);
			}
			
			_spawnCharacters.Clear();
		}
		
		private Character CreateCharacter(CharacterPreset preset, Vector3 position, Team team)
		{
			var character = Object.Instantiate(preset.CharacterView, position, Quaternion.identity);
			var characterModel = new CharacterModel(preset, character, team);
			return new Character(characterModel, new Weapon(character.WeaponView), _fightService, _gameModel);
		}

		public void Dispose()
		{
			_disposables.Clear();
		}
	}
}