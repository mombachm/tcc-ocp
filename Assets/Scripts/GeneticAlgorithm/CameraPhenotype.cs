using UnityEngine;
using GeneticSharp.Runner.UnityApp.Commons;

public class CameraPhenotype : PhenotypeEntityBase
{
    public CameraPhenotype(int camPosLength)
    {
        Phenotypes = new IPhenotype[] {
            new Phenotype("camPos", 15) {
              MinValue = 0,
              MaxValue = camPosLength - 1
            }
        };
    }

    public int PositionIndex
    {
        get {
          return (int)Phenotypes[0].Value;
        }
    }
}
