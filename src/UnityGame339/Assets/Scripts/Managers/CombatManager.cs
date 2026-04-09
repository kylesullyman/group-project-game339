using Game.Runtime;
using Game339.Shared.Diagnostics;
using Game339.Shared.Models;
using Game339.Shared.Services;
using Game339.Shared.ViewModels;
using ScriptableObjects;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("Unit Data")]
    [SerializeField] private ChessUnitData playerUnit;
    [SerializeField] private ChessUnitData enemyUnit;

    private bool _isPlayerTurn = true;
    private bool _combatActive;
    private bool _playerBlocking;
    private bool _enemyBlocking;

    private static GameState GameState => ServiceResolver.Resolve<GameState>();
    private static IDamageService DamageSvc => ServiceResolver.Resolve<IDamageService>();
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    private static ICombatViewModel CombatViewModel => ServiceResolver.Resolve<ICombatViewModel>();

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
        if (playerUnit == null || enemyUnit == null)
        {
            Log.Error("CombatManager is missing ChessUnitData references.");
            _combatActive = false;
            return;
        }

        GameState.GoodGuy.Name.Value = playerUnit.unitName;
        GameState.GoodGuy.Health.Value = playerUnit.health;
        GameState.GoodGuy.Damage.Value = playerUnit.damage;
        GameState.GoodGuy.Armor.Value = playerUnit.armor;

        GameState.BadGuy.Name.Value = enemyUnit.unitName;
        GameState.BadGuy.Health.Value = enemyUnit.health;
        GameState.BadGuy.Damage.Value = enemyUnit.damage;
        GameState.BadGuy.Armor.Value = enemyUnit.armor;

        _playerBlocking = false;
        _enemyBlocking = false;
        _isPlayerTurn = true;
        _combatActive = true;

        CombatViewModel.OnCombatStarted(playerUnit.health, enemyUnit.health);

        Log.Info("Combat started.");
    }

    public void RestartCombat()
    {
        Log.Info("Combat restarted.");
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
        CombatViewModel.OnStatusUpdated("Player attacked for " + damage + " damage.");
        Log.Info("Player attacked for " + damage + " damage.");

        if (GameState.BadGuy.Health.Value <= 0)
        {
            EndCombat(true);
            return;
        }

        _isPlayerTurn = false;
        CombatViewModel.OnEnemyTurnBegan();
        EnemyTakeTurn();
    }

    public void PlayerBlock()
    {
        if (!_combatActive || !_isPlayerTurn) return;

        _playerBlocking = true;
        _isPlayerTurn = false;
        CombatViewModel.OnStatusUpdated("Player is blocking.");
        CombatViewModel.OnEnemyTurnBegan();
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
        CombatViewModel.OnStatusUpdated("Player used Special for " + damage + " damage.");
        Log.Info("Player used Special for " + damage + " damage.");

        if (GameState.BadGuy.Health.Value <= 0)
        {
            EndCombat(true);
            return;
        }

        _isPlayerTurn = false;
        CombatViewModel.OnEnemyTurnBegan();
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
        {
            _isPlayerTurn = true;
            CombatViewModel.OnPlayerTurnBegan();
        }
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
        CombatViewModel.OnStatusUpdated("Enemy attacked for " + damage + " damage.");
        Log.Info("Enemy attacked for " + damage + " damage.");

        if (GameState.GoodGuy.Health.Value <= 0)
            EndCombat(false);
    }

    private void EnemyBlock()
    {
        _enemyBlocking = true;
        _playerBlocking = false;
        CombatViewModel.OnStatusUpdated("Enemy is blocking.");
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
        CombatViewModel.OnStatusUpdated("Enemy used Special for " + damage + " damage.");
        Log.Info("Enemy used Special for " + damage + " damage.");

        if (GameState.GoodGuy.Health.Value <= 0)
            EndCombat(false);
    }

    private void EndCombat(bool playerWon)
    {
        _combatActive = false;
        _playerBlocking = false;
        _enemyBlocking = false;

        CombatViewModel.OnCombatEnded(playerWon);
        Log.Info(playerWon ? "Player won combat." : "Player lost combat.");
    }
}
