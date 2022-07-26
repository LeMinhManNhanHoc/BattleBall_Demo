using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using TMPro;

public class ARCheckBox : MonoBehaviour
{
    public TextMeshProUGUI arText;
    
    // Update is called once per frame
    void Update()
    {
        arText.text = GameManager.Instance.enableARMode ? "AR ON" : "AR OFF";
    }

    public void OnARButtonClicked()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        else
        {
            GameManager.Instance.enableARMode = !GameManager.Instance.enableARMode;
        }
    }
}
