using UnityEngine;
using Random = UnityEngine.Random;

namespace GameSetup {
    public class EnemyBoardSetup : MonoBehaviour
    {
        public static EnemyBoardSetup Instance;
        public GameObject[] tiles;
        public GameObject[] ships;
        public GameObject currentShip;
        // public Button rotateBtn;
    
        private int _shipIndex = -1;
        private bool _setupComplete;

        void Start()
        {
            Instance = this;
            PopulateTiles();
            SetNextShip();
        }

        void Update() {
            Setup();
        }

        private void Setup() {
            if (_setupComplete) {
                GameManager.Instance.EnemyCompleteSetup();
            }
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
            
            // TODO - Remove so ships are invisible 
            currentShip.GetComponent<Renderer>().enabled = true;
            currentShip.GetComponent<EnemyShipScript>().gameObject.SetActive(true);

            PlaceShip();
            SetNextShip();
        }

        private void PlaceShip() {
            var startPosition = Random.Range(101, 200);
            var shipSize = currentShip.GetComponent<EnemyShipScript>().shipSize;
            var rotated = Random.value > 0.5f;

            // Debug.Log($"Rotated: {rotated} Start Pos: {startPosition}");

            if (CanPlaceShip(startPosition, shipSize, rotated)) {
                
            }
        }

        private bool CanPlaceShip(double startPosition, int shipSize, bool rotated) {
            var startTile = GameObject.Find($"Tile ({startPosition})");
            var tilePosition = startTile.transform.position;

            if (rotated) {
                currentShip.GetComponent<EnemyShipScript>().SetRotated(true);
                currentShip.transform.Rotate(0, 90, 0);
            }

            var zOffset = currentShip.GetComponent<EnemyShipScript>().GetZOffset();
            var xOffset = currentShip.GetComponent<EnemyShipScript>().GetXOffset();
            var dropPosition = new Vector3(tilePosition.x - xOffset, tilePosition.y + 1f, tilePosition.z - zOffset);
            currentShip.transform.position = dropPosition;

            return true;
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
