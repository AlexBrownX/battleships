using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Player {
    public class PlayerBoardSetup : MonoBehaviour {
    
        public static PlayerBoardSetup Instance;
        
        public GameObject[] tiles;
        public GameObject[] ships;
        public GameObject currentShip;
        public Button rotateBtn;

        private readonly List<GameObject[]> _playerTiles = new();
        private int _shipIndex = -1;

        void Start() {
            Instance = this;
            PopulateTiles();
            SetNextShip();
            rotateBtn.onClick.AddListener(RotateBtnClicked);
        }

        private void RotateBtnClicked() {
            currentShip.GetComponent<PlayerShip>().Rotate();
        }

        private void PopulateTiles() {
            tiles = new GameObject[100];
            for (var i = 0; i < tiles.Length; i++) {
                var tile = GameObject.Find($"Tile ({i + 1})");
                tiles[i] = tile;
            }
        }

        private void SetNextShip() {
            _shipIndex += 1;

            if (_shipIndex == ships.Length) {
                SetupCompleted();
                return;
            }
        
            currentShip = ships[_shipIndex];
            currentShip.GetComponent<Renderer>().enabled = true;
            currentShip.GetComponent<PlayerShip>().gameObject.SetActive(true);
        }

        public bool CanDrop() {
            var highlighted = tiles.Count(tile => tile.GetComponent<PlayerTile>().shipHovering);
            return currentShip.GetComponent<PlayerShip>().shipSize == highlighted;
        }
    
        public void ShipPlaced() {
            var shipIndex = 0;
            var shipTiles = new GameObject[currentShip.GetComponent<PlayerShip>().shipSize];

            foreach (var tile in tiles) {
                if (!tile.GetComponent<PlayerTile>().shipHovering) continue;
                tile.GetComponent<PlayerTile>().PlaceShipOnTile();
                shipTiles[shipIndex++] = tile;
            }
            
            _playerTiles.Add(shipTiles);
            // currentShip.GetComponent<PlayerShip>().SetupComplete();
            SetNextShip();
        }

        private void SetupCompleted() {
            foreach (var tile in tiles) {
                tile.GetComponent<PlayerTile>().CompleteSetup();
            }
            
            rotateBtn.gameObject.SetActive(false);
            currentShip = null;
            GameManager.Instance.PlayerCompleteSetup(_playerTiles);
        }
    }
}
