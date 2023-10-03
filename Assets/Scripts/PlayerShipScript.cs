using UnityEngine;

public class PlayerShipScript : MonoBehaviour {

    public float zPositionOffset;
    private bool _setupComplete = false;

    void Start() {
        // GetComponent<Renderer>().enabled = false;
    }

    void Update()
    {
        Setup();
    }
    
    private void Setup() {
        if (_setupComplete) return;
    }
}
