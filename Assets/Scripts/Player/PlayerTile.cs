using UnityEngine;

namespace Player {
    public class PlayerTile : MonoBehaviour {

        public Material tile;
        public Material redTile;
        public Material greenTile;

        public bool shipHovering;

        private bool _hasShip;
        private bool _missileDroppedOnTile;
        private bool _setupComplete;

        void Start() {
            // Tiles drop at the start of game at different rates
            GetComponent<Rigidbody>().maxLinearVelocity = Random.Range(1.5f, 3.0f);
        }

        void Update() {
            Setup();
        }

        private void Setup() {
            if (_setupComplete) return;
            shipHovering = true;
        
            if (MouseOverTile()) {
                HoverShip();
            }

            if (isShipOverTile()) {
                if (PlayerBoardSetup.Instance.CanDrop()) {
                    GetComponent<Renderer>().material = greenTile;
        
                    if (MouseOverTile() && Input.GetMouseButtonDown(0)) {
                        PlayerBoardSetup.Instance.ShipPlaced();
                        return;
                    }
                }
                else {
                    GetComponent<Renderer>().material = redTile;
                }
                return;
            }
        
            shipHovering = false;
            GetComponent<Renderer>().material = tile;
        }

        public void CompleteSetup() {
            shipHovering = false;
            _setupComplete = true;
            GetComponent<Renderer>().material = tile;
        }
    
        private bool isShipOverTile() {
            var direction = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            return Physics.Raycast(transform.position, direction, out var hit) && 
                   hit.collider.gameObject.name == PlayerBoardSetup.Instance.currentShip.name;
        }

        private void HoverShip() {
            var tilePosition = transform.position;
            var zOffset = PlayerBoardSetup.Instance.currentShip.GetComponent<PlayerShipScript>().GetZOffset();
            var xOffset = PlayerBoardSetup.Instance.currentShip.GetComponent<PlayerShipScript>().GetXOffset();
            var hoverPosition = new Vector3(tilePosition.x - xOffset, tilePosition.y + 1f, tilePosition.z - zOffset);
            PlayerBoardSetup.Instance.currentShip.transform.position = hoverPosition;
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

        public void HighlightTile() {
            GetComponent<Renderer>().material = _hasShip ? greenTile : redTile;
        }

        public void PlaceShipOnTile() {
            _hasShip = true;
            CompleteSetup();
        }
        
        public void MissileDroppedOnTile() {
            _missileDroppedOnTile = true;
        }
        
        public bool HasShip() {
            return _hasShip;
        }

        public bool HasMissile() {
            return _missileDroppedOnTile;
        }
    }
}
