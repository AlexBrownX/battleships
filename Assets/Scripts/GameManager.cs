using GameSetup;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    private bool _playerSetupComplete;
    private bool _enemySetupComplete;
    private bool _playerTurn = true;
    private bool _turnTaken = false;

    void Start() {
        Instance = this;
    }

    void Update() {
        if (!_playerSetupComplete || !_enemySetupComplete) return;

        if (_playerTurn && !_turnTaken) {
            PlayerTurn();
        }
        else {
            EnemyTurn();
        }
    }

    private void PlayerTurn() {
        GetComponent<CameraScript>().RightBtnClicked();
    }

    private void EnemyTurn() {
        GetComponent<CameraScript>().LeftBtnClicked();
    }

    public void PlayerCompleteSetup() {
        _playerSetupComplete = true;
        Destroy(GetComponent<PlayerBoardSetup>());
        Debug.Log("Player board setup complete");
    }
    
    public void EnemyCompleteSetup() {
        _enemySetupComplete = true;
        Destroy(GetComponent<EnemyBoardSetup>());
        Debug.Log("Enemy board setup complete");
    }
}
