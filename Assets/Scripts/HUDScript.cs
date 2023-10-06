using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour {

    public Button newGameBtn;
    public Button exitGameBtn;
    public Button rightBtn;
    public Button leftBtn;

    public Text statusText;
    public Text playerInfo;
    public Text enemyInfo;
    public Text playerHits;
    public Text enemyHits;
    
    private const string StartText = "Place your battleships";
    private const string SelectTargetText = "Select enemy target";
    private const string EnemyTargetingText = "Enemy is targeting you";
    private const string PlayerShipsText = "Player Ships: {0}";
    private const string EnemyShipsText = "Enemy Ships: {0}";
    private const string PlayerHitsText = "Player Hits: {0}";
    private const string EnemyHitsText = "Enemy Hits: {0}";
    private const string HitText = "Hit!";
    private const string MissText = "Miss!";
    private const string ShipSunkText = "Ship Sunk!";
    private const string YouWinText = "You Win!";
    private const string YouLoseText = "You Lose!";
    
    void Start() {
        rightBtn.gameObject.SetActive(false);
        leftBtn.gameObject.SetActive(false);
        
        statusText.text = StartText;
        
        newGameBtn.onClick.AddListener(NewGame);
        exitGameBtn.onClick.AddListener(ExitGame);
    }

    private void NewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void ExitGame() {
        Application.Quit();
    }
    
    public void ClearText() {
        statusText.text = "";
    }

    public void UpdateShipCounts(int playerShips, int enemyShips) {
        playerInfo.text = string.Format(PlayerShipsText, playerShips);
        enemyInfo.text = string.Format(EnemyShipsText, enemyShips);
    }

    public void ClearShipCounts() {
        playerInfo.text = "";
        enemyInfo.text = "";
    }

    public void SetSelectTargetText() {
        statusText.text = SelectTargetText;
    }
    
    public void SetEnemyTargetingText() {
        statusText.text = EnemyTargetingText;
    }

    public void SetHit() {
        statusText.text = HitText;
    }
    
    public void SetMiss() {
        statusText.text = MissText;
    }
    
    public void SetShipSunk() {
        statusText.text = ShipSunkText;
    }
    
    public void SetYouWin() {
        statusText.text = YouWinText;
    }
    
    public void SetYouLose() {
        statusText.text = YouLoseText;
    }

    public void SetHits(int playerCount, int enemyCount) {
        playerHits.text = string.Format(PlayerHitsText, playerCount);
        enemyHits.text = string.Format(EnemyHitsText, enemyCount);
    }
}
