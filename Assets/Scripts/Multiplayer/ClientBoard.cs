using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class ClientBoard : NetworkBehaviour {

        public static ClientBoard Instance;

        [SerializeField] public GameObject[] ships = new GameObject[5];

        public GameObject currentShip;

        private readonly GameObject[] _tiles = new GameObject[100];
        private int _shipIndex = -1;
        private bool _boardInView;

        private void Awake() {
            if (!NetworkManager.Singleton.IsHost) {
                GameObject.Find("HostRotateBtn").gameObject.SetActive(false);
            }

            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }
        
        void Start() {
            if (NetworkManager.Singleton.IsHost) {
                PopulateTiles();
                return;
            }

            MainCamera.Instance.MoveCamera(7f);
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
            GameManager.Instance.ClientSetupCompletedServerRpc();
        }
        
        public void HostSetupCompleted() {
            foreach (var tile in _tiles) {
                tile.GetComponent<ClientTile>().CompleteSetup(null);
            }
        }
    }
}
