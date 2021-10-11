using System;
using System.Collections.Generic;
using FusionCore.Test.Data;
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

		public FightController(GameModel gameModel, FightService fightService, SpawnPoint[] spawnPoints, CharacterPreset[] characters)
		{
			_spawnPoints = spawnPoints;
			_characters = characters;
			_gameModel = gameModel;
			_fightService = fightService;

			gameModel.CurrentGameState.Subscribe(OnChangeGameState).AddTo(_disposables);
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
			}
		}

		public void Update()
		{
			if (_gameModel.CurrentGameState.Value != GameState.Fight) 
				return;
			
			foreach (var character in _fightService.SpawnCharacters)
				character.Update();
		}
		
		private void SpawnCharactersInBattlefield(SpawnPoint[] spawnPoints, CharacterPreset[] characters)
		{
			foreach (var positionsPair in spawnPoints)
			{
				foreach (var spawn in positionsPair.PointsView)
				{
					var index = Random.Range(0, characters.Length);
					var spawnCharacter = CreateCharacterAt(characters[index], spawn.transform.position, positionsPair.Team);
					_spawnCharacters.Add(spawnCharacter);
				}
			}
		}

		private void ClearBattlefield()
		{
			foreach (var character in _spawnCharacters)
			{
				character.Dispose();
				Object.Destroy(character.Model.CharacterView);
			}
			
			_spawnCharacters.Clear();
		}
		
		private Character CreateCharacterAt(CharacterPreset preset, Vector3 position, Team team)
		{
			var character = Object.Instantiate(preset.CharacterView, position, Quaternion.identity);
			var characterModel = new CharacterModel(preset, team);
			return new Character(characterModel, new Weapon(character.WeaponView));
		}

		public void Dispose()
		{
			_disposables.Clear();
		}
	}
}