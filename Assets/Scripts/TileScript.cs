using UnityEngine;

public class TileScript : MonoBehaviour {

    // public GameManager GameManager;
    public Material tile;
    public Material redTile;
    public Material greenTile;
    
    private bool _setupComplete = false;
    
    void Start() {
        // Tiles drop at the start of game at different rates
        GetComponent<Rigidbody>().maxLinearVelocity = Random.Range(1.5f, 3.0f);
    }

    void Update() {
        Setup();
    }

    private void Setup() {
        if (_setupComplete) return;
        GetComponent<Renderer>().material = MouseOverTile() ? greenTile : tile;

        var currentShip = GameManager.Instance.currentShip;

        if (MouseOverTile()) {
            HoverShip(currentShip);
        }
    }

    private void HoverShip(GameObject currentShip) {
        var tilePosition = transform.position;
        var zPositionOffset = currentShip.GetComponent<PlayerShipScript>().zPositionOffset;
        var hoverPosition = new Vector3(tilePosition.x, tilePosition.y + 1f, tilePosition.z - zPositionOffset);
        currentShip.transform.position = hoverPosition;
    }

    private bool MouseOverTile() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var raycastHits = new RaycastHit[5];
        var hits = Physics.RaycastNonAlloc(ray, raycastHits);

        for (var i = 0; i < hits; i++) {
            if (raycastHits[i].collider.gameObject.name == name) {
                return true;
            }
        }

        return false;
    }
}
