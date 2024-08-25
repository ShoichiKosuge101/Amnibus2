using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Controller;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Manager
{
    public class EnemyManager
    {
        /// <summary>
        /// 敵リスト
        /// </summary>
        public List<EnemyController> Enemies { get; private set; }= new List<EnemyController>();
        public List<EnemyController> AttackEnemies { get; private set; }= new List<EnemyController>();
        
        /// <summary>
        /// 敵かどうか
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsEnemy(Vector3 position)
        {
            return Enemies.Any(enemy => enemy.transform.position == position);
        }
        
        public bool IsEnemyNextPosition(Vector2 nextPosition)
        {
            return Enemies.Any(enemy => enemy.NextPosition == nextPosition);
        }
        
        public void AddEnemy(EnemyController enemy)
        {
            Enemies.Add(enemy);
        }
        
        public void RemoveEnemy(EnemyController enemy)
        {
            Enemies.Remove(enemy);
        }
        
        public void AddAttackEnemy(EnemyController enemy)
        {
            AttackEnemies.Add(enemy);
        }
        
        public void ClearAttackEnemy()
        {
            AttackEnemies.Clear();
        }
        
        /// <summary>
        /// 指定位置の敵を取得
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public EnemyController GetAttackTarget(Vector3 position)
        {
            return Enemies.FirstOrDefault(enemy => enemy.transform.position == position);
        }
    }
}