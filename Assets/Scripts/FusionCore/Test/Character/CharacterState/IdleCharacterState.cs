namespace FusionCore.Test.CharacterState
{
    public class IdleCharacterState : BaseCharacterState
    {
        private readonly ICharacterModel _model;
        private readonly FightService _fightService;
        
        public IdleCharacterState(ICharacterModel model, FightService fightService)
        {
            _model = model;
            _fightService = fightService;
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
            
            _time = _model.AimTime;
        }
    }
}
