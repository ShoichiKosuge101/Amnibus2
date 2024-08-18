namespace Controller.Logic.State
{
    public class TurnEndState
        : StateBase
    {
        public TurnEndState(in StateManager owner) : base(in owner)
        {
        }

        public override void OnEnter()
        {
            // ゴールしていなければ、次のターンへ
            if (!Owner.IsGoalReached)
            {
                Owner.ChangeState(Owner.PlayerInputState);
            }
            else
            {
                // ゴールしていれば、ゲームクリア
                Owner.ChangeScene("Title");
            }
        }
    }
}