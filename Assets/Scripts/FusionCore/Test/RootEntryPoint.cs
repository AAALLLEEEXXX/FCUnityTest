using System.Collections.Generic;
using FusionCore.Test.Models;
using FusionCore.Test.Views;
using FusionCore.Ui;
using UniRx;
using UnityEngine;

namespace FusionCore.Test
{
	public class RootEntryPoint : MonoBehaviour
	{
		[SerializeField] 
		private MainMenuView _mainMenuView;
		
		[SerializeField]
		private SpawnPoint[] _spawns;
		
		
		
		[SerializeField]
		private CharacterPrefab[] _characters;
		
		private Battlefield _battlefield;
		
		private readonly CompositeDisposable _disposables = new CompositeDisposable();

		private GameModel _gameModel;

		private void Awake()
		{
			_gameModel = new GameModel();
		}

		public void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			new MainMenuController(_mainMenuView, _gameModel).AddTo(_disposables);
			new FightController(_gameModel).AddTo(_disposables);
			
			
			_battlefield = new Battlefield(_spawns);
			_battlefield.Start(_characters);
			
		}

		public void Update()
		{
			_battlefield.Update(Time.deltaTime);
		}

		private void OnDestroy()
		{
			_disposables.Clear();
		}
	}
}