using UnityEngine;

namespace GameSetup {
    public class EnemyTileSetup : MonoBehaviour {

        public Material tile;
        public Material greenTile;
        public Material redTile;
        public Material yellowTile;
        public bool hasShip;

        private bool _missileDroppedOnTile;
        private bool _setupComplete;
        
        void Update() {
            if (!_setupComplete) return;
            if (_missileDroppedOnTile) return;

            if (MouseOverTile()) {
                GetComponent<Renderer>().material = yellowTile;

                if (Input.GetMouseButtonDown(0)) {
                    DropMissileOnTile();
                    GameManager.Instance.PlayerTurnTaken();
                }
                
                return;
            }

            GetComponent<Renderer>().material = tile;
        }

        private void DropMissileOnTile() {
            _missileDroppedOnTile = true;
            GetComponent<Renderer>().material = tile;
            PlayerMissileScript.Instance.DropMissileOnTile(gameObject);
        }

        public void HighlightTile() {
            GetComponent<Renderer>().material = hasShip ? greenTile : redTile;
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
