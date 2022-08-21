using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Runner.UnityApp.Commons;
using System.Collections.Generic;
using GeneticSharp.Domain.Randomizations;

public class CameraChromosome : BitStringChromosome<CameraPhenotype>
{
  private Vector3 minPosition, maxPosition;
  public CameraChromosome(Vector3 minPosition, Vector3 maxPosition)
  {
      this.minPosition = minPosition;
      this.maxPosition = maxPosition;
      var phenotypes = new CameraPhenotype(minPosition, maxPosition);
      SetPhenotypes(phenotypes);
      CreateGenes();
  }

  public string ID { get; } = System.Guid.NewGuid().ToString();
  public int CamerasCount { get; private set; }
  public bool Evaluated { get; set; }
  public double Score { get; set; }

  public override IChromosome CreateNew()
  {
      return new CameraChromosome(this.minPosition, this.maxPosition);
  }
}