using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Multiplayer {
    public class ClientTile : NetworkBehaviour {

        [SerializeField] private GameObject missilePrefab;
        [SerializeField] public Material clearTile;
        [SerializeField] public Material redTile;
        [SerializeField] public Material greenTile;
        [SerializeField] public Material yellowTile;
        [SerializeField] public AudioClip bigExplosion;
        [SerializeField] public AudioClip explosion;
        [SerializeField] public AudioClip splash;
        
        private GameObject _missile;
        private bool _hasShip;
        private bool _setupComplete;
        private bool _shipHovering;
        private bool _missileHit;

        void Start() {
            GetComponent<Rigidbody>().maxLinearVelocity = Random.Range(1.5f, 3.0f);
        }

        private void Update() {
            if (!_setupComplete) {
                Setup();
                return;
            }

            if (_missileHit) return;
            
            if (NetworkManager.Singleton.IsHost && 
                GameManager.Instance.hostTurn && 
                _missile == null &&
                MouseOverTile()) {
                
                GetComponent<Renderer>().material = yellowTile;

                if (Input.GetMouseButtonDown(0)) {
                    DropMissileAboveTile();
                }
                
                return;             
            }
            
            GetComponent<Renderer>().material = clearTile;
        }

        private void DropMissileAboveTile() {
            GetComponent<Renderer>().material = clearTile;

            var position = transform.position;
            var dropPosition = new Vector3(position.x, position.y + 7f, position.z);

            _missile = Instantiate(missilePrefab);
            _missile.transform.position = dropPosition;

            _missile.GetComponent<NetworkObject>().Spawn();
        }

        public void MissileHitTile() {
            _missileHit = true;
            if (_hasShip) {
                AudioSource.PlayClipAtPoint(explosion, transform.position, 0.8f);
                GetComponent<Renderer>().material = greenTile;
                ClientBoard.Instance.ShipHit(name);
            }
            else {
                AudioSource.PlayClipAtPoint(splash, transform.position, 1f);
                GetComponent<Renderer>().material = redTile;
            }
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
        
        // SETUP ------------------------------------------------------------------

        private void Setup() {
            _shipHovering = true;
            
            if (MouseOverTile()) {
                HoverShip();
            }
            
            if (IsShipOverTile()) {
                if (ClientBoard.Instance.CanPlaceShip()) {
                    GetComponent<Renderer>().material = greenTile;
            
                    if (MouseOverTile() && Input.GetMouseButtonDown(0)) {
                        ClientBoard.Instance.PlaceShip();
                        return;
                    }
                }
                else {
                    GetComponent<Renderer>().material = redTile;
                }
                return;
            }
            
            _shipHovering = false;
            GetComponent<Renderer>().material = clearTile;
        }

        public bool IsShipHovering() {
            return _shipHovering;
        }

        private void HoverShip() {
            if (ClientBoard.Instance.currentShip == null) return;
            var tilePosition = transform.position;
            var hoverPosition = ClientBoard.Instance.currentShip.GetComponent<ClientShip>().GetHoverPosition(tilePosition);
            ClientBoard.Instance.currentShip.transform.position = hoverPosition;
        }

        private bool IsShipOverTile() {
            if (_setupComplete) return false;
            var position = transform.position;
            var direction = new Vector3(position.x, position.y + 50f, position.z);
            return Physics.Raycast(position, direction, out var hit) && 
                   hit.collider.gameObject.name == ClientBoard.Instance.currentShip.name;
        }

        public void CompleteSetup(bool hasShip) {
            if (_setupComplete) return;
            if (_hasShip) return;
            
            _setupComplete = true;
            _shipHovering = false;
            _hasShip = hasShip;
            GetComponent<Renderer>().material = clearTile;
        }
    }
}
