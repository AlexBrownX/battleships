using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    private bool _setupComplete;
    private bool _playerTurn = true;
    private bool _turnTaken = false;

    void Start() {
        Instance = this;
    }

    void Update() {
        if (!_setupComplete) return;

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

    public void CompleteSetup() {
        _setupComplete = true;
    }
}
