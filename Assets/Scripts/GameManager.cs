using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    private bool _setupComplete;
    private bool _playerTurn = true;

    void Start() {
        Instance = this;
    }

    void Update() {
        if (!_setupComplete) return;

        if (_playerTurn) {
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
