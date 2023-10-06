using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy {
    
    public class EnemyPlayer : MonoBehaviour {

        public static EnemyPlayer Instance;
     
        private bool _turnTaken;
        private bool _lastShotHit;
        private bool _secondLastShotHit;
        private bool _playerShipSunk;

        private const int MinTile = 1;
        private const int MaxTile = 100;
        
        private readonly List<GameObject> _tilesTargeted = new();
        private Stack<GameObject> _nextTargets = new();

        void Update() {
            if (GameManager.Instance.IsPlayerTurn()) return;
            if (_turnTaken) return;

            StartCoroutine(FireShot());
            _turnTaken = true;
        }

        IEnumerator FireShot() {
            yield return new WaitForSeconds(3.5f);

            GameObject tile = GetNextTarget();
            _tilesTargeted.Add(tile);

            if (tile.GetComponent<PlayerTile>().HasShip()) {
                PrepareNextShots(tile);
                _lastShotHit = true;
            }
            else if (_nextTargets.Count == 0) {
                _lastShotHit = false;
            }
            
            MissileScript.Instance.DropMissileOnTile(tile);
        }

        private GameObject GetNextTarget() {
            GameObject tile;

            if (!_lastShotHit || _nextTargets.Count == 0) {
                do {
                    var tileNumber = Random.Range(MinTile, MaxTile);
                    tile = GameObject.Find($"Tile ({tileNumber})");
                } while (_tilesTargeted.Contains(tile));
            }
            else {
                tile = _nextTargets.Pop();
            }

            return tile;
        }

        private void PrepareNextShots(GameObject tile) {
            var tileIndex = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
            var rightTileIndex = tileIndex + 1;
            var topTileIndex = tileIndex + 10;
            var leftTileIndex = tileIndex - 1;
            var bottomTileIndex = tileIndex - 10;
            
            // Get Right
            if (leftTileIndex / 10 % 10 == tileIndex / 10 % 10) {
                var targetTile = GameObject.Find($"Tile ({rightTileIndex})");
                
                if (!targetTile.GetComponent<PlayerTile>().HasMissile()) {
                    _nextTargets.Push(targetTile);                    
                }
            }
            
            // Get Top
            if (topTileIndex <= MaxTile) {
                var targetTile = GameObject.Find($"Tile ({topTileIndex})");
                
                if (!targetTile.GetComponent<PlayerTile>().HasMissile()) {
                    _nextTargets.Push(targetTile);                    
                }
            }
            
            // Get Left 
            if (leftTileIndex % 10 != 0) {
                var targetTile = GameObject.Find($"Tile ({leftTileIndex})");
                
                if (!targetTile.GetComponent<PlayerTile>().HasMissile()) {
                    _nextTargets.Push(targetTile);                    
                }
            }
            
            // Get Bottom
            if (bottomTileIndex >= MinTile) {
                var targetTile = GameObject.Find($"Tile ({bottomTileIndex})");
                
                if (!targetTile.GetComponent<PlayerTile>().HasMissile()) {
                    _nextTargets.Push(targetTile);                    
                }
            }

            Debug.Log($"Hit {tile.name} Next targets - {string.Join(", ", _nextTargets.Select(nextTarget => nextTarget.name).ToArray())}");
        }

        public void TakeTurn() {
            _turnTaken = false;
        }

        public void PlayerShipSunk() {
            _nextTargets = new Stack<GameObject>();
        }
    }
}
