using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy {
    public class EnemyTile : MonoBehaviour {

        public Material tile;
        public Material greenTile;
        public Material redTile;
        public Material yellowTile;
        
        private bool _hasShip;
        private bool _missileDroppedOnTile;
        private bool _setupComplete;
        
        void Update() {
            if (!_setupComplete) return;
            if (_missileDroppedOnTile) return;
            if (!GameManager.Instance.IsPlayerTurn()) return;

            if (MouseOverTile() && !MissileScript.Instance.IsFiring()) {
                GetComponent<Renderer>().material = yellowTile;

                if (Input.GetMouseButtonDown(0)) {
                    DropMissileOnTile();
                }
                
                return;
            }

            GetComponent<Renderer>().material = tile;
        }

        private void DropMissileOnTile() {
            GetComponent<Renderer>().material = tile;
            MissileScript.Instance.DropMissileOnTile(gameObject);
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

        public void HighlightTile() {
            GetComponent<Renderer>().material = _hasShip ? greenTile : redTile;
        }

        public void CompleteSetup() {
            _setupComplete = true;
        }

        public void PlaceShipOnTile() {
            _hasShip = true;
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
