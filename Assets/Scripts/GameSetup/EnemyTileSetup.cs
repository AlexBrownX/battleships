using UnityEngine;

namespace GameSetup {
    public class EnemyTileSetup : MonoBehaviour
    {
        public bool shipHovering = false;
        public bool setupComplete = false;
    
        void Start()
        {
        
        }

        void Update()
        {
        
        }
    
        public void CompleteSetup() {
            shipHovering = false;
            setupComplete = true;
            // GetComponent<Renderer>().material = tile;
            // Debug.Log($"{name} setup complete");
        }
    }
}
