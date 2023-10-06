using UnityEngine;

namespace Player {
    public class PlayerTile : MonoBehaviour {

        public AudioClip bigExplosion;
        public AudioClip explosion;
        public AudioClip splash;
        
        public Material tile;
        public Material redTile;
        public Material greenTile;

        public bool shipHovering;

        private GameObject _ship;
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

            if (IsShipOverTile()) {
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
    
        private bool IsShipOverTile() {
            var position = transform.position;
            var direction = new Vector3(position.x, position.y + 50f, position.z);
            return Physics.Raycast(position, direction, out var hit) && 
                   hit.collider.gameObject.name == PlayerBoardSetup.Instance.currentShip.name;
        }

        private void HoverShip() {
            var tilePosition = transform.position;
            var zOffset = PlayerBoardSetup.Instance.currentShip.GetComponent<PlayerShip>().GetZOffset();
            var xOffset = PlayerBoardSetup.Instance.currentShip.GetComponent<PlayerShip>().GetXOffset();
            var hoverPosition = new Vector3(tilePosition.x - xOffset, tilePosition.y + 1f, tilePosition.z - zOffset);
            PlayerBoardSetup.Instance.currentShip.transform.position = hoverPosition;
        }

        private bool MouseOverTile() {
            if (Camera.main == null) return false;
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
            if (HasShip()) {
                if (_ship.GetComponent<PlayerShip>().IsSunk()) {
                    AudioSource.PlayClipAtPoint(bigExplosion, transform.position, 1f);
                }
                else {
                    AudioSource.PlayClipAtPoint(explosion, transform.position, 0.8f);
                }
            }
            else {
                AudioSource.PlayClipAtPoint(splash, transform.position, 1f);
            }
            
            GetComponent<Renderer>().material = _hasShip ? greenTile : redTile;
        }

        public void PlaceShipOnTile() {
            _ship = PlayerBoardSetup.Instance.currentShip;
            _hasShip = true;
            CompleteSetup();
        }
        
        public void MissileDroppedOnTile() {
            _missileDroppedOnTile = true;

            if (HasShip()) {
                _ship.GetComponent<PlayerShip>().MissileHit();
                // Debug.Log($"{_ship.name} hit !");
            }
        }
        
        public bool HasShip() {
            return _hasShip;
        }

        public bool HasMissile() {
            return _missileDroppedOnTile;
        }
    }
}
