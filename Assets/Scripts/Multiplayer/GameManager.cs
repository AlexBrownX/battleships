using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class GameManager : NetworkBehaviour {
        
        public static GameManager Instance;

        public NetworkVariable<bool> hostSetupComplete = new();
        public NetworkVariable<bool> clientSetupComplete = new();
        
        public bool hostTurn;
        public bool turnTaken;
        
        private char[] _shipGroupsDelimiter = { '[', ']' };
        char[] _tileDelimiter = { ',' };

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

        public void HostSetupCompleted(List<string[]> hostShipTiles) {
            hostSetupComplete.Value = true;
            HostSetupCompletedClientRpc(Serialize(hostShipTiles));
        }

        [ClientRpc]
        private void HostSetupCompletedClientRpc(string serializedShipTiles) {
            var deserialized = Deserialize(serializedShipTiles);
            HostBoard.Instance.ClientSetupCompleted(deserialized);
        }

        public void ClientSetupCompleted(List<string[]> clientShipTiles) {
            ClientSetupCompletedServerRpc(Serialize(clientShipTiles));
        }

        [ServerRpc(RequireOwnership = false)]
        public void ClientSetupCompletedServerRpc(string serializedShipTiles) {
            clientSetupComplete.Value = true;
            var deserialized = Deserialize(serializedShipTiles);
            ClientBoard.Instance.HostSetupCompleted(deserialized);
        }

        private string Serialize(List<string[]> hostShipTiles) {
            var serialized = "";
            foreach (var ship in hostShipTiles) {
                serialized += "[";
                foreach (var shipTile in ship) {
                    serialized +=  shipTile + ",";
                }
                serialized = serialized.Remove(serialized.Length - 1, 1);
                serialized += "]";
            }

            return serialized;
        }

        private List<string[]> Deserialize(string serialized) {
            var ships = serialized.Split(_shipGroupsDelimiter);
            ships = ships.Where(ship => !string.IsNullOrEmpty(ship)).ToArray();
            return ships.Select(ship => ship.Split(_tileDelimiter)).ToList();
        }
    }
}
