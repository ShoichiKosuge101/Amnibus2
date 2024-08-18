namespace Controller.Logic.State
{
    public class EnemyMoveState
        : StateBase
    {
        public EnemyMoveState(in StateManager owner) : base(in owner)
        {
        }
        
        public override void OnEnter()
        {
            // TODO: 敵の移動処理
            
            // 移動終了後、ターン終了Stateに遷移
            Owner.ChangeState(Owner.TurnEndState);
        }
    }
}