using System.Collections.Generic;
using UnityEngine;

public class CameraAreaService
{
    private Vector3[] possibleCamPositions;
    public CameraAreaService() {
    }

    public Vector3[] getAllPossiblePositions()
    {
      if (possibleCamPositions != null) {
        return possibleCamPositions;
      }

      CameraBox[] camBoxes = GameObject.FindObjectsOfType<CameraBox>();
      List<Vector3> list = new List<Vector3>();
      foreach (CameraBox box in camBoxes) {
        Vector3[] camPositions = box.GetComponent<CameraBox>().cameraPositions;
        list.AddRange(camPositions);
      }
      possibleCamPositions = list.ToArray();
      return possibleCamPositions;
    }
}