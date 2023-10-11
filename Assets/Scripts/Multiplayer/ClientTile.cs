using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Multiplayer {
    public class ClientTile : NetworkBehaviour {

        [SerializeField] public Material clearTile;
        [SerializeField] public Material redTile;
        [SerializeField] public Material greenTile;
        [SerializeField] public Material yellowTile;

        private GameObject _ship;
        private bool _setupComplete;
        private bool _shipHovering;

        void Start() {
            GetComponent<Rigidbody>().maxLinearVelocity = Random.Range(1.5f, 3.0f);
        }

        private void Update() {
            if (!_setupComplete) {
                Setup();
                return;
            }

            if (NetworkManager.Singleton.IsHost && 
                GameManager.Instance.hostTurn && 
                !Missile.Instance.IsFiring() &&
                MouseOverTile()) {
                
                GetComponent<Renderer>().material = yellowTile;

                if (Input.GetMouseButtonDown(0)) {
                    DropMissile();
                }
                
                return;             
            }
            
            GetComponent<Renderer>().material = clearTile;
        }

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

        private void DropMissile() {
            GetComponent<Renderer>().material = clearTile;
            Missile.Instance.DropMissileOnTile(transform.position);
        }

        public void MissileHit() {
            GameManager.Instance.TurnTaken();
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
        
        private void HoverShip() {
            if (ClientBoard.Instance.currentShip == null) return;
            var tilePosition = transform.position;
            var hoverPosition = ClientBoard.Instance.currentShip.GetComponent<ClientShip>().GetHoverPosition(tilePosition);
            ClientBoard.Instance.currentShip.transform.position = hoverPosition;
        }

        public bool IsShipHovering() {
            return _shipHovering;
        }
        
        private bool IsShipOverTile() {
            if (_setupComplete) return false;
            var position = transform.position;
            var direction = new Vector3(position.x, position.y + 50f, position.z);
            return Physics.Raycast(position, direction, out var hit) && 
                   hit.collider.gameObject.name == ClientBoard.Instance.currentShip.name;
        }

        public void CompleteSetup(GameObject ship) {
            if (_setupComplete) return;
            _setupComplete = true;
            _shipHovering = false;
            _ship = ship;
            GetComponent<Renderer>().material = clearTile;
        }
    }
}
