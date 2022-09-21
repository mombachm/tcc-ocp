using UnityEngine;

public class CoverageService
{
    public CoverageService() {}

    public CoverageData getTotalCoverageData()
    {
      float areaCov = 0.0f;
      float totalArea = 0.0f;
      float totalCamDistanceAvg = 0.0f;
      int countPriorityAreas = 0;
      CoverageBox2[] covBoxes = GameObject.FindObjectsOfType<CoverageBox2>();
      foreach (CoverageBox2 box in covBoxes) {
        CoverageData boxCovData = box.GetComponent<CoverageBox2>().getCoverageData();
        areaCov += boxCovData.AreaCovered;
        totalArea += boxCovData.TotalArea;
        if (box.type == CoverageBox2.CoverageType.Cover && boxCovData.avgCamDistance > 0) {
          countPriorityAreas++;
          totalCamDistanceAvg += boxCovData.avgCamDistance;
        }
      }
      float totalScore = (areaCov / totalArea) * 100;
      CoverageData covData = new CoverageData();
      covData.TotalArea = totalArea;
      covData.AreaCovered = areaCov;
      covData.Score = totalScore;
      covData.avgCamDistance = totalCamDistanceAvg / countPriorityAreas;
      return covData;
    }
}