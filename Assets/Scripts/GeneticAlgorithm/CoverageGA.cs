using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;
using System.Threading;

public class CoverageGA : MonoBehaviour
{
  private GeneticAlgorithm ga;
  private CoverageFitness fitness;
  private Thread gaThread;
  private CameraConfigService cameraConfigService;

  private void Awake() {
    this.cameraConfigService = new CameraConfigService();
    this.cameraConfigService.instantiateCameras(2);
  }

  private void Start() {
    Debug.Log("Initalizing Genetic Algorithm...");

    var selection = new EliteSelection();
    var crossover = new UniformCrossover();
    var mutation = new ReverseSequenceMutation();
    this.fitness = new CoverageFitness();
    CameraAreaService cameraAreaService = new CameraAreaService();
    var chromosome = new CameraChromosome(
      cameraAreaService.getAllPossiblePositions(),
      this.cameraConfigService.getPanAngles(),
      this.cameraConfigService.getTiltAngles(),
      2
    );
    var population = new Population (minSize: 50, 70, chromosome);

    this.ga = new GeneticAlgorithm(population, this.fitness, selection, crossover, mutation);
    this.ga.Termination = new GenerationNumberTermination(20);
    this.ga.GenerationRan += delegate
    {
        double score = ((CameraChromosome)this.ga.BestChromosome).Score;
        Debug.Log($"Generation: {this.ga.GenerationsNumber} - Score: ${score}");
    };

    Debug.Log("GA running..."); 
    this.gaThread = new Thread(() => this.ga.Start());
  }

  void Update()
  {
    if (this.ga.State == GeneticAlgorithmState.Started) {
      CameraChromosome c;
      if(this.fitness != null && this.fitness.ChromosomesToEvaluate.Count > 0) {
        this.fitness.ChromosomesToEvaluate.TryTake(out c);
        if (c != null) {
          setChromosomeInCameras(c);
        }
      }
    } else if(this.ga.State == GeneticAlgorithmState.TerminationReached && this.ga.State != GeneticAlgorithmState.Stopped) {
        Debug.Log("GA terminated..."); 
        var bestChromosome = (CameraChromosome)this.ga.BestChromosome;
        if (bestChromosome != null) {
          Debug.Log($"Best chromossome: {bestChromosome.Fitness}"); 
          setChromosomeInCameras(bestChromosome);
        }
        this.ga.Stop();
        this.gaThread.Abort();
    }
  }

  private void setChromosomeInCameras(CameraChromosome c) {
    var cameras = Camera.allCameras;
    foreach (var cam in cameras) {
      CameraController camController = cam.GetComponent<CameraController>();
      camController.setChromossome(c);
    }
  }

  public void startGA() {
    if (this.gaThread.ThreadState == ThreadState.Unstarted) {
      this.gaThread.Start();
    }
  }

  private void OnDestroy()
  {
      this.ga.Stop();
      this.gaThread.Abort();
  }
}