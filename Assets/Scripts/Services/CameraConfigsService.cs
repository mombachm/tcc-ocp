using System.Collections.Generic;
using UnityEngine;

public class CameraConfigService
{
    const int MIN_PAN = 0;
    const int MAX_PAN = 359;
    const int MIN_TILT = -90;
    const int MAX_TILT = 90;
    const int ANGLE_STEP = 5;

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
}