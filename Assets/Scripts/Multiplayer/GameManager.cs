using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class GameManager : NetworkBehaviour {
        
        public static GameManager Instance;

        public NetworkVariable<bool> hostSetupComplete = new();
        public NetworkVariable<bool> clientSetupComplete = new();
        public NetworkVariable<bool> hostTurn = new(true);

        private bool _turnTaken = true;
        
        private void Awake() {
            Debug.Log("Awake Game Manager");
            
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        private void Update() {
            if (!hostSetupComplete.Value || !clientSetupComplete.Value) return;
            if (!_turnTaken) return;

            if (hostTurn.Value) {
                Debug.Log("Host turn");
                HostBoard.Instance.HostTurn();
            }
            else {
                Debug.Log("Client turn");
                ClientBoard.Instance.ClientTurn();
            }
        }

        public void TurnTaken() {
            _turnTaken = true;

            if (NetworkManager.Singleton.IsHost) {
                HostTurnTakenClientRpc();
            }
            else {
                ClientTurnTakenServerRpc();
            }
        }
        
        [ClientRpc]
        private void HostTurnTakenClientRpc() {
            hostTurn.Value = false;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void ClientTurnTakenServerRpc() {
            hostTurn.Value = true;
        }

        [ClientRpc]
        public void HostSetupCompletedClientRpc() {
            hostSetupComplete.Value = true;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void ClientSetupCompletedServerRpc() {
            clientSetupComplete.Value = true;
        }
    }
}
