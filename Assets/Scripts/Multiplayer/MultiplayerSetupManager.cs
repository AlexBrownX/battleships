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
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Multiplayer {
    public class MultiplayerSetupManager : NetworkBehaviour {

        public static MultiplayerSetupManager Instance;

        [SerializeField] private GameObject startHostBtn;
        [SerializeField] private GameObject startClientBtn;
        
        [SerializeField] private GameObject multiplayerPanel;
        [SerializeField] private GameObject loadingImage;
        [SerializeField] private TextMeshProUGUI joinCodeOutput;
        [SerializeField] private TMP_InputField joinCodeInput;
        [SerializeField] private GameObject gamePrefab;

        private GameObject _game;
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
                // Debug.Log($"Client {clientId} connected");
                
                if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2) {
                    SpawnGame();
                    multiplayerPanel.SetActive(false);
                    HidePanelClientRpc();
                }
            };
        }

        private static Action<ulong> OnClientDisconnectCallback() {
            return _ => {
                SceneManager.LoadScene("Scenes/MainMenu/MainMenuScene");
            };
        }

        private void SpawnGame() {
            _game = Instantiate(gamePrefab);
            _game.GetComponent<NetworkObject>().Spawn();
        }

        [ClientRpc]
        private void HidePanelClientRpc() {
            multiplayerPanel.SetActive(false);
        }

        public async void StartHost() {
            try {
                startHostBtn.GetComponentInChildren<TMP_Text>().text = "Generating Join Code";

                ToggleLoadingImage(true);
                var allocation = await RelayService.Instance.CreateAllocationAsync(1);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                joinCodeOutput.text = "Join Code: " + joinCode;
                startHostBtn.GetComponentInChildren<TMP_Text>().text = "Send To Player 2";

                var relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartHost();
            }
            catch (Exception exception) {
                startHostBtn.GetComponentInChildren<TMP_Text>().text = "Host Game";
                ToggleLoadingImage(false);
                Debug.LogError(exception);
            }
        }

        public async void StartClient() {
            try {
                startClientBtn.GetComponentInChildren<TMP_Text>().text = "Joining Game";
                ToggleLoadingImage(true);
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);
                var relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception exception) {
                startClientBtn.GetComponentInChildren<TMP_Text>().text = "Join Game";
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
