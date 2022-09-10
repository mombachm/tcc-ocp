using UnityEngine;

public class CoverageService
{
    public CoverageService() {
    }

    public CoverageData getTotalCoverageData()
    {
      float areaCov = 0.0f;
      float totalArea = 0.0f;
      CoverageBox[] covBoxes = GameObject.FindObjectsOfType<CoverageBox>();
      foreach (CoverageBox box in covBoxes) {
        CoverageData boxCovData = box.GetComponent<CoverageBox>().getCoverageData();
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