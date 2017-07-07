using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public class Test : MonoBehaviour, IInputClickHandler
{
    public bool IsBeingPlaced;
    protected WorldAnchorManager anchorManager;
    protected SpatialMappingManager spatialMappingManager;
    GameObject clonedObject;
    public GameObject cloneObject;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        IsBeingPlaced = !IsBeingPlaced;

        if (IsBeingPlaced)
        {
            spatialMappingManager.DrawVisualMeshes = true;
            clonedObject = Instantiate(cloneObject);
        }
        else
        {
            spatialMappingManager.DrawVisualMeshes = false;
            anchorManager.AttachAnchor(clonedObject, "test");
        }
    }

    // Use this for initialization
    void Start ()
    {
        anchorManager = WorldAnchorManager.Instance;
        if (anchorManager == null)
        {
            Debug.LogError("This script expects that you have a WorldAnchorManager component in your scene.");
        }

        spatialMappingManager = SpatialMappingManager.Instance;
        if (spatialMappingManager == null)
        {
            Debug.LogError("This script expects that you have a SpatialMappingManager component in your scene.");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (IsBeingPlaced)
        {
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, spatialMappingManager.LayerMask))
            {
                // Rotate this object to face the user.
                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;

                // Move this object to where the raycast
                // hit the Spatial Mapping mesh.
                // Here is where you might consider adding intelligence
                // to how the object is placed.  For example, consider
                // placing based on the bottom of the object's
                // collider so it sits properly on surfaces.
                if(clonedObject != null)
                {
                    clonedObject.transform.position = hitInfo.point;
                    clonedObject.transform.rotation = toQuat;
                }
                
            }
        }
    }
}
