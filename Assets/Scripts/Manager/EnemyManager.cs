using System.Collections.Generic;
using Controller;

namespace Manager
{
    public class EnemyManager
    {
        /// <summary>
        /// 敵リスト
        /// </summary>
        public List<EnemyController> Enemies { get; private set; }= new List<EnemyController>();
        
        public void AddEnemy(EnemyController enemy)
        {
            Enemies.Add(enemy);
        }
        
        public void RemoveEnemy(EnemyController enemy)
        {
            Enemies.Remove(enemy);
        }
    }
}