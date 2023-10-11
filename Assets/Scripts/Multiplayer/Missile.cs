using UnityEngine;

namespace Multiplayer {
    public class Missile : MonoBehaviour {
        
        public static Missile Instance;

        private bool _isFiring;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
            
            DisableMissile();
        }

        public void DropMissileOnTile(Vector3 tilePosition) {
            var dropPosition = new Vector3(tilePosition.x, tilePosition.y + 7f, tilePosition.z);
            transform.position = dropPosition;
            EnableMissile();
        }

        void OnCollisionEnter(Collision collision) {
            if (GameManager.Instance.hostTurn) {
                collision.gameObject.GetComponent<ClientTile>().MissileHit();
            }
            else {
                collision.gameObject.GetComponent<HostTile>().MissileHit();
            }
            
            DisableMissile();
        }

        public bool IsFiring() {
            return _isFiring;
        }
        
        private void EnableMissile() {
            GetComponent<Renderer>().enabled = true;
            gameObject.SetActive(true);
            _isFiring = true;
        }

        private void DisableMissile() {
            GetComponent<Renderer>().enabled = false;
            gameObject.SetActive(false);
            _isFiring = false;
        }
    }
}
