using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class HostBoard : NetworkBehaviour {
        
        public static HostBoard Instance;

        [SerializeField] public AudioClip bigExplosion;
        [SerializeField] public GameObject[] ships = new GameObject[5];

        public GameObject currentShip;

        private List<List<KeyValuePair<string, bool>>> _shipLocations = new();
        private readonly GameObject[] _tiles = new GameObject[100];
        private int _shipIndex = -1;
        private bool _boardInView;
        private bool _missileActive;
        
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
            currentShip.GetComponent<HostShip>().PlaceShip();

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
            GameHUD.Instance.SetupComplete();
            GameManager.Instance.HostSetupCompleted(_hostShipTiles);
        }

        public void ClientSetupCompleted(List<string[]> hostShipLocations) {
            foreach (var hostShipLocation in hostShipLocations) {
                List<KeyValuePair<string, bool>> ship = hostShipLocation.Select(shipTile => new KeyValuePair<string, bool>(shipTile, false)).ToList();
                _shipLocations.Add(ship);
            }

            foreach (var tile in _tiles) {
                var hasShip = hostShipLocations.SelectMany(shipLocation => shipLocation).Any(shipTile => shipTile == tile.name);
                tile.GetComponent<HostTile>().CompleteSetup(hasShip);
            }        
        }
        
        public void ShipHit(string tileHit) {
            var sunkCountBefore = SunkCounts();
            UpdateShipLocations(tileHit);
            var sunkCountAfter = SunkCounts();

            if (sunkCountBefore != sunkCountAfter) {
                AudioSource.PlayClipAtPoint(bigExplosion, transform.position, 1f);
            }
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
        
        public bool IsMissileActive() {
            return _missileActive;
        }
        
        public void MissileStart() {
            _missileActive = true;
        }

        public void MissileEnd() {
            _missileActive = false;
        }
    }
}
