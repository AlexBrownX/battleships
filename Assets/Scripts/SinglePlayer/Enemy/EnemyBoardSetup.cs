using System.Collections.Generic;
using UnityEngine;

namespace SinglePlayer.Enemy {
    public class EnemyBoardSetup : MonoBehaviour
    {
        public static EnemyBoardSetup Instance;
        
        public GameObject[] tiles;
        public GameObject[] ships;
        public GameObject currentShip;

        private readonly List<GameObject[]> _enemyTiles = new();
        private int _shipIndex = -1;
        private readonly int _minTile = 101;
        private readonly int _maxTile = 200;
        private bool _setupComplete;

        void Start() {
            Instance = this;
            PopulateTiles();
            SetNextShip();
        }

        void Update() {
            if (!_setupComplete) return;
            GameManager.Instance.EnemyCompleteSetup(_enemyTiles);
        }
        
        private void PopulateTiles() {
            tiles = new GameObject[100];
            for (var i = 0; i < tiles.Length; i++) {
                var tile = GameObject.Find($"Tile ({i + 101})");
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

            do { } while (!PlaceShip());
            SetNextShip();
        }

        private bool PlaceShip() {
            var tileNumber = Random.Range(_minTile, _maxTile);
            var isRotated = Random.value > 0.5f;
            var shipSize = currentShip.GetComponent<EnemyShip>().shipSize;
            var tile = GameObject.Find($"Tile ({tileNumber})");
            var shipTiles = new GameObject[shipSize];
            
            for (var i = 0; i < shipSize; i++) {
                var tileIndex = isRotated ? tileNumber + i : tileNumber + (i * 10);

                if (tileIndex > _maxTile) {
                    return false;
                }
                
                if (isRotated && IsOffGrid(tileNumber, tileIndex)) {
                    return false;
                }

                var shipTile = GameObject.Find($"Tile ({tileIndex})");

                if (shipTile.GetComponent<EnemyTile>().HasShip()) {
                    return false;
                }
                
                shipTiles[i] = shipTile;
            }

            _enemyTiles.Add(shipTiles);

            foreach (var shipTile in shipTiles) {
                shipTile.GetComponent<EnemyTile>().PlaceShipOnTile();
            }

            if (isRotated) {
                currentShip.GetComponent<EnemyShip>().SetRotated(true);
                currentShip.transform.Rotate(0, 90, 0);
            }
            
            var zOffset = currentShip.GetComponent<EnemyShip>().GetZOffset();
            var xOffset = currentShip.GetComponent<EnemyShip>().GetXOffset();
            var position = tile.transform.position;
            var dropPosition = new Vector3(position.x - xOffset, position.y + 1f, position.z - zOffset);
            currentShip.transform.position = dropPosition;

            currentShip.GetComponent<EnemyShip>().SetupComplete();
            return true;
        }

        private bool IsOffGrid(int tileNumber, int tileIndex) {
            return tileNumber / 10 % 10 != tileIndex / 10 % 10 || tileIndex % 10 == 0;
        }

        private void SetupCompleted() {
            foreach (var tile in tiles) {
                tile.GetComponent<EnemyTile>().CompleteSetup();
            }
        
            currentShip = null;
            _setupComplete = true;
        }
    }
}
