using System;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class GameManager : NetworkBehaviour {
        
        public static GameManager Instance;

        private bool _hostSetupComplete;
        private bool _clientSetupComplete;

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
            if (!_hostSetupComplete || !_clientSetupComplete) return;
        }

        public void HostSetupCompleted() {
            _hostSetupComplete = true;
        }
        
        public void ClientSetupCompleted() {
            _clientSetupComplete = true;
        }
    }
}
