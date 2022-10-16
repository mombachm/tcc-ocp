using System.Collections.Generic;
using UnityEngine;

public class CameraConfigService
{
    const int MIN_PAN = 0;
    const int MAX_PAN = 359;
    const int MIN_TILT = 30;
    const int MAX_TILT = 70;
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
      foreach (var cam in Camera.allCameras) {
        CameraController camController = cam.GetComponent<CameraController>();
        camController.destroy();
      }
      for (int i = 0; i < cameraCount; i++) {        
        var cameraGameObject = new GameObject( $"GA cam {i}" );
        cameraGameObject.gameObject.SetActive(true);
        var newCam = cameraGameObject.AddComponent<Camera>();
        newCam.gameObject.AddComponent<CameraController>();
        newCam.nearClipPlane = 0.01f;
        newCam.fieldOfView = Camera.HorizontalToVerticalFieldOfView(90, 16/9);
        newCam.gateFit = Camera.GateFitMode.None;
        newCam.useOcclusionCulling = true;
        newCam.targetDisplay = i;
      }
      CoverageBox2[] covBoxes = GameObject.FindObjectsOfType<CoverageBox2>();
      foreach (CoverageBox2 box in covBoxes) {
        box.initCamData();
      }
    }

    public void randomPositionCameras() {
      foreach (var cam in Camera.allCameras) {
        CameraController camController = cam.GetComponent<CameraController>();
        var positions = new CameraAreaService().getAllPossiblePositions();
        var tiltAngles = this.getTiltAngles();
        var panAngles = this.getPanAngles();
        var randPosIndex = Random.Range(0, positions.Length - 1);
        var randTiltIndex = Random.Range(0, tiltAngles.Length - 1);
        var randPanIndex = Random.Range(0, panAngles.Length - 1);
        Debug.Log($"RANDOM INDEXES: {randPosIndex} {randTiltIndex} {randPanIndex}");
        Debug.Log($"RANDOM VALUES: {positions[randPosIndex].ToString()} {tiltAngles[randTiltIndex].ToString()} {panAngles[randPanIndex].ToString()}");
        camController.transform.position = positions[randPosIndex];
        camController.transform.rotation = Quaternion.Euler((float)tiltAngles[randTiltIndex], (float)panAngles[randPanIndex], 0);
      }
    }

    public bool checkCamerasState() {
      foreach (var cam in Camera.allCameras) {
        CameraController camController = cam.GetComponent<CameraController>();
        if(!camController.hasChanged) return false;
      }
      return true;
    }
}