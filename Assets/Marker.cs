using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Marker : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;
    public Camera currentCamera = null;
    public GameObject placementIndicator;
    public GameObject objectToPlace;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    public List<GameObject> points = null;
    public LineRenderer lineRenderer = null;

    void Start()
    {
        points = new List<GameObject>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var go = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
            points.Add(go);
        }

        if (points.Count > 1)
        {
            var positions = new Vector3[points.Count];
            for (var i = 0; i < points.Count; i++)
            {
                positions[i] = points[i].transform.position;
            }
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }
        else
        {
            lineRenderer.SetPositions(new Vector3[0]);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = currentCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = currentCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

}
