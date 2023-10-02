using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileScript : MonoBehaviour {
    
    public Material redTile;
    private bool _tileClicked = false;

    void Start() {
        // Tiles drop at the start of game at different rates
        GetComponent<Rigidbody>().maxLinearVelocity = Random.Range(1.5f, 3.0f);
    }

    void Update() {
        if (!Input.GetMouseButtonDown(0) || _tileClicked) {
            return;
        }

        if (TileClicked()) {
            // GetComponent<Renderer>().material.color = Color.red;
            GetComponent<Renderer>().material = redTile;
            _tileClicked = true;
        }
    }

    private bool TileClicked() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out var hit) && hit.collider.gameObject.name == name;
    }
}
