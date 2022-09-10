using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Runner.UnityApp.Commons;
using System.Collections.Generic;
using GeneticSharp.Domain.Randomizations;

public class CameraChromosome : BitStringChromosome<CameraPhenotype>
{
  private Vector3[] possiblePositions;
  int[] panAngles;
  int[] tiltAngles;

  public CameraChromosome(Vector3[] possiblePositions, int[] panAngles, int[] tiltAngles)
  {
      this.possiblePositions = possiblePositions;
      this.panAngles = panAngles;
      this.tiltAngles = tiltAngles;
      var phenotypes = new CameraPhenotype(
        this.possiblePositions.Length, 
        panAngles.Length,
        tiltAngles.Length
      );
      SetPhenotypes(phenotypes);
      CreateGenes();
  }

  public string ID { get; } = System.Guid.NewGuid().ToString();
  public int CamerasCount { get; private set; }
  public bool Evaluated { get; set; }
  public double Score { get; set; }
  public Vector3 CameraPosition { 
    get {
      return this.possiblePositions[this.GetPhenotypes()[0].PositionIndex];
    }
  }

  public double PanAngle { 
    get {
      return this.panAngles[this.GetPhenotypes()[0].PanIndex];
    }
  }

  public double TiltAngle { 
    get {
      return this.tiltAngles[this.GetPhenotypes()[0].TiltIndex];
    }
  }

  public override IChromosome CreateNew()
  {
      return new CameraChromosome(
        this.possiblePositions,
        this.panAngles,
        this.tiltAngles
      );
  }
}