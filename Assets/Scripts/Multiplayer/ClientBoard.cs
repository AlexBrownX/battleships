using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class ClientBoard : NetworkBehaviour {

        public static ClientBoard Instance;

        [SerializeField] public GameObject[] ships = new GameObject[5];

        public GameObject currentShip;
        public List<List<KeyValuePair<string, bool>>> _shipLocations = new();

        private readonly GameObject[] _tiles = new GameObject[100];
        private int _shipIndex = -1;
        private bool _boardInView;
        private readonly List<string[]> _clientShipTiles = new();

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
            var shipIndex = 0;
            var shipTileNames = new string[currentShip.GetComponent<ClientShip>().shipSize];
            var shipTiles = _tiles.Where(tile => tile.GetComponent<ClientTile>().IsShipHovering()).ToArray();
            currentShip.GetComponent<ClientShip>().PlaceShip(shipTiles);

            foreach (var shipTile in shipTiles) {
                shipTile.GetComponent<ClientTile>().CompleteSetup(true);
                shipTileNames[shipIndex] = shipTile.name;
                shipIndex++;
            }
            
            _clientShipTiles.Add(shipTileNames);
            SetNextShip();
        }
        
        private void SetupCompleted() {
            currentShip = null;
            
            foreach (var tile in _tiles) {
                tile.GetComponent<ClientTile>().CompleteSetup(false);
            }
            
            Destroy(SetupHUD.Instance.gameObject);
            GameManager.Instance.ClientSetupCompleted(_clientShipTiles);
        }
        
        public void HostSetupCompleted(List<string[]> clientShipLocations) {
            foreach (var clientShipLocation in clientShipLocations) {
                List<KeyValuePair<string, bool>> ship = clientShipLocation.Select(shipTile => new KeyValuePair<string, bool>(shipTile, false)).ToList();
                _shipLocations.Add(ship);
            }
            
            foreach (var tile in _tiles) {
                var hasShip = clientShipLocations.SelectMany(shipLocation => shipLocation).Any(shipTile => shipTile == tile.name);
                tile.GetComponent<ClientTile>().CompleteSetup(hasShip);
            }
        }

        public void ShipHit(string tileHit) {
            UpdateShipLocations(tileHit);
            
        }

        private void UpdateShipLocations(string tileHit) {
            var updatedShipLocations = new List<List<KeyValuePair<string, bool>>>();
            
            foreach (var shipLocation in _shipLocations) {
                var updatedShitLocation = new List<KeyValuePair<string, bool>>();
                
                foreach (var tile in shipLocation) {
                    if (tile.Key == tileHit) {
                        var updatedTile = new KeyValuePair<string, bool>(tile.Key, true);
                        updatedShitLocation.Add(updatedTile);
                    }
                    else {
                        updatedShitLocation.Add(tile);
                    }
                }
                updatedShipLocations.Add(updatedShitLocation);
            }

            _shipLocations = updatedShipLocations;
        }

        public int HitCounts() {
            var hitCounts = 0;
            
            _shipLocations.ForEach(shipLocation => {
                hitCounts += shipLocation.Count(ship => ship.Value);
            });

            return hitCounts;
        }

        public int SunkCounts() {
            return _shipLocations.Count(shipLocation => shipLocation.Count == shipLocation.Count(ship => ship.Value));
        }
    }
}
