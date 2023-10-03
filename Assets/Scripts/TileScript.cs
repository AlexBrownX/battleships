using UnityEngine;

public class TileScript : MonoBehaviour {

    public Material tile;
    public Material redTile;
    public Material greenTile;

    public bool shipHovering = false;
    public bool setupComplete = false;
    
    void Start() {
        // Tiles drop at the start of game at different rates
        GetComponent<Rigidbody>().maxLinearVelocity = Random.Range(1.5f, 3.0f);
    }

    void Update() {
        Setup();
    }

    private void Setup() {
        if (setupComplete) return;
        shipHovering = true;

        if (MouseOverTile()) {
            HoverShip();

            if (BoardSetup.Instance.CanDrop()) {
                GetComponent<Renderer>().material = greenTile;

                if (Input.GetMouseButtonDown(0)) {
                    BoardSetup.Instance.ShipPlaced();
                    return;
                }
            }
            else {
                GetComponent<Renderer>().material = redTile;
            }
            return;
        }
        
        if (isShipOverTile()) {
            GetComponent<Renderer>().material = BoardSetup.Instance.CanDrop() ? greenTile : redTile;
            return;
        }
        
        shipHovering = false;
        GetComponent<Renderer>().material = tile;
    }

    public void CompleteSetup() {
        shipHovering = false;
        setupComplete = true;
        GetComponent<Renderer>().material = tile;
        // Debug.Log($"{name} setup complete");
    }
    
    private bool isShipOverTile() {
        var direction = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
        // Debug.DrawLine(transform.position, direction, Color.red, 1);
        return Physics.Raycast(transform.position, direction, out var hit) && 
               hit.collider.gameObject.name == BoardSetup.Instance.currentShip.name;
    }

    private void HoverShip() {
        var tilePosition = transform.position;
        var zOffset = BoardSetup.Instance.currentShip.GetComponent<PlayerShipScript>().GetOffset();
        var hoverPosition = new Vector3(tilePosition.x, tilePosition.y + 1f, tilePosition.z - zOffset);
        BoardSetup.Instance.currentShip.transform.position = hoverPosition;
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
