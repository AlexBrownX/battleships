using UnityEngine;
using UnityEngine.UI;

public class StatusTextScript : MonoBehaviour
{
    public Text statusText;
    private const string StartText = "Place your battleships";

    void Start()
    {
        statusText.text = StartText;
    }

    void Update()
    {
        
    }
}
