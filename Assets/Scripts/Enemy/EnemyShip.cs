using UnityEngine;

namespace Enemy {
    public class EnemyShip : MonoBehaviour {

        public float zOffset;
        public float zRotatedOffset;
        public float xOffset;
        public float xRotatedOffset;
        public int shipSize;

        private bool _setupComplete = false;
        private bool _rotated = false;
        private bool _sunk;
        private int _hits;
        
        private void Awake() {
            GetComponent<Renderer>().enabled = false;
            gameObject.SetActive(false);
        }

        void Update() {
            if (_setupComplete) return;
        }

        public void MissileHit() {
            _hits++;
            if (_hits == shipSize) {
                _sunk = true;
                // GetComponent<Renderer>().enabled = true;
                // GetComponent<EnemyShip>().gameObject.SetActive(true);
            }
        }
        
        public bool IsSunk() {
            return _sunk;
        }

        public void SetRotated(bool rotated) {
            _rotated = rotated;
        }

        public float GetZOffset() {
            return _rotated ? zRotatedOffset : zOffset;
        }
    
        public float GetXOffset() {
            return _rotated ? xRotatedOffset : xOffset;
        }

        public void SetupComplete() {
            _setupComplete = true;
            // Debug.Log($"{name} setup complete");
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
    }
}
