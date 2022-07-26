using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class TestCameraPermission : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Permission.RequestUserPermission(Permission.Camera);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
