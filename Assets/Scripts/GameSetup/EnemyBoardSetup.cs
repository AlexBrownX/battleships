using System.Linq;
using UnityEngine;

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
            if (_setupComplete) return;
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
        }
    
        private void SetupCompleted() {
            foreach (var tile in tiles) {
                tile.GetComponent<EnemyTileSetup>().CompleteSetup();
            }
        
            currentShip = null;
            _setupComplete = true;
            GameManager.Instance.CompleteSetup();
        }
    }
}
