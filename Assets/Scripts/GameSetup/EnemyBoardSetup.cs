using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameSetup {
    public class EnemyBoardSetup : MonoBehaviour
    {
        public static EnemyBoardSetup Instance;
        
        public GameObject[] tiles;
        public GameObject[] ships;
        public GameObject currentShip;
        
        private int _shipIndex = -1;
        private int _minTile = 101;
        private int _maxTile = 200;
        private bool _setupComplete;

        void Start()
        {
            Instance = this;
            PopulateTiles();
            SetNextShip();
        }

        void Update() {
            if (!_setupComplete) return;
            GameManager.Instance.EnemyCompleteSetup();
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
            currentShip.GetComponent<Renderer>().enabled = true;
            currentShip.GetComponent<EnemyShipScript>().gameObject.SetActive(true);
            
            do { } while (!PlaceShip());
            SetNextShip();
        }

        private bool PlaceShip() {
            var tileNumber = Random.Range(_minTile, _maxTile);
            var isRotated = Random.value > 0.5f;
            var shipSize = currentShip.GetComponent<EnemyShipScript>().shipSize;
            var tile = GameObject.Find($"Tile ({tileNumber})");
            var shipTiles = new GameObject[shipSize];
            
            for (var i = 0; i < shipSize; i++) {
                var tileIndex = isRotated ? tileNumber + i : tileNumber + (i * 10);

                if (tileIndex > _maxTile) {
                    // Debug.Log($"Start tile - {tileNumber} Can't place ship here, off the board - {tileIndex}");
                    return false;
                }
                
                if (isRotated && IsOffGrid(tileNumber, tileIndex)) {
                    // Debug.Log($"Start tile - {tileNumber} Can't place ship here, off the board - {tileIndex}");
                    return false;
                }

                var shipTile = GameObject.Find($"Tile ({tileIndex})");

                if (shipTile.GetComponent<EnemyTileSetup>().hasShip) {
                    // Debug.Log($"Start tile - {tileNumber} Can't place ship here, already taken - {tileIndex}");
                    return false;
                }
                
                shipTiles[i] = shipTile;
            }

            foreach (var shipTile in shipTiles) {
                shipTile.GetComponent<EnemyTileSetup>().PlaceShipOnTile();
            }

            var shipTilesNames = string.Join(",", shipTiles.Select(shipTile => shipTile.name.ToString()).ToArray());
            Debug.Log($"Start tile - {tileNumber} Placing {currentShip.name} - [{shipTilesNames}]");
            
            if (isRotated) {
                currentShip.GetComponent<EnemyShipScript>().SetRotated(true);
                currentShip.transform.Rotate(0, 90, 0);
            }
            
            var zOffset = currentShip.GetComponent<EnemyShipScript>().GetZOffset();
            var xOffset = currentShip.GetComponent<EnemyShipScript>().GetXOffset();
            var dropPosition = new Vector3(tile.transform.position.x - xOffset, tile.transform.position.y + 1f, tile.transform.position.z - zOffset);
            currentShip.transform.position = dropPosition;

            return true;
        }

        private bool IsOffGrid(int tileNumber, int tileIndex) {
            return tileNumber / 10 % 10 != tileIndex / 10 % 10 || tileIndex % 10 == 0;
        }

        private void SetupCompleted() {
            foreach (var tile in tiles) {
                tile.GetComponent<EnemyTileSetup>().CompleteSetup();
            }
        
            currentShip = null;
            _setupComplete = true;
        }
    }
}