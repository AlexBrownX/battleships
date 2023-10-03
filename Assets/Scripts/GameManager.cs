using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    private bool _setupComplete;

    void Start() {
        Instance = this;
    }

    void Update() {
        Setup();
    }
    
    private void Setup() {
        if (_setupComplete) return;
    }
}
