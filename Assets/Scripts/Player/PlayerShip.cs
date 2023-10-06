using Enemy;
using UnityEngine;

namespace Player {
    public class PlayerShip : MonoBehaviour {

        public AudioClip splash;
        
        public float zOffset;
        public float zRotatedOffset;
        public float xOffset;
        public float xRotatedOffset;
        public int shipSize;
        
        private bool _rotated;
        private bool _sunk;
        private int _hits;

        private void Awake() {
            GetComponent<Renderer>().enabled = false;
            gameObject.SetActive(false);
        }
        
        public void MissileHit() {
            _hits++;
            if (_hits == shipSize) {
                _sunk = true;
                GameManager.Instance.GetComponent<EnemyPlayer>().PlayerShipSunk();
            }
        }

        public bool IsSunk() {
            return _sunk;
        }
        
        public float GetZOffset() {
            return _rotated ? zRotatedOffset : zOffset;
        }
    
        public float GetXOffset() {
            return _rotated ? xRotatedOffset : xOffset;
        }
        
        public void Rotate() {
            if (_rotated) {
                gameObject.transform.Rotate(0, 90, 0);
                _rotated = false;
            }
            else {
                gameObject.transform.Rotate(0, -90, 0);
                _rotated = true;
            }
        }
        
        void OnCollisionEnter() {
            AudioSource.PlayClipAtPoint(splash, transform.position, 0.1f);
        }
    }
}
