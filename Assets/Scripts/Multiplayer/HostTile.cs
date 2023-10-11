using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Multiplayer {
    public class HostTile : NetworkBehaviour {

        [SerializeField] public Material clearTile;
        [SerializeField] public Material redTile;
        [SerializeField] public Material greenTile;

        private GameObject _ship;
        private bool _setupComplete;
        private bool _shipHovering;

        void Start() {
            GetComponent<Rigidbody>().maxLinearVelocity = Random.Range(1.5f, 3.0f);
        }

        private void Update() {
            Setup();
            
            if (!GameManager.Instance.hostTurn.Value && MouseOverTile() && Input.GetMouseButtonDown(0)) {
                Debug.Log($"Clicked {name}");
                DropBomb();
            }
        }

        private void DropBomb() {
            GameManager.Instance.TurnTaken();
        }

        private void Setup() {
            if (_setupComplete) return;
            _shipHovering = true;
            
            if (MouseOverTile()) {
                HoverShip();
            }
            
            if (IsShipOverTile()) {
                if (HostBoard.Instance.CanPlaceShip()) {
                    GetComponent<Renderer>().material = greenTile;
            
                    if (MouseOverTile() && Input.GetMouseButtonDown(0)) {
                        HostBoard.Instance.PlaceShip();
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
        
        private bool MouseOverTile() {
            if (Camera.main == null) return false;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var raycastHits = new RaycastHit[5];
            var hits = Physics.RaycastNonAlloc(ray, raycastHits);

            for (var i = 0; i < hits; i++) {
                if (raycastHits[i].collider.gameObject.name == name) {
                    // Debug.Log($"Tile ray cast hit {raycastHits[i].collider.gameObject.name}");
                    return true;
                }
            }

            return false;
        }
        
        private void HoverShip() {
            if (HostBoard.Instance.currentShip == null) return;
            var tilePosition = transform.position;
            var hoverPosition = HostBoard.Instance.currentShip.GetComponent<HostShip>().GetHoverPosition(tilePosition);
            HostBoard.Instance.currentShip.transform.position = hoverPosition;
        }

        public bool IsShipHovering() {
            return _shipHovering;
        }
        
        public bool IsShipOverTile() {
            if (_setupComplete) return false;
            var position = transform.position;
            var direction = new Vector3(position.x, position.y + 50f, position.z);
            return Physics.Raycast(position, direction, out var hit) && 
                   hit.collider.gameObject.name == HostBoard.Instance.currentShip.name;
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
