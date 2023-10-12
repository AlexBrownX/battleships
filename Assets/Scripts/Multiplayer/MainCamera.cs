using UnityEngine;

namespace Multiplayer {
    public class MainCamera : MonoBehaviour {
        
        public static MainCamera Instance;

        private readonly float _cameraSpeed = 10f;
        private bool _isMoving;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        
        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        private void Update() {
            if (!_isMoving) return;
            
            _startPosition = Camera.main.transform.position;
            Camera.main.transform.position = Vector3.MoveTowards(_startPosition, _endPosition, _cameraSpeed * Time.deltaTime);
            if (Camera.main.transform.position.Equals(_endPosition)) {
                _isMoving = false;
            }
        }

        public void MoveCamera(float xAxis) {
            _isMoving = true;
            _startPosition = Camera.main.transform.position;
            _endPosition = new Vector3(xAxis, _startPosition.y,  _startPosition.z);
        }
    }
}
