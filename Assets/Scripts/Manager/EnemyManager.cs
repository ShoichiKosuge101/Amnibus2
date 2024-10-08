﻿using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UniRx;
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
        /// <param name="enemy"></param>
        /// <returns></returns>
        public bool IsEnemy(Vector3 position, out EnemyController enemy)
        {
            enemy = Enemies.FirstOrDefault(enemy => enemy.transform.position == position);
            return enemy != null;
        }
        
        public bool IsEnemyNextPosition(Vector2 nextPosition)
        {
            return Enemies.Any(enemy => enemy.NextPosition == nextPosition);
        }
        
        public void AddEnemy(EnemyController enemy)
        {
            Enemies.Add(enemy);
            
            // 倒されたときの購読処理
            enemy
                .OnDefeatRx
                .TakeUntilDestroy(enemy)
                .Subscribe(_ =>
            {
                // 自分自身を管理から外す
                RemoveEnemy(enemy);
            });
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