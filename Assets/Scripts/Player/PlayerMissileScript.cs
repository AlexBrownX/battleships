using Enemy;
using UnityEngine;

namespace Player {
    public class PlayerMissileScript : MonoBehaviour {

        public static PlayerMissileScript Instance;
    
        private GameObject _targetTile;
    
        void Start() {
            Instance = this;
            DisableMissile();
        }

        void Update() {
            if (DetectHit()) {
                _targetTile.GetComponent<EnemyTileSetup>().HighlightTile();
                _targetTile = null;
                DisableMissile();
            }
        }

        public void DropMissileOnTile(GameObject targetTile) {
            _targetTile = targetTile;
            var tilePosition = targetTile.transform.position;
            var dropPosition = new Vector3(tilePosition.x, tilePosition.y + 7f, tilePosition.z);
            transform.position = dropPosition;
            EnableMissile();
        }

        private void EnableMissile() {
            GetComponent<Renderer>().enabled = true;
            gameObject.SetActive(true);
        }

        private void DisableMissile() {
            GetComponent<Renderer>().enabled = false;
            gameObject.SetActive(false);
        }
    
        private bool DetectHit() {
            var missilePosition = transform.position;
            var direction = new Vector3(missilePosition.x, missilePosition.y - 1f, missilePosition.z);
            return Physics.Raycast(missilePosition, direction);
        }
    }
}
