using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public struct GenerationData
{
    public int generationNumber;
    public double bestPriorityCoverage;
    public double bestMultiPrioCoverage;
    public double bestPrivacyCoverage;
    public double bestScore;
    public double avgTimeCalcCoverage;
}

public static class LogService
{
    private static StringBuilder csv = new StringBuilder();

    public static double bestScore;
    public static double bestPriorityCoverage;
    public static double bestMultiPrioCoverage;
    public static double bestPrivacyCoverage;
    public static List<GenerationData> generationData = new List<GenerationData>();
    public static TotalCoverageData randomCovData;

    public static int contCalcCovCalls = 0;
    public static double sumGenCalcCovTime = 0;

    public static void logToCSV(CoverageGA ga, string logName)
    {
      logBestCoverages(ga);
      logRandomCoverageData();
      logCameraPositions();
      logGenerationsData();
      File.WriteAllText($"./Logs/{logName}.csv", csv.ToString());
    }

    private static void logBestCoverages(CoverageGA ga) {
      csv.AppendLine(value: string.Format("\nCAM COUNT;{0}", Constants.CAM_COUNT));  
      csv.AppendLine(value: string.Format(format: "CELL DENSITY;{0}", Constants.CELLS_DENSITY));
      csv.AppendLine(value: string.Format(format: "W PRIO;{0}", Constants.WEIGHT_PRI0));   
      csv.AppendLine(value: string.Format(format: "W PRIV;{0}", Constants.WEIGHT_PRIV));   
      csv.AppendLine(value: string.Format(format: "W MULTI PRIO;{0}", Constants.WEIGHT_MULTI_PRIO));   
      csv.AppendLine(string.Format("MELHOR SCORE (FITNESS);{0}", bestScore));  
      csv.AppendLine(string.Format("MELHOR COBERTURA (PRIORIDADE);{0}", bestPriorityCoverage));  
      csv.AppendLine(string.Format("MELHOR COBERTURA (PRIVACIDADE);{0}", bestPrivacyCoverage));
      csv.AppendLine(string.Format("MELHOR COBERTURA MULTICAM (PRIORIDADE);{0}", bestMultiPrioCoverage));
    }

    private static void logRandomCoverageData() {
      csv.AppendLine(string.Format("\nRANDOM SCORE (FITNESS);{0}", randomCovData.Score));  
      csv.AppendLine(string.Format("RANDOM COBERTURA (PRIORIDADE);{0}", randomCovData.PriorityCoverage));  
      csv.AppendLine(string.Format("RANDOM COBERTURA (PRIVACIDADE);{0}", randomCovData.PrivacyCoverage));
      csv.AppendLine(string.Format("RANDOM COBERTURA MULTICAM (PRIORIDADE);{0}", randomCovData.MultiPriorityCoverage));    
    }

    private static void logGenerationsData() {
      csv.AppendLine(string.Format("\nGERAÇÃO;W PRIO;W PRIV;W MULTI PRIO;SCORE;MELHOR COBERTURA (PRIORIDADE);MELHOR COBERTURA (PRIVACIDADE);MELHOR COBERTURA MULTICAM (PRIORIDADE);AVG TIME CALC COVERAGE")); 
      foreach (var genData in generationData) {
        string newLine = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", genData.generationNumber, Constants.WEIGHT_PRIV, Constants.WEIGHT_PRI0, Constants.WEIGHT_MULTI_PRIO, genData.bestScore, genData.bestPriorityCoverage, genData.bestPrivacyCoverage, genData.bestMultiPrioCoverage, genData.avgTimeCalcCoverage);
        csv.AppendLine(newLine);  
      }
      generationData.Clear();
    }
    private static void logCameraPositions() {
      csv.AppendLine(string.Format("\nCAM NAME;CAM POS")); 
      foreach (var cam in Camera.allCameras) {
        string newLine = string.Format("{0};{1}", cam.name, cam.transform.position.ToString());
        csv.AppendLine(newLine);  
      }
    }

    public static void resetTimeCalcCov() {
      contCalcCovCalls = 0;
      sumGenCalcCovTime = 0;
    }
}