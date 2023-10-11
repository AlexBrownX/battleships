using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class GameManager : NetworkBehaviour {
        
        public static GameManager Instance;

        public NetworkVariable<bool> hostSetupComplete = new();
        public NetworkVariable<bool> clientSetupComplete = new();
        
        public bool hostTurn;
        public bool turnTaken;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
            
            
            hostTurn = true;
            turnTaken = true;
        }

        private void Update() {
            if (!hostSetupComplete.Value || !clientSetupComplete.Value) return;
            if (!turnTaken) return;
            
            turnTaken = false;
            
            if (hostTurn) {
                Debug.Log("Host turn");
                MainCamera.Instance.MoveCamera(7f);
                return;
            }

            if (!hostTurn) {
                Debug.Log("Client turn");
                MainCamera.Instance.MoveCamera(-7f);
                return;
            }
        }

        public void TurnTaken() {
            if (NetworkManager.Singleton.IsHost) {
                turnTaken = true;
                hostTurn = false;
                HostTurnTakenClientRpc();
            }
            else {
                turnTaken = true;
                hostTurn = true;
                ClientTurnTakenServerRpc();
            }
        }
        
        [ClientRpc]
        private void HostTurnTakenClientRpc() {
            turnTaken = true;
            hostTurn = false;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void ClientTurnTakenServerRpc() {
            turnTaken = true;
            hostTurn = true;
        }

        public void HostSetupCompleted() {
            hostSetupComplete.Value = true;
            HostSetupCompletedClientRpc();
        }
        
        [ClientRpc]
        private void HostSetupCompletedClientRpc() {
            HostBoard.Instance.ClientSetupCompleted();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void ClientSetupCompletedServerRpc() {
            clientSetupComplete.Value = true;
            ClientBoard.Instance.HostSetupCompleted();
        }
    }
}
