using UnityEngine;
using UnityEngine.UI;

public class StatusTextScript : MonoBehaviour
{
    public Text statusText;
    private const string StartText = "Place your battleships";
    private const string SelectTargetText = "Select enemy target";
    private const string EnemyTargetingText = "Enemy is targeting you";

    void Start() {
        statusText.text = StartText;
    }
    
    public void ClearText() {
        statusText.text = "";
    }

    public void SetSelectTargetText() {
        statusText.text = SelectTargetText;
    }
    
    public void SetEnemyTargetingText() {
        statusText.text = EnemyTargetingText;
    }
}
