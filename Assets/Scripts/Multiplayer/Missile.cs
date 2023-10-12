using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class Missile : NetworkBehaviour {
        
        public static Missile Instance;

        private bool _isFiring;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        void OnCollisionEnter(Collision collision) {
            GetComponent<Renderer>().enabled = false;

            if (GameManager.Instance.hostTurn) {
                collision.gameObject.GetComponent<ClientTile>().MissileHitTile();
            }

            if (!GameManager.Instance.hostTurn) {
                collision.gameObject.GetComponent<HostTile>().MissileHitTile();
            }
            
            StartCoroutine(DelayedTakeTurn());
        }

        private IEnumerator DelayedTakeTurn() {
            yield return new WaitForSeconds(0.3f);
            
            if (GameManager.Instance.hostTurn && NetworkManager.Singleton.IsHost) {
                GameManager.Instance.TurnTaken();
                Despawn();
            }

            if (!GameManager.Instance.hostTurn && !NetworkManager.Singleton.IsHost) {
                GameManager.Instance.TurnTaken();
                DespawnServerRpc();
            }
        }
        
        private void Despawn() {
            GetComponent<NetworkObject>().Despawn();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DespawnServerRpc() {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
