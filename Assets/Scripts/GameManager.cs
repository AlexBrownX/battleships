using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public GameObject[] ships;
    public GameObject currentShip;

    private int _shipIndex = 0;
    private bool _setupComplete = false;

    void Start() {
        Instance = this;
    }

    void Update() {
        Setup();
    }
    
    private void Setup() {
        if (_setupComplete) return;
        currentShip = ships[_shipIndex];
    }
}
