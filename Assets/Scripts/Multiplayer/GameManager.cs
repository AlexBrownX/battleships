using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class GameManager : MonoBehaviour {

        public static GameManager Instance;

        [SerializeField]
        private GameObject cubePrefab;

        private GameObject _cube;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        private void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += clientId => {
                Debug.Log($"Client {clientId} connected");
                
                if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2) {
                    Debug.Log($"Spawn cube");
                    SpawnCube();
                }
            };
            
            NetworkManager.Singleton.OnClientDisconnectCallback += clientId => {
                Debug.Log($"Client {clientId} disconnected");
            };
        }
        
        private void SpawnCube() {
            _cube = Instantiate(cubePrefab);
            _cube.GetComponent<NetworkObject>().Spawn();
        }

        public void StartHost() {
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient() {
            NetworkManager.Singleton.StartClient();
        }
    }
}
