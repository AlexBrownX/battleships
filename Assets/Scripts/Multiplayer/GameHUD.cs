using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer {
    public class GameHUD : NetworkBehaviour {
    
        public static GameHUD Instance;

        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI hostHitCount;
        [SerializeField] private TextMeshProUGUI hostSunkCount;
        [SerializeField] private TextMeshProUGUI clientHitCount;
        [SerializeField] private TextMeshProUGUI clientSunkCount;

        private bool _setupComplete;
        
        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }

            headerText.text = "Place your battleships";
            hostHitCount.text = "";
            hostSunkCount.text = "";
            clientHitCount.text = "";
            clientSunkCount.text = "";
        }
        
        private void Update() {
            if (!_setupComplete) return;
            hostHitCount.text = $"Player 1 hits: {GameManager.Instance.hostHitCount.Value}";
            hostSunkCount.text = $"Player 1 sunk: {GameManager.Instance.hostSunkCount.Value}";
            clientHitCount.text = $"Player 2 hits: {GameManager.Instance.clientHitCount.Value}";
            clientSunkCount.text = $"Player 2 sunk: {GameManager.Instance.clientSunkCount.Value}";
        }

        public void SetupComplete() {
            _setupComplete = true;
            headerText.text = NetworkManager.Singleton.IsHost ? "Waiting for Player 2" : "Waiting for Player 1";
        }
        
        public void ExitScene() {
            SceneManager.LoadScene("Scenes/MainMenu/MainMenuScene");
        }

        public void HostTurn() {
            headerText.text = NetworkManager.Singleton.IsHost ? "Choose target" : "Player 1 targeting...";
        }

        public void ClientTurn() {
            headerText.text = NetworkManager.Singleton.IsHost ? "Player 2 targeting..." : "Choose target";
        }
        
        public void HostWin() {
            headerText.text = NetworkManager.Singleton.IsHost ? "Winner!" : "Loser!";
        }

        public void ClientWin() {
            headerText.text = NetworkManager.Singleton.IsHost ? "Loser!" : "Winner!";
        }
    }
}
