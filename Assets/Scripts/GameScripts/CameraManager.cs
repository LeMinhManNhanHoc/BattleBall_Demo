using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public RenderTexture output;

    public void UpdateCamera()
    {
        if (GameController.Instance.isMatcheOver || GameController.Instance.isGameOver || GameController.Instance.enterPenalty)
            return;

        RaycastTarget();
        ZoomCamera();
    }

    public void RaycastTarget()
    {
        if (Input.touchCount <= 1)
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
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
                else if (objectHit.tag == "EnemyField" && GameManager.Instance.isLocalPVP)
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

    public void ZoomCamera()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
            {
                Vector2 curDist = touch0.position - touch1.position;
                Vector2 prevDist = ((touch0.position - touch0.deltaPosition) - (touch1.position - touch1.deltaPosition));
                float touchDelta = curDist.magnitude - prevDist.magnitude;

                if (!GameManager.Instance.enableARMode)
                {
                    float fov = mainCamera.fieldOfView + touchDelta;
                    mainCamera.fieldOfView = Mathf.Clamp(fov, GameManager.Instance.minFov, GameManager.Instance.maxFov);
                }
                else
                {
                    mainCamera.transform.position += Vector3.up * touchDelta;
                }
            }
        }
        else
        {
            if (!GameManager.Instance.enableARMode)
            {
                mainCamera.fieldOfView -= Input.mouseScrollDelta.y;
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, GameManager.Instance.minFov, GameManager.Instance.maxFov);
            }
            else
            {
                mainCamera.transform.position -= Vector3.up * Input.mouseScrollDelta.y;
            }
        }
    }
}
