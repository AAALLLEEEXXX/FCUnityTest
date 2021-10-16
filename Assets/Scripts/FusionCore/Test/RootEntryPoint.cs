using FusionCore.Test.Data;
using FusionCore.Test.Models;
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

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private FightController _fightController;
        private FightService _fightService;

        private GameModel _gameModel;

        private void Awake()
        {
            _gameModel = new GameModel();
            _fightService = new FightService();
        }

        public void Start()
        {
            Initialize();
        }

        public void Update()
        {
            _fightController.Update();
        }

        private void OnDestroy()
        {
            _disposables.Clear();
        }

        private void Initialize()
        {
            new MainMenuController(_mainMenuView, _gameModel).AddTo(_disposables);
            _fightController = new FightController(_fightService, _gameModel, _spawns, _characters,
                _modifierCharacterPreset, _modifierWeaponPreset).AddTo(_disposables);
        }
    }
}