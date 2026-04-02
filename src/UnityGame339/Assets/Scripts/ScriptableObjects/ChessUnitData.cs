using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewChessUnit", menuName = "Chess Unit")]
    public class ChessUnitData : ScriptableObject
    {
        [Header("Stats")]
        public string unitName;
        public int health;
        public int damageDice;
    }
}