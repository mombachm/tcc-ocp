using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Runner.UnityApp.Commons;
using System.Collections.Generic;
using GeneticSharp.Domain.Randomizations;

public class CameraChromosome : BitStringChromosome<CameraPhenotype>
{
  private Vector3[] possiblePositions;
  public CameraChromosome(Vector3[] possiblePositions)
  {
      this.possiblePositions = possiblePositions;
      var phenotypes = new CameraPhenotype(this.possiblePositions.Length);
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

  public override IChromosome CreateNew()
  {
      return new CameraChromosome(this.possiblePositions);
  }
}