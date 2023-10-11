using Unity.Netcode;

namespace Multiplayer {
    public class SetupHUD : NetworkBehaviour {
        
        public static SetupHUD Instance;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        public void HostRotateClick() {
            HostBoard.Instance.RotateShip();
        }
        
        public void ClientRotateClick() {
            ClientBoard.Instance.RotateShip();
        }
    }
}
