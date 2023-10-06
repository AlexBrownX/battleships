using Enemy;
using UnityEngine;

namespace Player {
    public class MissileScript : MonoBehaviour {

        public static MissileScript Instance;

        private GameObject _targetTile;
        private bool _isFiring;
    
        void Start() {
            Instance = this;
            DisableMissile();
        }

        void Update() {
            // if (!DetectHit()) return;
            // if (_targetTile == null) return;
            //
            // if (GameManager.Instance.IsPlayerTurn()) {
            //     _targetTile.GetComponent<EnemyTile>().HighlightTile();
            // }
            // else {
            //     _targetTile.GetComponent<PlayerTile>().HighlightTile();
            // }
            //
            // _targetTile = null;
            // DisableMissile();
            // GameManager.Instance.TurnTaken();
        }
        
        void OnCollisionEnter(Collision collision) {
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
    
        private bool DetectHit() {
            var missilePosition = transform.position;
            var direction = new Vector3(missilePosition.x, missilePosition.y - 1.5f, missilePosition.z);
            // Debug.DrawLine(missilePosition, direction, Color.red, 1);
            // return Physics.Raycast(missilePosition, direction);
            
            // var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // var raycastHits = new RaycastHit[5];
            // var hits = Physics.RaycastNonAlloc(ray, raycastHits);
            //
            // var direction2 = new Vector3(missilePosition.x, missilePosition.y - 0.7f, missilePosition.z);
            // Debug.DrawLine(missilePosition, direction2, Color.red, 1);
            //
            // RaycastHit[] m_Results = new RaycastHit[10];
            // int hits = Physics.RaycastNonAlloc(missilePosition, direction2, m_Results, Mathf.Infinity, ~0);
            //
            // for (int i = 0; i < hits; i++)
            // {
            //     Debug.Log("Hit " + m_Results[i].collider.gameObject.name);
            // }
            
            return Physics.Raycast(missilePosition, direction);
        }
    }
}
