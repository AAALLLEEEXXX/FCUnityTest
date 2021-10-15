using System.Collections.Generic;
using FusionCore.Test.Data;
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
		private ModifierCharacterPreset _modifierCharacterPreset;
		
		[SerializeField] 
		private ModifierWeaponPreset _modifierWeaponPreset;

		[SerializeField]
		private SpawnPoint[] _spawns;
		
		[SerializeField]
		private CharacterPreset[] _characters;
		
		private FightController _fightController;
		
		private readonly CompositeDisposable _disposables = new CompositeDisposable();

		private GameModel _gameModel;
		private FightService _fightService;

		private void Awake()
		{
			_gameModel = new GameModel();
			_fightService = new FightService();
			
		}

		public void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			new MainMenuController(_mainMenuView, _gameModel).AddTo(_disposables);
			_fightController = new FightController(_fightService, _gameModel, _spawns, _characters, _modifierCharacterPreset, _modifierWeaponPreset).AddTo(_disposables);
		}

		public void Update()
		{
			_fightController.Update();
		}

		private void OnDestroy()
		{
			_disposables.Clear();
		}
	}
}