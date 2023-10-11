using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class ClientBoard : MonoBehaviour
    {
        public static ClientBoard Instance;

        [SerializeField] public GameObject[] tiles;
        [SerializeField] public GameObject[] ships;
        [SerializeField] public GameObject currentShip;
        
        private void Awake() {
            Debug.Log("Awake Client board");

            // TODO - TEMP
            // GameObject.Find("HostRotateBtn").gameObject.SetActive(false);

            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }
        
        void Start() {
            // TODO - TEMP
            return;
            if (NetworkManager.Singleton.IsHost) return;

            // Move camera right
            // PopulateTiles();
            // StartCoroutine(DelayDropShip());
        }

        
        public void RotateShip() {
            
        }
    }
}
