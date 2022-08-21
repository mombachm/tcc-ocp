using System.Collections.Concurrent;
using System.Threading;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

public class CoverageFitness : IFitness
{  
  public BlockingCollection<CameraChromosome> ChromosomesToEvaluate { get; private set; }
	public double Evaluate (IChromosome chromosome)
	{
    CameraChromosome c = chromosome as CameraChromosome;
    do
    {
        Thread.Sleep(100);
    } while (!c.Evaluated);

    return c.Score;
	}
}