namespace FusionCore.Test.CharacterState
{
    public class IdleCharacterState : BaseCharacterState
    {
        private readonly CharacterModel _model;
        private readonly FightService _fightService;
        
        public IdleCharacterState(CharacterModel model, FightService fightService)
        {
            _model = model;
            _fightService = fightService;
            
            _time = _model.AimTime;
        }
        
        public override void Something()
        {
            if (!_fightService.TryGetNearestAliveEnemyNew(_model, out var target))
                return;
            
            _model.CurrentTarget.Value = target;

            _model.CharacterView.Animator.SetBool(Aiming, false);
            _model.CharacterView.Animator.SetBool(Reloading, false);
			
            _model.PersonState.Value = PersonState.Aiming;
            _model.CharacterView.transform.LookAt(_model.CurrentTarget.Value.Position);
        }
    }
}
