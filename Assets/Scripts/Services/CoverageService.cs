using UnityEngine;

public class CoverageService
{
    public CoverageService() {}

    public CoverageData getTotalCoverageData()
    {
      float areaCov = 0.0f;
      float totalArea = 0.0f;
      CoverageBox2[] covBoxes = GameObject.FindObjectsOfType<CoverageBox2>();
      foreach (CoverageBox2 box in covBoxes) {
        CoverageData boxCovData = box.GetComponent<CoverageBox2>().getCoverageData();
        areaCov += boxCovData.AreaCovered;
        totalArea += boxCovData.TotalArea;
      }
      float totalScore = (areaCov / totalArea) * 100;
      CoverageData covData = new CoverageData();
      covData.TotalArea = totalArea;
      covData.AreaCovered = areaCov;
      covData.Score = totalScore;
      return covData;
    }
}