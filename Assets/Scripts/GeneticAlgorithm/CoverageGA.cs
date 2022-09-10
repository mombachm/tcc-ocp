using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;
using System.Collections;
using GeneticSharp.Infrastructure.Framework.Threading;
using System.Threading;

public class CoverageGA : MonoBehaviour
{
  private GeneticAlgorithm ga;
  private CoverageFitness fitness;
  private Thread gaThread;

  private void Start() {
    Debug.Log("Initalizing Genetic Algorithm...");

    var selection = new EliteSelection();
    var crossover = new UniformCrossover();
    var mutation = new ReverseSequenceMutation();
    this.fitness = new CoverageFitness();
    CameraAreaService cameraAreaService = new CameraAreaService();
    CameraConfigService cameraConfigService = new CameraConfigService();
    var chromosome = new CameraChromosome(
      cameraAreaService.getAllPossiblePositions(),
      cameraConfigService.getPanAngles(),
      cameraConfigService.getTiltAngles()
    );
    var population = new Population (50, 70, chromosome);

    this.ga = new GeneticAlgorithm(population, this.fitness, selection, crossover, mutation);
    this.ga.Termination = new GenerationNumberTermination(5);
    this.ga.GenerationRan += delegate
    {
        double score = ((CameraChromosome)this.ga.BestChromosome).Score;
        Debug.Log($"Generation: {this.ga.GenerationsNumber} - Score: ${score}");
    };
    // this.ga.TaskExecutor = new ParallelTaskExecutor
    //     {
    //         MinThreads = 100,
    //         MaxThreads = 200
    //     };

    Debug.Log("GA running...");
    this.gaThread = new Thread(() => this.ga.Start());
    this.gaThread.Start();
    //StartCoroutine(waiter());
  }

  IEnumerator waiter()
  {
      //Wait for 4 seconds
      yield return new WaitForSeconds(1);

  }

  void Update()
  {
    if (this.ga.State == GeneticAlgorithmState.Started) {
      CameraChromosome c;
      if(this.fitness != null && this.fitness.ChromosomesToEvaluate.Count > 0) {
        this.fitness.ChromosomesToEvaluate.TryTake(out c);

        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (c != null) {
          cam.setChromossome(c);
        }
      }
    } else if(this.ga.State == GeneticAlgorithmState.TerminationReached) {
        double score = ((CameraChromosome)this.ga.BestChromosome).Score;
        var bestChromosome = (CameraChromosome)this.ga.BestChromosome;
        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (bestChromosome != null) {
          cam.setChromossome(bestChromosome);
        }
        this.ga.Stop();
    }
  }

  private void OnDestroy()
  {
      this.ga.Stop();
      this.gaThread.Abort();
  }
}