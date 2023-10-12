using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class HostBoard : NetworkBehaviour {
        
        public static HostBoard Instance;

        [SerializeField] public GameObject[] ships = new GameObject[5];

        public GameObject currentShip;

        private readonly GameObject[] _tiles = new GameObject[100];
        private int _shipIndex = -1;
        private bool _boardInView;
        private readonly List<string[]> _hostShipTiles = new();

        private void Awake() {
            if (NetworkManager.Singleton.IsHost) {
                GameObject.Find("ClientRotateBtn").gameObject.SetActive(false);
            }

            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        void Start() {
            if (!NetworkManager.Singleton.IsHost) {
                PopulateTiles();
                return;
            }
            MainCamera.Instance.MoveCamera(-7f);
            PopulateTiles();
            StartCoroutine(DelayDropShip());
        }

        private void PopulateTiles() {
            for (var i = 0; i < _tiles.Length; i++) {
                _tiles[i] = GameObject.Find($"HostTile{i}");
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
            currentShip.GetComponent<HostShip>().RotateShip();
        }
        
        public bool CanPlaceShip() {
            var highlighted = _tiles.Count(tile => tile.GetComponent<HostTile>().IsShipHovering());
            return currentShip.GetComponent<HostShip>().shipSize == highlighted;
        }
        
        public void PlaceShip() {
            var shipIndex = 0;
            var shipTileNames = new string[currentShip.GetComponent<HostShip>().shipSize];
            var shipTiles = _tiles.Where(tile => tile.GetComponent<HostTile>().IsShipHovering()).ToArray();
            currentShip.GetComponent<HostShip>().PlaceShip(shipTiles);

            foreach (var shipTile in shipTiles) {
                shipTile.GetComponent<HostTile>().CompleteSetup(currentShip);
                shipTileNames[shipIndex] = shipTile.name;
                shipIndex++;
            }
            
            _hostShipTiles.Add(shipTileNames);
            SetNextShip();
        }
        
        private void SetupCompleted() {
            currentShip = null;
            
            foreach (var tile in _tiles) {
                tile.GetComponent<HostTile>().CompleteSetup(false);
            }
            
            Destroy(SetupHUD.Instance.gameObject);
            GameManager.Instance.HostSetupCompleted(_hostShipTiles);
        }

        public void ClientSetupCompleted(List<string[]> hostShipLocations) {
            foreach (var tile in _tiles) {
                var hasShip = hostShipLocations.SelectMany(shipLocation => shipLocation).Any(shipTile => shipTile == tile.name);
                tile.GetComponent<HostTile>().CompleteSetup(hasShip);
            }        
        }
    }
}
