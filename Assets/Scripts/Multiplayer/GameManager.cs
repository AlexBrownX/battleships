using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer {
    public class GameManager : MonoBehaviour {

        public static GameManager Instance;

        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private TextMeshProUGUI joinCodeOutput;
        [SerializeField] private TMP_InputField joinCodeInput;

        private GameObject _cube;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        private async void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += clientId => {
                Debug.Log($"Client {clientId} connected");
                
                if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2) {
                    Debug.Log($"Spawn cube");
                    SpawnCube();
                }
            };
            
            NetworkManager.Singleton.OnClientDisconnectCallback += clientId => {
                Debug.Log($"Client {clientId} disconnected");
                ExitScene();
            };

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        
        private void SpawnCube() {
            _cube = Instantiate(cubePrefab);
            _cube.GetComponent<NetworkObject>().Spawn();
        }

        public async void StartHost() {
            try {
                var allocation = await RelayService.Instance.CreateAllocationAsync(1);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                joinCodeOutput.text = "Join Code: " + joinCode;

                var relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartHost();
            }
            catch (RelayServiceException exception) {
                Debug.LogError(exception);
            }
        }

        public async void StartClient() {
            try {
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);
                var relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException exception) {
                Debug.LogError(exception);
            }
        }

        public void ExitScene() {
            SceneManager.LoadScene("Scenes/MainMenu/MainMenuScene");
        }
    }
}
