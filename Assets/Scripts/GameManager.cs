using System.Collections.Generic;
using System.Linq;
using Enemy;
using Player;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public AudioClip win;
    public AudioClip lose;

    private bool _playerTurn = true;
    private bool _playerSetupComplete;
    private bool _enemySetupComplete;
    private bool _turnTaken;
    
    private List<GameObject[]> _playerTiles;
    private List<GameObject[]> _enemyTiles;

    private GameObject[] _playerShips;
    private GameObject[] _enemyShips;
        
    void Start() {
        Instance = this;
    }

    void Update() {
        if (!_playerSetupComplete || !_enemySetupComplete) {
            GetComponent<HUDScript>().ClearShipCounts();
            return;
        }
        
        if (!_turnTaken) return;
        
        DisplayHits();
        
        var playerShipCount = 5 - _playerShips.Count(ship => ship.GetComponent<PlayerShip>().IsSunk());
        var enemyShipCount = 5 - _enemyShips.Count(ship => ship.GetComponent<EnemyShip>().IsSunk());
        GetComponent<HUDScript>().UpdateShipCounts(playerShipCount, enemyShipCount);

        if (playerShipCount == 0) {
            GetComponent<AudioSource>().Stop();
            GetComponent<HUDScript>().SetYouLose();
            AudioSource.PlayClipAtPoint(lose, transform.position, 1f);
            _turnTaken = false;
            return;
        }

        if (enemyShipCount == 0) {
            GetComponent<AudioSource>().Stop();
            GetComponent<HUDScript>().SetYouWin();
            AudioSource.PlayClipAtPoint(win, transform.position, 1f);
            _turnTaken = false;
            return;
        }
        
        GetComponent<HUDScript>().ClearText();
        
        if (_playerTurn) {
            PlayerTurn();
        }
        else {
            EnemyTurn();
        }
    }

    private void UpdateShipCounts() {
        var playerShipCount = 5 - _playerShips.Count(ship => ship.GetComponent<PlayerShip>().IsSunk());
        var enemyShipCount = 5 - _enemyShips.Count(ship => ship.GetComponent<EnemyShip>().IsSunk());

        GetComponent<HUDScript>().UpdateShipCounts(playerShipCount, enemyShipCount);
    }

    public bool IsPlayerTurn() {
        return _playerTurn;
    }

    public void TurnTaken() {
        _turnTaken = true;
        _playerTurn = !_playerTurn;
    }
    
    private void PlayerTurn() {
        _turnTaken = false;
        GetComponent<CameraScript>().ViewEnemyBoard();
    }

    private void EnemyTurn() {
        _turnTaken = false;
        GetComponent<CameraScript>().ViewPlayerBoard();
        GetComponent<EnemyPlayer>().TakeTurn();
    }

    public void PlayerCompleteSetup(List<GameObject[]> playerTiles) {
        _playerTiles = playerTiles;
        _playerSetupComplete = true;
        _turnTaken = true;
        _playerShips = PlayerBoardSetup.Instance.ships;
        Destroy(GetComponent<PlayerBoardSetup>());
        // LogPlayerTiles();
    }

    public void EnemyCompleteSetup(List<GameObject[]> enemyTiles) {
        _enemyTiles = enemyTiles;
        _enemySetupComplete = true;
        _enemyShips = EnemyBoardSetup.Instance.ships;
        Destroy(GetComponent<EnemyBoardSetup>());
        LogEnemyTiles();
    }

    private void LogPlayerTiles() { 
        Debug.Log("Player board setup complete");
        _playerTiles.ForEach(enemyTile => {
            var playerShip = string.Join(",", enemyTile.Select(e => e.name.ToString()).ToArray());
            Debug.Log($"Player ship : {playerShip}");
        });
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
        
        GetComponent<HUDScript>().SetHits(playerHits, enemyHits);
    }
}
