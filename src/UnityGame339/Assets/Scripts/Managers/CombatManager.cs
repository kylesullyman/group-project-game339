using Game.Runtime;
using Game339.Shared.Diagnostics;
using Game339.Shared.Models;
using Game339.Shared.Services;
using ScriptableObjects;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("Spawn Positions")]
    [SerializeField] private Vector3 playerSpawnPosition = new Vector3(-4f, -3f, 0f);
    [SerializeField] private Vector3 enemySpawnPosition = new Vector3(4f, 3f, 0f);

    [Header("Enemy Data")]
    [SerializeField] private ChessUnitData pawn;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPawnPrefab;
    [SerializeField] private GameObject enemyPawnPrefab;

    private bool _isPlayerTurn = true;
    private bool _combatActive;
    private bool _playerBlocking;
    private bool _enemyBlocking;

    private GameObject _playerInstance;
    private GameObject _enemyInstance;

    private static GameState GameState => ServiceResolver.Resolve<GameState>();
    private static IDamageService DamageSvc => ServiceResolver.Resolve<IDamageService>();
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    public bool IsCombatActive => _combatActive;
    public bool IsPlayerTurn => _isPlayerTurn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartCombat()
    {
        if (_combatActive) return;

        GameState.GoodGuy.Name.Value = "Player";
        GameState.GoodGuy.Health.Value = 10;
        GameState.GoodGuy.Damage.Value = 2;
        GameState.GoodGuy.Armor.Value = 0;

        GameState.BadGuy.Name.Value = "Enemy Pawn";
        GameState.BadGuy.Health.Value = pawn != null ? pawn.health : 10;
        GameState.BadGuy.Damage.Value = pawn != null ? pawn.damage : 2;
        GameState.BadGuy.Armor.Value = pawn != null ? pawn.armor : 0;

        if (playerPawnPrefab != null && _playerInstance == null)
            _playerInstance = Instantiate(playerPawnPrefab, playerSpawnPosition, Quaternion.identity);

        if (enemyPawnPrefab != null && _enemyInstance == null)
            _enemyInstance = Instantiate(enemyPawnPrefab, enemySpawnPosition, Quaternion.identity);

        _playerBlocking = false;
        _enemyBlocking = false;
        _isPlayerTurn = true;
        _combatActive = true;

        Log.Info("Combat started.");
    }

    public void PlayerAttack()
    {
        if (!_combatActive || !_isPlayerTurn) return;

        int damage = DamageSvc.CalculateDamage(GameState.GoodGuy, GameState.BadGuy);

        if (_enemyBlocking)
        {
            damage = Mathf.Max(0, damage - 2);
            _enemyBlocking = false;
            Log.Info("Enemy blocked part of the player's attack.");
        }

        DamageSvc.ApplyDamage(GameState.BadGuy, damage);
        Log.Info("Player attacked for " + damage + " damage.");

        if (GameState.BadGuy.Health.Value <= 0)
        {
            EndCombat("Enemy defeated.");
            return;
        }

        _isPlayerTurn = false;
        EnemyTakeTurn();
    }

    public void PlayerParry()
    {
        if (!_combatActive || !_isPlayerTurn) return;

        _playerBlocking = true;
        _isPlayerTurn = false;
        Log.Info("Player used Block.");

        EnemyTakeTurn();
    }

    public void PlayerSpecial()
    {
        if (!_combatActive || !_isPlayerTurn) return;

        int damage = DamageSvc.CalculateDamage(GameState.GoodGuy, GameState.BadGuy) + 2;

        if (_enemyBlocking)
        {
            damage = Mathf.Max(0, damage - 2);
            _enemyBlocking = false;
            Log.Info("Enemy blocked part of the player's special.");
        }

        DamageSvc.ApplyDamage(GameState.BadGuy, damage);
        Log.Info("Player used Special for " + damage + " damage.");

        if (GameState.BadGuy.Health.Value <= 0)
        {
            EndCombat("Enemy defeated by special.");
            return;
        }

        _isPlayerTurn = false;
        EnemyTakeTurn();
    }

    public void ResetCombat()
    {
        _combatActive = false;
        _isPlayerTurn = true;
        _playerBlocking = false;
        _enemyBlocking = false;

        if (_playerInstance != null)
        {
            Destroy(_playerInstance);
            _playerInstance = null;
        }

        if (_enemyInstance != null)
        {
            Destroy(_enemyInstance);
            _enemyInstance = null;
        }

        Log.Info("Combat reset.");
    }

    private void EnemyTakeTurn()
    {
        if (!_combatActive) return;

        int action = Random.Range(0, 3);

        if (action == 0)
        {
            EnemyAttack();
        }
        else if (action == 1)
        {
            EnemyBlock();
        }
        else
        {
            EnemySpecial();
        }

        if (_combatActive)
            _isPlayerTurn = true;
    }

    private void EnemyAttack()
    {
        int damage = DamageSvc.CalculateDamage(GameState.BadGuy, GameState.GoodGuy);

        if (_playerBlocking)
        {
            damage = Mathf.Max(0, damage - 2);
            _playerBlocking = false;
            Log.Info("Player blocked part of the enemy attack.");
        }

        DamageSvc.ApplyDamage(GameState.GoodGuy, damage);
        Log.Info("Enemy attacked for " + damage + " damage.");

        if (GameState.GoodGuy.Health.Value <= 0)
        {
            EndCombat("Player defeated.");
        }
    }

    private void EnemyBlock()
    {
        _enemyBlocking = true;
        _playerBlocking = false;
        Log.Info("Enemy used Block.");
    }

    private void EnemySpecial()
    {
        int damage = DamageSvc.CalculateDamage(GameState.BadGuy, GameState.GoodGuy) + 2;

        if (_playerBlocking)
        {
            damage = Mathf.Max(0, damage - 2);
            _playerBlocking = false;
            Log.Info("Player blocked part of the enemy special.");
        }

        DamageSvc.ApplyDamage(GameState.GoodGuy, damage);
        Log.Info("Enemy used Special for " + damage + " damage.");

        if (GameState.GoodGuy.Health.Value <= 0)
        {
            EndCombat("Player defeated by enemy special.");
        }
    }

    private void EndCombat(string message)
    {
        _combatActive = false;
        _playerBlocking = false;
        _enemyBlocking = false;
        Log.Info(message);
    }
}