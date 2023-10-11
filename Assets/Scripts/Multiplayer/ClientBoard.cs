using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class ClientBoard : MonoBehaviour {

        public static ClientBoard Instance;

        [SerializeField] public GameObject[] ships = new GameObject[5];

        public GameObject currentShip;

        private readonly GameObject[] _tiles = new GameObject[100];
        private int _shipIndex = -1;

        private void Awake() {
            Debug.Log("Awake Client board");

            // TODO - TEMP
            GameObject.Find("HostRotateBtn").gameObject.SetActive(false);

            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }
        
        void Start() {
            // TODO - TEMP
            //return;
            // if (NetworkManager.Singleton.IsHost) return;

            // Move camera right
            PopulateTiles();
            StartCoroutine(DelayDropShip());
        }
        
        private void PopulateTiles() {
            for (var i = 0; i < _tiles.Length; i++) {
                _tiles[i] = GameObject.Find($"ClientTile{i}");
            }
        }
        
        private IEnumerator DelayDropShip() {
            yield return new WaitForSeconds(2);
            SetNextShip();
        }

        private void SetNextShip() {
            _shipIndex += 1;

            if (_shipIndex == ships.Length) {
                SetupCompleted();
                return;
            }
        
            currentShip = ships[_shipIndex];
            currentShip.GetComponent<Renderer>().enabled = true;
            currentShip.gameObject.SetActive(true);
        }

        public void RotateShip() {
            currentShip.GetComponent<ClientShip>().RotateShip();
        }

        public bool CanPlaceShip() {
            var highlighted = _tiles.Count(tile => tile.GetComponent<ClientTile>().IsShipHovering());
            return currentShip.GetComponent<ClientShip>().shipSize == highlighted;
        }
        
        public void PlaceShip() {
            var shipTiles = _tiles.Where(tile => tile.GetComponent<ClientTile>().IsShipHovering()).ToArray();
            currentShip.GetComponent<ClientShip>().PlaceShip(shipTiles);

            foreach (var shipTile in shipTiles) {
                shipTile.GetComponent<ClientTile>().CompleteSetup(currentShip);
            }
            
            SetNextShip();
        }
        
        private void SetupCompleted() {
            currentShip = null;
            
            foreach (var tile in _tiles) {
                tile.GetComponent<ClientTile>().CompleteSetup(null);
            }
            
            Destroy(SetupHUD.Instance.gameObject);
            GameManager.Instance.ClientSetupCompleted();
        }
    }
}
