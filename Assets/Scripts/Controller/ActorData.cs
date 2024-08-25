using UnityEngine;

namespace Controller
{
    /// <summary>
    /// キャラデータ
    /// </summary>
    [CreateAssetMenu(fileName = "ActorData", menuName = "Scriptable Objects/ActorData")]
    public class ActorData
        : ScriptableObject
    {
        [SerializeField] 
        private int _hp;
        public int Hp => _hp;
        
        [SerializeField] 
        private int _attack;
        public int Attack => _attack;
    }
}