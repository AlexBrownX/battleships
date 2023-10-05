using System.Collections;
using System.Collections.Generic;
using GameSetup;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    
    private bool _playerSetupComplete;
    private bool _enemySetupComplete;
    private bool _playerTurn = true;
    private bool _turnTaken;
    private List<GameObject[]> _enemyTiles;

    void Start() {
        Instance = this;
    }

    void Update() {
        if (!_playerSetupComplete || !_enemySetupComplete) return;
        if (!_turnTaken) return;
        
        if (_playerTurn) {
            PlayerTurn();
        }
        else {
            EnemyTurn();
        }
    }

    public void PlayerTurnTaken() {
        _turnTaken = true;
        _playerTurn = false;
    }
    
    public void EnemyTurnTaken() {
        _turnTaken = true;
        _playerTurn = true;
    }
    
    private void PlayerTurn() {
        _turnTaken = false;
        Debug.Log("Player turn");
        GetComponent<CameraScript>().MoveToEnemyBoard();
    }

    private void EnemyTurn() {
        _turnTaken = false;
        Debug.Log("Enemy turn");
        GetComponent<CameraScript>().MoveToPlayerBoard();
    }

    public void PlayerCompleteSetup() {
        _playerSetupComplete = true;
        _turnTaken = true;
        Destroy(GetComponent<PlayerBoardSetup>());
        Debug.Log("Player board setup complete");
    }
    
    public void EnemyCompleteSetup(List<GameObject[]> enemyTiles) {
        _enemyTiles = enemyTiles;
        _enemySetupComplete = true;
        Destroy(GetComponent<EnemyBoardSetup>());
        Debug.Log("Enemy board setup complete");
    }
}
