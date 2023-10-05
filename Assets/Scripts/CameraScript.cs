using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour {
    
    public static CameraScript Instance;
    
    public Button rightBtn;
    public Button leftBtn;
    
    private bool _isPlayerView = true;
    private bool _moveCameraRight;
    private bool _moveCameraLeft;
    private float _cameraMoveSpeed = 13f;

    void Start() {
        rightBtn.onClick.AddListener(ViewEnemyBoard);
        leftBtn.onClick.AddListener(ViewPlayerBoard);
    }

    void Update() {
        PlayerView();
        EnemyView();
    }

    public void ViewEnemyBoard() {
        if (!_isPlayerView) {
            return;
        }

        // GetComponent<HUDScript>().SetSelectTargetText();

        _isPlayerView = false;
        _moveCameraLeft = false;
        _moveCameraRight = true;
    }

    public void ViewPlayerBoard() {
        if (_isPlayerView) {
            return;
        }

        // GetComponent<HUDScript>().SetEnemyTargetingText();

        _isPlayerView = true;
        _moveCameraLeft = true;
        _moveCameraRight = false;
    }

    private void PlayerView() {
        if (!_moveCameraRight) return;
        StartCoroutine(PlayerViewDelayed(1.5f));
    }


    private void EnemyView() {
        if (!_moveCameraLeft) return;
        StartCoroutine(EnemyViewDelayed(1.5f));
    }

    IEnumerator PlayerViewDelayed(float seconds) {
        yield return new WaitForSeconds(seconds);
        
        // GetComponent<HUDScript>().ClearText();
        var mainCameraPosition = Camera.main.transform.position;
        var enemyCameraPosition = new Vector3(-13.0f, mainCameraPosition.y,  mainCameraPosition.z);
            
        Camera.main.transform.position = 
            Vector3.MoveTowards(mainCameraPosition, enemyCameraPosition, _cameraMoveSpeed * Time.deltaTime);

        if (Camera.main.transform.position.Equals(enemyCameraPosition)) {
            // GetComponent<HUDScript>().SetSelectTargetText();
            _moveCameraRight = false;
        }
    }
    
    IEnumerator EnemyViewDelayed(float seconds) {
        yield return new WaitForSeconds(seconds);
        
        // GetComponent<HUDScript>().ClearText();
        var mainCameraPosition = Camera.main.transform.position;
        var playerCameraPosition = new Vector3(0f, mainCameraPosition.y,  mainCameraPosition.z);
            
        Camera.main.transform.position =
            Vector3.MoveTowards(mainCameraPosition, playerCameraPosition, _cameraMoveSpeed * Time.deltaTime);;

        if (Camera.main.transform.position.Equals(playerCameraPosition)) {
            // GetComponent<HUDScript>().SetEnemyTargetingText();
            _moveCameraLeft = false;
        }
    }
}
