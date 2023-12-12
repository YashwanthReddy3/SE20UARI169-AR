using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARSubsystems;

public class FurniturePlacementManager : MonoBehaviour
{
    public GameObject SpawnableFurniture;
    public XROrigin sessionOrigin;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private bool isPlacing = false; 

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartPlacement(touch.position);
                    break;
                case TouchPhase.Moved:
                    if (isPlacing)
                    {
                        UpdatePlacement(touch.position);
                    }
                    break;
                case TouchPhase.Ended:
                    if (isPlacing)
                    {
                        EndPlacement();
                    }
                    break;
            }
        }
    }

    private void StartPlacement(Vector2 touchPosition)
    {
        bool collision = raycastManager.Raycast(touchPosition, raycastHits, TrackableType.PlaneWithinPolygon);

        if (collision)
        {
            GameObject _object = Instantiate(SpawnableFurniture);
            _object.transform.position = raycastHits[0].pose.position;

        
            isPlacing = true;
        }
    }

    private void UpdatePlacement(Vector2 touchPosition)
    {
    
        float rotationSpeed = 2.0f;
        float rotationAmount = touchPosition.x * rotationSpeed * Time.deltaTime;
        SpawnableFurniture.transform.Rotate(Vector3.up, rotationAmount);

    
        float pinchAmount = 0.0f;
        if (Input.touchCount >= 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 prevTouchDeltaPosition1 = touch1.position - touch1.deltaPosition;
            Vector2 prevTouchDeltaPosition2 = touch2.position - touch2.deltaPosition;

            float prevTouchDeltaMagnitude = (prevTouchDeltaPosition1 - prevTouchDeltaPosition2).magnitude;
            float touchDeltaMagnitude = (touch1.position - touch2.position).magnitude;

            pinchAmount = (touchDeltaMagnitude - prevTouchDeltaMagnitude) * 0.01f;
        }

        SpawnableFurniture.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);
    }

    private void EndPlacement()
    {
    
        isPlacing = false;

        foreach (var planes in planeManager.trackables)
        {
            planes.gameObject.SetActive(false);
        }
        planeManager.enabled = false;
    }

    public void SwitchFurniture(GameObject furniture)
    {
        SpawnableFurniture = furniture;
    }
}

