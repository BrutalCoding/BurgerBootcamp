using HoloToolkit.Unity.SpatialMapping;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSpatianalMeshes : MonoBehaviour
{
    public bool showVisualMeshes = true;
    // Use this for initialization
    void Start()
    {
        SpatialMappingManager.Instance.DrawVisualMeshes = showVisualMeshes;
    }

    void OnTriggerEnter(Collider hit)
    {
        showVisualMeshes = !showVisualMeshes;
        SpatialMappingManager.Instance.DrawVisualMeshes = showVisualMeshes;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
