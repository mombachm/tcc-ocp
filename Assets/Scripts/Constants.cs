using Unity.VisualScripting;
using UnityEngine;

public class Constants
{
  public static string TAG_CAMERA_BOX = "CameraBox";
  public static string TAG_COVERAGE_BOX = "CoverageBox";
  public static string TAG_AVOID_BOX = "AvoidBox";
  public static bool DRAW_GISMOS = false;
  public static bool SHOW_COV_AREAS = true;
  public static bool SHOW_PRIV_AREAS = true;
  public static bool SHOW_CAM_AREAS = true;
  public static float WEIGHT_PRIV = 1.0f;
  public static float WEIGHT_PRI0 = 1.0f;
  public static int CELLS_DENSITY = 2; // 1 = 1 / meter
  public static int CAM_CELLS_DENSITY = 4; // 1 = 1 / meter
}