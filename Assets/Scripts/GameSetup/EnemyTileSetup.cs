using UnityEngine;

namespace GameSetup {
    public class EnemyTileSetup : MonoBehaviour {

        public Material tile;
        public Material yellowTile;
        public bool hasShip;

        private bool _setupComplete;

        void Start()
        {

        }

        void Update() {
            if (!_setupComplete) return;

            if (MouseOverTile()) {
                GetComponent<Renderer>().material = yellowTile;
                return;
            }

            GetComponent<Renderer>().material = tile;
        }
    
        public void CompleteSetup() {
            _setupComplete = true;
        }

        public void PlaceShipOnTile() {
            hasShip = true;
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
}
