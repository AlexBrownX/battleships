using System.Collections.Generic;
using System.Linq;
using Enemy;
using Player;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private bool _playerTurn = true;
    private bool _playerSetupComplete;
    private bool _enemySetupComplete;
    private bool _turnTaken;
    
    private List<GameObject[]> _playerTiles;
    private List<GameObject[]> _enemyTiles;

    void Start() {
        Instance = this;
    }

    void Update() {
        if (!_playerSetupComplete || !_enemySetupComplete) return;
        if (!_turnTaken) return;

        DisplayHits();
        
        if (_playerTurn) {
            PlayerTurn();
        }
        else {
            EnemyTurn();
        }
    }

    public bool IsPlayerTurn() {
        return _playerTurn;
    }

    public void TurnTaken() {
        _turnTaken = true;
        _playerTurn = !_playerTurn;
    }
    
    private void PlayerTurn() {
        Debug.Log("Player turn");
        _turnTaken = false;
        GetComponent<CameraScript>().ViewEnemyBoard();
    }

    private void EnemyTurn() {
        Debug.Log("Enemy turn");
        _turnTaken = false;
        GetComponent<CameraScript>().ViewPlayerBoard();
        GetComponent<EnemyPlayer>().TakeTurn();
    }

    public void PlayerCompleteSetup(List<GameObject[]> playerTiles) {
        _playerTiles = playerTiles;
        _playerSetupComplete = true;
        _turnTaken = true;
        Destroy(GetComponent<PlayerBoardSetup>());
        // LogPlayerTiles();
    }

    private void LogPlayerTiles() { 
        Debug.Log("Player board setup complete");
        _playerTiles.ForEach(enemyTile => {
            var playerShip = string.Join(",", enemyTile.Select(e => e.name.ToString()).ToArray());
            Debug.Log($"Player ship : {playerShip}");
        });
    }
    
    public void EnemyCompleteSetup(List<GameObject[]> enemyTiles) {
        _enemyTiles = enemyTiles;
        _enemySetupComplete = true;
        Destroy(GetComponent<EnemyBoardSetup>());
        // LogEnemyTiles();
    }

    private void LogEnemyTiles() {
        Debug.Log("Enemy board setup complete");
        _enemyTiles.ForEach(enemyTile => {
            var enemyShip = string.Join(",", enemyTile.Select(e => e.name.ToString()).ToArray());
            Debug.Log($"Enemy ship : {enemyShip}");
        });
    }
    
    private void DisplayHits() {
        var playerHits = 0;
        var enemyHits = 0;
        _enemyTiles.ForEach(enemyShipTiles => {
            playerHits += enemyShipTiles.Count(enemyShipTile => 
                enemyShipTile.GetComponent<EnemyTile>().HasShip() && 
                enemyShipTile.GetComponent<EnemyTile>().HasMissile());
        });
        _playerTiles.ForEach(playerShipTiles => {
            enemyHits += playerShipTiles.Count(playerShipTile => 
                playerShipTile.GetComponent<PlayerTile>().HasShip() &&
                playerShipTile.GetComponent<PlayerTile>().HasMissile());
        });
        
        Debug.Log($"Player successful hits: {playerHits}");
        Debug.Log($"Enemy successful hits: {enemyHits}");
    }
}
