using Game.Runtime;
using Game339.Shared.Models;
using Game339.Shared.Services;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("Spawn Positions")]
    [SerializeField] private Vector3 playerSpawnPosition = new Vector3(-4f, -3f, 0f);
    [SerializeField] private Vector3 enemySpawnPosition = new Vector3(4f, 3f, 0f);

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPawnPrefab;
    [SerializeField] private GameObject enemyPawnPrefab;

    private bool _isPlayerTurn = true;
    private bool _combatActive;
    private bool _playerParried;

    private GameObject _playerInstance;
    private GameObject _enemyInstance;

    private static GameState GameState => ServiceResolver.Resolve<GameState>();
    private static IDamageService DamageSvc => ServiceResolver.Resolve<IDamageService>();

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

        if (GameState == null || DamageSvc == null)
        {
            Debug.LogError("GameState or IDamageService is not registered.");
            return;
        }

        GameState.GoodGuy.Name.Value = "Player";
        GameState.GoodGuy.Health.Value = 10;
        GameState.GoodGuy.Damage.Value = 2;
        GameState.GoodGuy.Armor.Value = 0;

        GameState.BadGuy.Name.Value = "Enemy Pawn";
        GameState.BadGuy.Health.Value = 10;
        GameState.BadGuy.Damage.Value = 1;
        GameState.BadGuy.Armor.Value = 0;

        if (playerPawnPrefab != null && _playerInstance == null)
            _playerInstance = Instantiate(playerPawnPrefab, playerSpawnPosition, Quaternion.identity);

        if (enemyPawnPrefab != null && _enemyInstance == null)
            _enemyInstance = Instantiate(enemyPawnPrefab, enemySpawnPosition, Quaternion.identity);

        _playerParried = false;
        _isPlayerTurn = true;
        _combatActive = true;
    }

    public void PlayerAttack()
    {
        if (!_combatActive || !_isPlayerTurn) return;

        int damage = DamageSvc.CalculateDamage(GameState.GoodGuy, GameState.BadGuy);
        DamageSvc.ApplyDamage(GameState.BadGuy, damage);

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

        _playerParried = true;
        Debug.Log("Player parried.");
        _isPlayerTurn = false;
        EnemyTakeTurn();
    }

    public void PlayerSpecial()
    {
        if (!_combatActive || !_isPlayerTurn) return;

        int damage = DamageSvc.CalculateDamage(GameState.GoodGuy, GameState.BadGuy) + 2;
        DamageSvc.ApplyDamage(GameState.BadGuy, damage);

        if (GameState.BadGuy.Health.Value <= 0)
        {
            EndCombat("Enemy defeated by special attack.");
            return;
        }

        _isPlayerTurn = false;
        EnemyTakeTurn();
    }

    public void ResetCombat()
    {
        _combatActive = false;
        _isPlayerTurn = true;
        _playerParried = false;

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
    }

    private void EnemyTakeTurn()
    {
        if (!_combatActive) return;

        int damage = DamageSvc.CalculateDamage(GameState.BadGuy, GameState.GoodGuy);

        if (_playerParried)
        {
            damage = 0;
            _playerParried = false;
            Debug.Log("Parry blocked the enemy attack.");
        }

        DamageSvc.ApplyDamage(GameState.GoodGuy, damage);

        if (GameState.GoodGuy.Health.Value <= 0)
        {
            EndCombat("Player defeated.");
            return;
        }

        _isPlayerTurn = true;
    }

    private void EndCombat(string message)
    {
        _combatActive = false;
        _playerParried = false;
        Debug.Log(message);
    }
}