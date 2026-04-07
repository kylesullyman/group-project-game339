using Game.Runtime;
using Game339.Shared.Models;
using Game339.Shared.Services;
using ScriptableObjects;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Spawn Positions")]
    [SerializeField] private Vector3 playerSpawnPosition = new Vector3(-4f, -3f, 0f);
    [SerializeField] private Vector3 enemySpawnPosition  = new Vector3( 4f,  3f, 0f);

    [Header("Chess Unit Data")]
    [SerializeField] private ChessUnitData pawn;
    [SerializeField] private ChessUnitData knight;
    

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPawnPrefab;
    [SerializeField] private GameObject enemyPawnPrefab;

    private bool _isPlayerTurn = true;
    private bool _combatActive = false;

    private static GameState GameState      => ServiceResolver.Resolve<GameState>();
    private static IDamageService DamageSvc => ServiceResolver.Resolve<IDamageService>();

    public void StartCombat()
    {
        GameState.GoodGuy.Name.Value   = "Player";
        GameState.GoodGuy.Health.Value = 10;
        GameState.GoodGuy.Damage.Value = 2;
        GameState.GoodGuy.Armor.Value  = 0;

        GameState.BadGuy.Name.Value    = "Enemy Pawn";
        GameState.BadGuy.Health.Value = pawn.health;
        GameState.BadGuy.Damage.Value  = pawn.damage;
        GameState.BadGuy.Armor.Value   = pawn.armor;

        if (playerPawnPrefab != null)
            Instantiate(playerPawnPrefab, playerSpawnPosition, Quaternion.identity);

        if (enemyPawnPrefab != null)
            Instantiate(enemyPawnPrefab, enemySpawnPosition, Quaternion.identity);

        _isPlayerTurn = true;
        _combatActive = true;
    }

    public void PlayerAttack()
    {
        if (!_combatActive || !_isPlayerTurn) return;

        var damage = DamageSvc.CalculateDamage(GameState.GoodGuy, GameState.BadGuy);
        DamageSvc.ApplyDamage(GameState.BadGuy, damage);

        if (GameState.BadGuy.Health.Value <= 0)
        {
            _combatActive = false;
            return;
        }

        _isPlayerTurn = false;
        EnemyTakeTurn();
    }

    public void PlayerParry()
    {
        
    }

    public void PlayerSpecial()
    {
        
    }

    private void EnemyTakeTurn()
    {
        var damage = DamageSvc.CalculateDamage(GameState.BadGuy, GameState.GoodGuy);
        DamageSvc.ApplyDamage(GameState.GoodGuy, damage);

        if (GameState.GoodGuy.Health.Value <= 0)
        {
            _combatActive = false;
            return;
        }

        _isPlayerTurn = true;
    }
}
