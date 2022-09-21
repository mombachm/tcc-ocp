using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;
using System.Threading;
using System.Collections;

public class CoverageGA : MonoBehaviour
{
  const int CAMERA_COUNT = 3;
  private GeneticAlgorithm ga;
  private CoverageFitness fitness;
  private Thread gaThread;
  private CameraConfigService cameraConfigService;

  private CameraChromosome bestChromosome;

  private CameraChromosome currentChromosome;

  private void Awake() {
    this.cameraConfigService = new CameraConfigService();
    this.cameraConfigService.instantiateCameras(CAMERA_COUNT);
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
      CAMERA_COUNT
    );
    var population = new Population (minSize: 20, 50, chromosome);

    this.ga = new GeneticAlgorithm(population, this.fitness, selection, crossover, mutation);
    this.ga.Termination = new GenerationNumberTermination(30);
    this.ga.GenerationRan += delegate
    {
        double score = ((CameraChromosome)this.ga.BestChromosome).Score;
        Debug.Log($"Generation: {this.ga.GenerationsNumber} - Score: ${score} | Population: {this.ga.ToString()}");
    };
  }

  void Update()
  {

    if (this.ga.State == GeneticAlgorithmState.Started) {
      CameraChromosome c;
      if(this.fitness != null && this.fitness.ChromosomesToEvaluate.Count > 0 && (this.currentChromosome == null || this.currentChromosome.Evaluated == true)) {
        this.fitness.ChromosomesToEvaluate.TryTake(out c);
        if (c != null) {
          this.currentChromosome = c;
          setChromosomeInCameras(c);
        }
      }
    } else if (this.ga.State == GeneticAlgorithmState.TerminationReached && this.bestChromosome == null) {
      Debug.Log("GA terminated..."); 
      setBestChromosomeInScene();
    }
  }

  private void setChromosomeInCameras(CameraChromosome c) {
    var cameras = Camera.allCameras;
    foreach (var cam in cameras) {
      CameraController camController = cam.GetComponent<CameraController>();
      camController.setChromossome(c);
    }
  }

  public void setBestChromosomeInScene() {
    var bestChromosome = (CameraChromosome)this.ga.BestChromosome;
    if (bestChromosome != null) {
      this.bestChromosome = bestChromosome;
      Debug.Log($"Best chromossome: {this.bestChromosome.Fitness}"); 
      setChromosomeInCameras(this.bestChromosome);
    }
  }

  public void startGA() {
    if (this.gaThread == null) {
      this.bestChromosome = null;
      this.gaThread = new Thread(() => this.ga.Start());
      this.gaThread.Start();
      Debug.Log("GA running..."); 
    }
  }

  public void stopGA() {
    if (this.gaThread != null) {
      this.ga.Stop();
      this.gaThread.Abort();
      this.gaThread = null;
      Debug.Log("GA Stopped."); 
    }
  }

  private void OnDestroy()
  {
      this.ga.Stop();
      this.gaThread.Abort();
  }
}