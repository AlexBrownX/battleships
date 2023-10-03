using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShipScript : MonoBehaviour {

    public float zOffset;
    public int shipSize;
    private bool _setupComplete = false;
    private bool _rotated = false;

    private void Awake() {
        GetComponent<Renderer>().enabled = false;
        gameObject.SetActive(false);
    }

    void Start() {
    }

    void Update()
    {
        Setup();
    }

    public float GetOffset() {
        return _rotated ? 0f : zOffset;
    }
    
    private void Setup() {
        if (_setupComplete) return;
    }

    public void SetupComplete() {
        _setupComplete = true;
        // Debug.Log($"{name} setup complete");
    }

    public void Rotate() {
        if (_rotated) {
            gameObject.transform.Rotate(0, 90, 0);
            _rotated = false;
        }
        else {
            gameObject.transform.Rotate(0, -90, 0);
            _rotated = true;
        }
    }
}
