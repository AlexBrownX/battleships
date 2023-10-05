using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemy {
    
    public class EnemyPlayer : MonoBehaviour {
     
        private bool _turnTaken;
        private int _minTile = 1;
        private int _maxTile = 100;
        private List<GameObject> _tilesTargeted = new();

        void Update() {
            if (GameManager.Instance.IsPlayerTurn()) return;
            if (_turnTaken) return;

            StartCoroutine(FireShot());
            _turnTaken = true;
        }

        IEnumerator FireShot() {
            yield return new WaitForSeconds(3.5f);
            GameObject tile;

            do {
                var tileNumber = Random.Range(_minTile, _maxTile);
                tile = GameObject.Find($"Tile ({tileNumber})");
            } while (_tilesTargeted.Contains(tile));
        
            _tilesTargeted.Add(tile);
            MissileScript.Instance.DropMissileOnTile(tile);
        }
        
        public void TakeTurn() {
            _turnTaken = false;
        }
    }
}
