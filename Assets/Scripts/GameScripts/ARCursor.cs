using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCursor : MonoBehaviour
{
    public ARSessionOrigin origin;
    public GameObject cursorChildObject;
    public GameObject objectToPlace;
    public ARRaycastManager raycastManager;
    public Camera arCamera;

    public bool stopUdate = false;

    private bool stopUpdate;
    // Start is called before the first frame update
    void Start()
    {
        //cursorChildObject.SetActive(useCursor);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopUpdate)
        {
            UpdateCursor();
        }

        RaycastTarget();
    }

    private void UpdateCursor()
    {
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon);

        if (hits.Count > 0)
        {
            cursorChildObject.transform.position = hits[0].pose.position;
            cursorChildObject.transform.rotation = hits[0].pose.rotation;
            stopUpdate = true;
        }
    }

    public void RaycastTarget()
    {
        //if (Input.touchCount >0)
        //{
        //    Touch touch = Input.GetTouch(0);

        //    Ray ray = arCamera.ScreenPointToRay(touch.position);
        //    RaycastHit hitObject;
        //    if (Physics.Raycast(ray, out hitObject))
        //    {
        //        Transform objectHit = hitObject.transform;
        //        Debug.LogError(objectHit.name);
        //        if (objectHit.tag == "PlayerField")
        //        {
        //            Debug.LogError("Raycast enemy");
        //        }
        //        else if (objectHit.tag == "EnemyField")
        //        {
        //            if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isPlayerAttack)
        //            {
        //                GameManager.Instance.SpawnAttacker(hitObject.point + Vector3.up);
        //            }
        //            else if (Input.GetMouseButtonDown(0) && GameManager.Instance.isPlayerAttack)
        //            {
        //                GameManager.Instance.SpawnSpawnDefender(hitObject.point + Vector3.up);
        //            }
        //        }
        //    }
        //}

        if (Input.touchCount <= 1)
        {
            RaycastHit hit;
            Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                //Debug.LogError(objectHit.name);
                if (objectHit.tag == "PlayerField")
                {
                    if (Input.GetMouseButtonDown(0) && GameController.Instance.isPlayerAttack)
                    {
                        GameController.Instance.SpawnAttacker(hit.point + Vector3.up);
                    }
                    else if (Input.GetMouseButtonDown(0) && !GameController.Instance.isPlayerAttack)
                    {
                        GameController.Instance.SpawnDefender(hit.point + Vector3.up);
                    }
                }
                else if (objectHit.tag == "EnemyField")
                {
                    if (Input.GetMouseButtonDown(0) && !GameController.Instance.isPlayerAttack)
                    {
                        GameController.Instance.SpawnAttacker(hit.point + Vector3.up);
                    }
                    else if (Input.GetMouseButtonDown(0) && GameController.Instance.isPlayerAttack)
                    {
                        GameController.Instance.SpawnDefender(hit.point + Vector3.up);
                    }
                }
            }
        }
    }
}
