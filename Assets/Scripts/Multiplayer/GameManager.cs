using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class GameManager : NetworkBehaviour {
        
        public static GameManager Instance;

        public NetworkVariable<bool> hostSetupComplete = new();
        public NetworkVariable<bool> clientSetupComplete = new();
        
        public NetworkVariable<int> hostHitCount = new();
        public NetworkVariable<int> clientHitCount = new();
        public NetworkVariable<int> hostSunkCount = new();
        public NetworkVariable<int> clientSunkCount = new();
        
        [SerializeField] public AudioClip win;
        [SerializeField] public AudioClip lose;
        
        public bool hostTurn;
        public bool turnTaken;

        public NetworkVariable<bool> hostWin = new();
        public NetworkVariable<bool> clientWin = new();
        
        private readonly char[] _shipGroupsDelimiter = { '[', ']' };
        private readonly char[] _tileDelimiter = { ',' };

        private bool _gameOver;

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
            if (!hostSetupComplete.Value || !clientSetupComplete.Value || _gameOver) return;
            
            CountHits();

            if (hostWin.Value) {
                GetComponent<AudioSource>().Stop();
                AudioSource.PlayClipAtPoint(NetworkManager.Singleton.IsHost ? win : lose, transform.position, 1f);
                GameHUD.Instance.HostWin();
                _gameOver = true;
                return;
            }
            
            if (clientWin.Value) {
                GetComponent<AudioSource>().Stop();
                AudioSource.PlayClipAtPoint(NetworkManager.Singleton.IsHost ? lose : win, transform.position, 1f);
                GameHUD.Instance.ClientWin();
                _gameOver = true;
                return;
            }

            if (!turnTaken) return;
            turnTaken = false;

            switch (hostTurn) {
                case true:
                    GameHUD.Instance.HostTurn();
                    MainCamera.Instance.MoveCamera(7f);
                    return;
                case false:
                    GameHUD.Instance.ClientTurn();
                    MainCamera.Instance.MoveCamera(-7f);
                    break;
            }
        }
        
        private void CountHits() {
            if (NetworkManager.Singleton.IsHost) {
                hostHitCount.Value = ClientBoard.Instance.HitCounts();
                hostSunkCount.Value = ClientBoard.Instance.SunkCounts();
                hostWin.Value = hostSunkCount.Value == 5;
                clientWin.Value = clientSunkCount.Value == 5;
            }
            else {
                SetClientHitCountServerRpc(HostBoard.Instance.HitCounts());
                SetClientSunkCountServerRpc(HostBoard.Instance.SunkCounts());
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SetClientHitCountServerRpc(int count) {
            clientHitCount.Value = count;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SetClientSunkCountServerRpc(int count) {
            clientSunkCount.Value = count;
            hostWin.Value = hostSunkCount.Value == 5;
            clientWin.Value = clientSunkCount.Value == 5;
        }

        /*
         * Progress to the next players turn, and synchronise.
         */
        public void TurnTaken() {
            CountHits();

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
        
        /*
         * When host turn taken, synchronise the client
         */
        [ClientRpc]
        private void HostTurnTakenClientRpc() {
            turnTaken = true;
            hostTurn = false;
        }
        
        /*
         * When client turn taken, synchronise the host
         */
        [ServerRpc(RequireOwnership = false)]
        private void ClientTurnTakenServerRpc() {
            turnTaken = true;
            hostTurn = true;
        }

        /*
         * When the host has finished placing ships, sends the ship locations to the client.
         */
        public void HostSetupCompleted(List<string[]> hostShipTiles) {
            hostSetupComplete.Value = true;
            HostSetupCompletedClientRpc(Serialize(hostShipTiles));
        }

        [ClientRpc]
        private void HostSetupCompletedClientRpc(string serializedShipTiles) {
            var deserialized = Deserialize(serializedShipTiles);
            HostBoard.Instance.ClientSetupCompleted(deserialized);
        }

        /*
         * When the client has finished placing ships, sends the ship locations to the host.
         */        
        public void ClientSetupCompleted(List<string[]> clientShipTiles) {
            ClientSetupCompletedServerRpc(Serialize(clientShipTiles));
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClientSetupCompletedServerRpc(string serializedShipTiles) {
            clientSetupComplete.Value = true;
            var deserialized = Deserialize(serializedShipTiles);
            ClientBoard.Instance.HostSetupCompleted(deserialized);
        }

        /*
         * Serializes a list of ship tile arrays to a string format.
         */
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

        /*
         * Deserializes a string into a list of ship tile arrays.
         */
        private List<string[]> Deserialize(string serialized) {
            var ships = serialized.Split(_shipGroupsDelimiter);
            ships = ships.Where(ship => !string.IsNullOrEmpty(ship)).ToArray();
            return ships.Select(ship => ship.Split(_tileDelimiter)).ToList();
        }
    }
}
