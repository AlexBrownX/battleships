using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShipScript : MonoBehaviour {

    public float zOffset;
    public float zRotatedOffset;
    public float xOffset;
    public float xRotatedOffset;
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

    public float GetZOffset() {
        return _rotated ? zRotatedOffset : zOffset;
    }
    
    public float GetXOffset() {
        return _rotated ? xRotatedOffset : xOffset;
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
