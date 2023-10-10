using System;
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
    public class MultiplayerSetupManager : NetworkBehaviour {

        public static MultiplayerSetupManager Instance;
        
        [SerializeField] private GameObject multiplayerPanel;
        [SerializeField] private GameObject loadingImage;
        [SerializeField] private TextMeshProUGUI joinCodeOutput;
        [SerializeField] private TMP_InputField joinCodeInput;

        // TEMP
        private GameObject _cube;
        [SerializeField] private GameObject cubePrefab;
        private readonly int _spinSpeed = 50;
        private readonly Vector3 _spinDirection = new Vector3(0, 0, -1);

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
            
            ToggleLoadingImage(false);
        }

        private async void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback();
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback();
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void Update() {
            if (loadingImage.activeSelf) {
                loadingImage.gameObject.transform.Rotate(_spinDirection * (_spinSpeed * Time.deltaTime));
            }
        }

        private Action<ulong> OnClientConnectedCallback() {
            return clientId => {
                Debug.Log($"Client {clientId} connected");
                
                if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2) {
                    SpawnCube();
                    multiplayerPanel.SetActive(false);
                    HidePanelClientRpc();
                }
            };
        }

        private static Action<ulong> OnClientDisconnectCallback() {
            return clientId => {
                Debug.Log($"Client {clientId} disconnected");
            };
        }

        private void SpawnCube() {
            Debug.Log($"Spawn cube");
            _cube = Instantiate(cubePrefab);
            _cube.GetComponent<NetworkObject>().Spawn();
            DontDestroyOnLoad(_cube);
        }

        [ClientRpc]
        private void HidePanelClientRpc() {
            Debug.Log("Hiding panel");
            multiplayerPanel.SetActive(false);
        }

        public async void StartHost() {
            try {
                ToggleLoadingImage(true);
                var allocation = await RelayService.Instance.CreateAllocationAsync(1);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                joinCodeOutput.text = "Join Code: " + joinCode;

                var relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartHost();
            }
            catch (Exception exception) {
                ToggleLoadingImage(false);
                Debug.LogError(exception);
            }
        }

        public async void StartClient() {
            try {
                ToggleLoadingImage(true);
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);
                var relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception exception) {
                ToggleLoadingImage(false);
                Debug.LogError(exception);
            }
        }


        private void ToggleLoadingImage(bool active) {
            loadingImage.SetActive(active);
        }
        
        public void ExitScene() {
            SceneManager.LoadScene("Scenes/MainMenu/MainMenuScene");
        }
    }
}
