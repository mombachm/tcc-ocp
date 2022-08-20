using UnityEngine;

public class MainController
{
    static CoverageService coverageService = new CoverageService();

    [RuntimeInitializeOnLoadMethod]
    static void init()
    {
        Debug.Log("Starting OCP...");
        CameraBox[] objs = (CameraBox[])GameObject.FindObjectsOfType(typeof(CameraBox));
        Debug.Log("OBJECTS:");
        Debug.Log(objs.Length);
        Debug.Log(objs[0].cameraPositions.Length);
        GeneticAlgorithm ga = new GeneticAlgorithm();
    }

    public static float showTotalCoverageScore() {
      return coverageService.getTotalCoverageScore();
    }
}