using SinglePlayer.Enemy;
using SinglePlayer.Player;
using UnityEngine;

namespace SinglePlayer {
    public class MissileScript : MonoBehaviour {

        public static MissileScript Instance;

        private GameObject _targetTile;
        private bool _isFiring;
    
        void Start() {
            Instance = this;
            DisableMissile();
        }
        
        void OnCollisionEnter() {
            if (_targetTile == null) return;

            if (GameManager.Instance.IsPlayerTurn()) {
                _targetTile.GetComponent<EnemyTile>().HighlightTile();
            }
            else {
                _targetTile.GetComponent<PlayerTile>().HighlightTile();
            }
            
            _targetTile = null;
            DisableMissile();
            GameManager.Instance.TurnTaken();
        }

        public void DropMissileOnTile(GameObject targetTile) {
            _targetTile = targetTile;
            var tilePosition = targetTile.transform.position;
            var dropPosition = new Vector3(tilePosition.x, tilePosition.y + 7f, tilePosition.z);
            transform.position = dropPosition;

            if (GameManager.Instance.IsPlayerTurn()) {
                targetTile.GetComponent<EnemyTile>().MissileDroppedOnTile();
            }
            else {
                targetTile.GetComponent<PlayerTile>().MissileDroppedOnTile();
            }
            
            EnableMissile();
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