using Game.Runtime;
using Game339.Shared.Diagnostics;
using Game339.Shared.Models;
using Game339.Shared.Services;
using ScriptableObjects;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("Enemy Data")]
    [SerializeField] private ChessUnitData pawn;

    [Header("UI")]
    [SerializeField] private CombatResultUI combatResultUI;

    private bool _isPlayerTurn = true;
    private bool _combatActive;
    private bool _playerBlocking;
    private bool _enemyBlocking;

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

    private void Start()
    {
        StartCombat();
    }

    public void StartCombat()
    {
        GameState.GoodGuy.Name.Value = "Player";
        GameState.GoodGuy.Health.Value = 10;
        GameState.GoodGuy.Damage.Value = 2;
        GameState.GoodGuy.Armor.Value = 0;

        GameState.BadGuy.Name.Value = "Enemy";
        GameState.BadGuy.Health.Value = 10;
        GameState.BadGuy.Damage.Value = 2;
        GameState.BadGuy.Armor.Value = 0;
        
        _playerBlocking = false;
        _enemyBlocking = false;
        _isPlayerTurn = true;
        _combatActive = true;

        if (combatResultUI != null)
            combatResultUI.Hide();

        Log.Info("Combat started.");
    }

    public void RestartCombat()
    {
        StartCombat();
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
            EndCombat(true);
            return;
        }

        _isPlayerTurn = false;
        EnemyTakeTurn();
    }

    public void PlayerBlock()
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
            EndCombat(true);
            return;
        }

        _isPlayerTurn = false;
        EnemyTakeTurn();
    }

    private void EnemyTakeTurn()
    {
        if (!_combatActive) return;

        int action = Random.Range(0, 3);

        if (action == 0)
            EnemyAttack();
        else if (action == 1)
            EnemyBlock();
        else
            EnemySpecial();

        if (_combatActive)
            _isPlayerTurn = true;
    }

    private void EnemyAttack()
    {
        int damage = DamageSvc.CalculateDamage(GameState.BadGuy, GameState.GoodGuy);

        if (_playerBlocking)
        {
            damage = Mathf.Max(0, damage - 2);
            Log.Info("Player blocked part of the enemy attack.");
        }

        _playerBlocking = false;

        DamageSvc.ApplyDamage(GameState.GoodGuy, damage);
        Log.Info("Enemy attacked for " + damage + " damage.");

        if (GameState.GoodGuy.Health.Value <= 0)
            EndCombat(false);
    }

    private void EnemyBlock()
    {
        _enemyBlocking = true;

        // keep player's block for the next actual incoming hit only if you want;
        // better to consume it after enemy action cycle:
        _playerBlocking = false;

        Log.Info("Enemy used Block.");
    }

    private void EnemySpecial()
    {
        int damage = DamageSvc.CalculateDamage(GameState.BadGuy, GameState.GoodGuy) + 2;

        if (_playerBlocking)
        {
            damage = Mathf.Max(0, damage - 2);
            Log.Info("Player blocked part of the enemy special.");
        }

        _playerBlocking = false;

        DamageSvc.ApplyDamage(GameState.GoodGuy, damage);
        Log.Info("Enemy used Special for " + damage + " damage.");

        if (GameState.GoodGuy.Health.Value <= 0)
            EndCombat(false);
    }

    private void EndCombat(bool playerWon)
    {
        _combatActive = false;
        _playerBlocking = false;
        _enemyBlocking = false;

        if (combatResultUI != null)
        {
            if (playerWon)
                combatResultUI.ShowWin();
            else
                combatResultUI.ShowLose();
        }

        Log.Info(playerWon ? "Player won combat." : "Player lost combat.");
    }
}