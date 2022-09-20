using System.Collections.Concurrent;
using System.Threading;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

public class CoverageFitness : IFitness
{  
  public BlockingCollection<CameraChromosome> ChromosomesToEvaluate { get; private set; }

  public CoverageFitness() {
    ChromosomesToEvaluate = new BlockingCollection<CameraChromosome>();
  }

	public double Evaluate (IChromosome chromosome)
	{
    CameraChromosome c = chromosome as CameraChromosome;
    ChromosomesToEvaluate.Add(c);

    do {} while (!c.Evaluated);

    return c.Score;
	}
}