using UnityEngine;

public class MainController
{
    static CoverageService coverageService = new CoverageService();

    public static TotalCoverageData getTotalCoverageData() {
      return coverageService.getTotalCoverageData();
    }
}