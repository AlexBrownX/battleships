using UnityEngine;

namespace Multiplayer {
    
    public class ClientShip : MonoBehaviour {
        
        [SerializeField] public float offset;
        [SerializeField] public bool isRotated;
        [SerializeField] public int shipSize;
        [SerializeField] public AudioClip splash;

        private GameObject[] _tiles;

        public Vector3 GetHoverPosition(Vector3 tilePosition) {
            return isRotated
                ? new Vector3(tilePosition.x + offset, 1.1f, tilePosition.z)
                : new Vector3(tilePosition.x, 1.1f, tilePosition.z + offset);
        }

        public void RotateShip() {
            if (isRotated) {
                gameObject.transform.Rotate(0, 90, 0);
                isRotated = false;
            }
            else {
                gameObject.transform.Rotate(0, -90, 0);
                isRotated = true;
            }
        }

        public void PlaceShip(GameObject[] tiles) {
            _tiles = tiles;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            gameObject.GetComponent<BoxCollider>().isTrigger = true;

            // TODO - TEMP
            // Debug.Log($"{string.Join(", ", _tiles.Select(tile => tile.name).ToArray())}");
        }
        
        void OnCollisionEnter() {
            AudioSource.PlayClipAtPoint(splash, transform.position, 0.1f);
        }
    }
}
