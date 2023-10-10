using Unity.Netcode;
using UnityEngine;

namespace Scenes.Multiplayer {
    public class CubeScript : NetworkBehaviour {
        
        public NetworkVariable<bool> clockwise = new(true);
        
        [SerializeField] public Material greenTile;
        [SerializeField] public Material redTile;

        private readonly int _spinSpeed = 50;
        private readonly Vector3 _clockwiseSpin = new Vector3(0, 1, 0);
        private readonly Vector3 _antiClockwiseSpin = new Vector3(0, -1, 0);

        private void OnMouseDown() {
            /*
             * Host changes direction, networkVariable to synchronise to client
             */
            if (NetworkManager.Singleton.IsHost) {
                clockwise.Value = !clockwise.Value;
                GetComponent<Renderer>().material = greenTile;
                ChangeCubeColorClientRpc();
                return;
            }

            /*
             * Client changes direction, sends ServerRpc to synchronise to host
             */
            GetComponent<Renderer>().material = redTile;
            ChangeDirectionServerRpc();
        }

        void Update() {
            if (clockwise.Value) {
                gameObject.transform.Rotate(_clockwiseSpin * (_spinSpeed * Time.deltaTime));
            }
            else {
                gameObject.transform.Rotate(_antiClockwiseSpin * (_spinSpeed * Time.deltaTime));
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeDirectionServerRpc() {
            GetComponent<Renderer>().material = redTile;
            clockwise.Value = !clockwise.Value;
        }
        
        
        [ClientRpc]
        private void ChangeCubeColorClientRpc() {
            GetComponent<Renderer>().material = greenTile;
        }
    }
}
