using Unity.Netcode;
using UnityEngine;

public class CubeScript : NetworkBehaviour {
    
    private readonly int _spinSpeed = 50;
    private readonly Vector3 _clockwiseSpin = new Vector3(0, 1, 0);
    private readonly Vector3 _antiClockwiseSpin = new Vector3(0, -1, 0);
    public NetworkVariable<bool> clockwise = new(true);
    
    private void OnMouseDown() {
        if (NetworkManager.Singleton.IsHost) {
            clockwise.Value = !clockwise.Value;
            return;
        }

        ChangeDirectionServerRpc(!clockwise.Value);
    }

    void Update() {
        if (clockwise.Value) {
            gameObject.transform.Rotate(_clockwiseSpin * (_spinSpeed * Time.deltaTime));
        }
        else {
            gameObject.transform.Rotate(_antiClockwiseSpin * (_spinSpeed * Time.deltaTime));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeDirectionServerRpc(bool clockwiseValue) {
        clockwise.Value = clockwiseValue;
    }
}
