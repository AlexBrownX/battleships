using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public Button rightBtn;
    public Button leftBtn;
    
    private bool _isPlayerView = true;
    private bool _moveCameraRight;
    private bool _moveCameraLeft;
    private float _cameraMoveSpeed = 13f;

    void Start()
    {
        rightBtn.onClick.AddListener(() => RightBtnClicked());
        leftBtn.onClick.AddListener(() => LeftBtnClicked());
    }

    void Update() {
        PlayerView();
        EnemyView();
    }

    public void RightBtnClicked() {
        if (!_isPlayerView) {
            return;
        }

        _isPlayerView = false;
        _moveCameraLeft = false;
        _moveCameraRight = true;
    }

    public void LeftBtnClicked() {
        if (_isPlayerView) {
            return;
        }

        _isPlayerView = true;
        _moveCameraLeft = true;
        _moveCameraRight = false;
    }

    private void PlayerView() {
        if (!_moveCameraRight) return;
        
        GetComponent<StatusTextScript>().ClearText();
        var mainCameraPosition = Camera.main.transform.position;
        var enemyCameraPosition = new Vector3(-13.0f, mainCameraPosition.y,  mainCameraPosition.z);
            
        Camera.main.transform.position = 
            Vector3.MoveTowards(mainCameraPosition, enemyCameraPosition, _cameraMoveSpeed * Time.deltaTime);

        if (Camera.main.transform.position.Equals(enemyCameraPosition)) {
            GetComponent<StatusTextScript>().SetSelectTargetText();
            _moveCameraRight = false;
        }
    }


    private void EnemyView() {
        if (!_moveCameraLeft) return;
        
        GetComponent<StatusTextScript>().ClearText();
        var mainCameraPosition = Camera.main.transform.position;
        var playerCameraPosition = new Vector3(0f, mainCameraPosition.y,  mainCameraPosition.z);
            
        Camera.main.transform.position =
            Vector3.MoveTowards(mainCameraPosition, playerCameraPosition, _cameraMoveSpeed * Time.deltaTime);;

        if (Camera.main.transform.position.Equals(playerCameraPosition)) {
            GetComponent<StatusTextScript>().SetEnemyTargetingText();
            _moveCameraLeft = false;
        }
    }
}