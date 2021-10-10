using System;
using System.Collections.Generic;
using FusionCore.Test.Views;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FusionCore.Test.Models
{
	public class FightController : IDisposable
	{
		private readonly CompositeDisposable _disposables = new CompositeDisposable();
		
		
		private readonly Dictionary<uint, List<Character>> _charactersByTeam;

		private SpawnPoint[] _spawnPoints;
		private CharacterView[] _characters;
		private GameModel _gameModel;

		public FightController(GameModel gameModel, SpawnPoint[] spawnPoints, CharacterView[] characters)
		{
			_spawnPoints = spawnPoints;
			_characters = characters;
			_gameModel = gameModel;

			_charactersByTeam = new Dictionary<uint, List<Character>>();
			
			gameModel.CurrentGameState.Subscribe(OnChangeGameState).AddTo(_disposables);
			OnChangeGameState(GameState.MainMenu);
		}
		
		private void OnChangeGameState(GameState gameState)
		{
			switch (gameState)
			{
				case GameState.MainMenu:
					// очищаем поле от игроков
					break;
				
				case GameState.Fight:
					Start();
					break;
                
				
				case GameState.EndFight:
					break;
			}
		}

		private void Start()
		{
			_charactersByTeam.Clear();

			foreach (var positionsPair in _spawnPoints)
			{
				var positions = positionsPair.PointsView;
				var characters = new List<Character>();
				_charactersByTeam.Add(positionsPair.Team, characters);
				
				var i = 0;
				while (i < positions.Length && _characters.Length > 0)
				{
					var index = Random.Range(0, _characters.Length);
					characters.Add(CreateCharacterAt(_characters[index], this, positions[i].transform.position));
					i++;
				}
			}
		}

		public bool TryGetNearestAliveEnemy(Character character, out Character target)
		{
			if (TryGetTeam(character, out uint team))
			{
				Character nearestEnemy = null;
				var nearestDistance = float.MaxValue;
				var enemies = team == 1 ? _charactersByTeam[2] : _charactersByTeam[1];
				
				foreach (var enemy in enemies)
				{
					if (enemy.IsAlive)
					{
						float distance = Vector3.Distance(character.Position, enemy.Position);
						if (distance < nearestDistance)
						{
							nearestDistance = distance;
							nearestEnemy = enemy;
						}
					}
				}
				target = nearestEnemy;
				return target != null;
			}
			target = default;
			return false;
		}

		private bool TryGetTeam(Character target, out uint team)
		{
			foreach (var charactersPair in _charactersByTeam)
			{
				var characters = charactersPair.Value;
				
				foreach (var character in characters)
				{
					if (character == target)
					{
						team = charactersPair.Key;
						return true;
					}
				}
			}
			team = default;
			return false;
		}

		public void Update()
		{
			if (_gameModel.CurrentGameState.Value == GameState.Fight)
			{
				foreach (var charactersPair in _charactersByTeam)
				{
					var characters = charactersPair.Value;
					
					foreach (var character in characters)
						character.Update(Time.deltaTime);
				}
			}
		}

		private Character CreateCharacterAt(CharacterView view, FightController fightController, Vector3 position)
		{
			var character = Object.Instantiate(view, position, Quaternion.identity);
			return new Character(character, new Weapon(character.WeaponView), fightController);
		}

		public void Dispose()
		{
			_disposables.Clear();
		}
	}
}