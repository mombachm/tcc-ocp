using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public struct TotalCoverageData
{
    // public float TotalArea;

    public float Score;
    public float PriorityCoverage;
    public float MultiPriorityCoverage;
    public float PrivacyCoverage;
    public float AvgCamDistance;
}

public class CoverageService
{
    private Stopwatch stopwatch = new Stopwatch();
    public CoverageService() {}

    public TotalCoverageData getTotalCoverageData() {
      stopwatch.Reset();
      stopwatch.Start();
      float prioAreaCov = 0.0f;
      float multiPrioAreaCov = 0.0f;
      float totalPrioArea = 0.0f;
      float totalCamDistanceAvg = 0.0f;
      int countPriorityAreas = 0;
      float privAreaCov = 0.0f;
      float totalPrivArea = 0.0f;
      CoverageBox2[] covBoxes = GameObject.FindObjectsOfType<CoverageBox2>();
      foreach (CoverageBox2 box in covBoxes) {
        CoverageData boxCovData = box.getCoverageData();
        if (box.type == CoverageBox2.CoverageType.Cover) {
          prioAreaCov += boxCovData.AreaCovered;
          multiPrioAreaCov += boxCovData.AreaMultiCovered;
          totalPrioArea += boxCovData.TotalArea;
          if (boxCovData.avgCamDistance > 0) {
            countPriorityAreas++;
            totalCamDistanceAvg += boxCovData.avgCamDistance;
          }
        } else if (box.type == CoverageBox2.CoverageType.Avoid) {
          privAreaCov += boxCovData.AreaCovered;
          totalPrivArea += boxCovData.TotalArea;
        }
      }
      float totalPrioCoverage = (prioAreaCov / totalPrioArea) * 100;
      float totalPrioMultiCoverage = (multiPrioAreaCov / totalPrioArea) * 100;
      float totalPrivCoverage = (privAreaCov / totalPrivArea) * 100;
      if (totalPrivArea == 0.0f) totalPrivCoverage = 0;
      if (totalPrioArea == 0.0f) {
        totalPrivCoverage = 0;
        totalPrioMultiCoverage = 0;
      }
      TotalCoverageData covData = new TotalCoverageData();
      // covData.TotalArea = totalPrioArea;
      // covData.AreaCovered = areaCov;
      covData.PriorityCoverage = totalPrioCoverage;
      covData.MultiPriorityCoverage = totalPrioMultiCoverage;
      covData.PrivacyCoverage = totalPrivCoverage;
      covData.AvgCamDistance = totalCamDistanceAvg / countPriorityAreas;
      covData.Score = this.calculateScore(covData);
      stopwatch.Stop();
      LogService.sumGenCalcCovTime += stopwatch.ElapsedMilliseconds;
      LogService.contCalcCovCalls++;
      return covData;
    }
    
    private float calculateScore(TotalCoverageData covData) {
      var score = (covData.MultiPriorityCoverage * Constants.WEIGHT_MULTI_PRIO + covData.PriorityCoverage * Constants.WEIGHT_PRI0) - (covData.PrivacyCoverage * Constants.WEIGHT_PRIV);
      if (covData.PrivacyCoverage < 0) return 0;
      return score;
    }

    public void resetCullingInfo() {
      CoverageBox2[] covBoxes = GameObject.FindObjectsOfType<CoverageBox2>();
      foreach (CoverageBox2 box in covBoxes) {
        box.resetCullingInfo();
      }
    }
}