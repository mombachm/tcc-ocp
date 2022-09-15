using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Runner.UnityApp.Commons;
using System.Collections.Generic;
using GeneticSharp.Domain.Randomizations;

public interface ICameraSetup {
    Vector3 Position { get; }
    int PanAngle { get; }
    int TiltAngle { get; }
}

public class CameraSetup : ICameraSetup {
    public CameraSetup(Vector3 position, int panAngle, int tiltAngle) {
        Position = position;
        PanAngle = panAngle;
        TiltAngle = tiltAngle;
    }

    public Vector3 Position { get; }
    public int PanAngle { get; }
    public int TiltAngle { get; }
}

public class CameraChromosome : BitStringChromosome<CameraPhenotype>
{
  private Vector3[] possiblePositions;
  int[] panAngles;
  int[] tiltAngles;
  int cameraCount;

  public CameraChromosome(Vector3[] possiblePositions, int[] panAngles, int[] tiltAngles, int cameraCount)
  {
      this.possiblePositions = possiblePositions;
      this.panAngles = panAngles;
      this.tiltAngles = tiltAngles;
      this.cameraCount = cameraCount;
      var phenotypes = new List<CameraPhenotype>(cameraCount);
      for (int i = 0; i < cameraCount; i++) {
        phenotypes.Add(new CameraPhenotype(
          this.possiblePositions.Length, 
          panAngles.Length,
          tiltAngles.Length
        ));
      }

      SetPhenotypes(phenotypes.ToArray());
      CreateGenes();
  }

  public string ID { get; } = System.Guid.NewGuid().ToString();
  public int CamerasCount { get; private set; }
  public bool Evaluated { get; set; }
  public double Score { get; set; }

  public List<CameraSetup> CamerasSetup {
    get {
      var camerasSetup = new List<CameraSetup>();
      foreach (var item in this.GetPhenotypes()) {
        camerasSetup.Add(new CameraSetup(
          this.possiblePositions[item.PositionIndex],
          this.panAngles[item.PanIndex],
          this.tiltAngles[item.TiltIndex]
        ));
      }
      return camerasSetup;
    }
  }

  public override IChromosome CreateNew()
  {
      return new CameraChromosome(
        this.possiblePositions,
        this.panAngles,
        this.tiltAngles,
        this.cameraCount
      );
  }
}