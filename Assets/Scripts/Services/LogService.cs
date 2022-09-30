using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public struct GenerationData
{
    public int generationNumber;
    public double bestPriorityCoverage;
    public double bestPrivacyCoverage;
    public double bestScore;
}

public static class LogService
{
    private static StringBuilder csv = new StringBuilder();

    public static double bestScore;
    public static double bestPriorityCoverage;
    public static double bestPrivacyCoverage;
    public static List<GenerationData> generationData = new List<GenerationData>();

    public static void logToCSV()
    {
      logBestCoverages();
      logGenerationsData();
      File.WriteAllText("./Logs/logGA.csv", csv.ToString());
    }

    private static void logBestCoverages() {
      string newLine = string.Format("MELHOR SCORE (FITNESS);{0}", bestScore);
      var newLine2 = string.Format("MELHOR COBERTURA (PRIORIDADE);{0}", bestPriorityCoverage);
      var newLine3 = string.Format("MELHOR COBERTURA (PRIVACIDADE);{0}", bestPrivacyCoverage);
      csv.AppendLine(value: newLine);  
      csv.AppendLine(newLine2);  
      csv.AppendLine(newLine3);  
    }

    private static void logGenerationsData() {
      csv.AppendLine(string.Format("\n\nGERAÇÃO;W PRIO;W PRIV;SCORE;MELHOR COBERTURA (PRIORIDADE);MELHOR COBERTURA (PRIVACIDADE)")); 
      foreach (var genData in generationData) {
        string newLine = string.Format("{0};{1};{2};{3};{4};{5}", genData.generationNumber, Constants.WEIGHT_PRIV, Constants.WEIGHT_PRI0, genData.bestScore, genData.bestPriorityCoverage, genData.bestPrivacyCoverage);
        csv.AppendLine(newLine);  
      }
    }
}