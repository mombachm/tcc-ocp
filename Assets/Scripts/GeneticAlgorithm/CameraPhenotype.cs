using UnityEngine;
using GeneticSharp.Runner.UnityApp.Commons;

public class CameraPhenotype : PhenotypeEntityBase
{
    public CameraPhenotype(int camPosLength, int panAngleLength, int tiltAngleLength)
    {
        Phenotypes = new IPhenotype[] {
            new Phenotype("camPos", 15) {
              MinValue = 0,
              MaxValue = camPosLength - 1
            },
            new Phenotype("panAngle", 15) {
              MinValue = 0,
              MaxValue = panAngleLength - 1
            },
            new Phenotype("tiltAngle", 15) {
              MinValue = 0,
              MaxValue = tiltAngleLength - 1
            }
        };
    }

    public int PositionIndex
    {
        get {
          return (int)Phenotypes[0].Value;
        }
    }

    public int PanIndex
    {
        get {
          return (int)Phenotypes[1].Value;
        }
    }

    public int TiltIndex
    {
        get {
          return (int)Phenotypes[2].Value;
        }
    }
}
