using UnityEngine;

public class RecenterManager : MonoBehaviour
{
    void Start()
    {
        GameVRSettings.GetVRDevice();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire3") || Input.GetMouseButtonDown(2))
            GameVRSettings.Recenter();
    }
}
