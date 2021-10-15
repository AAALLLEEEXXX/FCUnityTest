namespace FusionCore.Test.CharacterState
{
    public class IdleCharacterState : BaseCharacterState
    {
        private readonly CharacterModel _model;
        
        public IdleCharacterState(CharacterModel model)
        {
            _model = model;
        }
        
        public override void Something()
        {
            if (_model.CurrentTarget.Value == null)
                return;
            
            _model.CharacterView.Animator.SetBool(Aiming, false);
            _model.CharacterView.Animator.SetBool(Reloading, false);
			
            _model.PersonState.Value = PersonState.Aiming;
            _time = _model.AimTime;
            _model.CharacterView.transform.LookAt(_model.CurrentTarget.Value.Position);
        }
    }
}
