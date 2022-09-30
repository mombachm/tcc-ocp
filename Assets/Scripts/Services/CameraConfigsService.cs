using System.Collections.Generic;
using UnityEngine;

public class CameraConfigService
{
    const int MIN_PAN = 0;
    const int MAX_PAN = 359;
    const int MIN_TILT = 0;
    const int MAX_TILT = 90;
    const int ANGLE_STEP = 1;

    public CameraConfigService() {}

    public int[] getTiltAngles() {
      return getAnglesByStep(MIN_TILT, MAX_TILT);
    }

    public int[] getPanAngles() {
      return getAnglesByStep(MIN_PAN, MAX_PAN);
    }

    private int[] getAnglesByStep(int min, int max) {
      int length = (max-min)/ANGLE_STEP + 1;
      int[] angles = new int[length];
      for (int i = 0; i < length; i++) {
        angles[i] = i * ANGLE_STEP + min;
      }
      return angles;
    }

    public void instantiateCameras(int cameraCount) {
      for (int i = 0; i < cameraCount; i++) {        
        // CameraController newCam = Object.Instantiate(Camera.main.GetComponent<CameraController>(),  new Vector3(0, 0, 0), Quaternion.identity);
        var cameraGameObject = new GameObject( $"GA cam {i}" );
        cameraGameObject.gameObject.SetActive(true);
        var newCam = cameraGameObject.AddComponent<Camera>();
        newCam.gameObject.AddComponent<CameraController>();
        newCam.nearClipPlane = 0.01f;
        //newCam.usePhysicalProperties = true;
        newCam.fieldOfView = Camera.HorizontalToVerticalFieldOfView(90, 16/9);
        newCam.gateFit = Camera.GateFitMode.None;
        newCam.useOcclusionCulling = true;
        newCam.targetDisplay = i;
      }
      // Camera.main.enabled = false;
    }
}