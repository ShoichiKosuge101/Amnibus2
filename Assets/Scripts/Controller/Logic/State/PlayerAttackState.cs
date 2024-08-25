using UnityEngine;

namespace Controller.Logic.State
{
    public class PlayerAttackState
        : StateBase
    {
        public PlayerAttackState(in StateManager owner) : base(in owner)
        {
        }

        public override void OnEnter()
        {
            // TODO: FIXME 移動先は戻しているのでここでは確実に取れない
            var enemy = Owner.MapManager.EnemyManager.GetAttackTarget(Owner.MapManager.PlayerController.NextPosition);
            // 対象を取って、攻撃処理を実行
            if (enemy != null)
            {
                Owner.MapManager.PlayerController.Attack(enemy);
            }
            
            // 敵のターンに移動
            Owner.ChangeState(Owner.EnemyMoveState);
        }
    }
}