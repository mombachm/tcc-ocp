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
using GeneticSharp.Infrastructure.Framework.Threading;

public class CoverageGA : MonoBehaviour
{
  const int CAMERA_COUNT = 7;
  private GeneticAlgorithm ga;
  private CoverageFitness fitness;
  private Thread gaThread;
  private CameraConfigService cameraConfigService;

  private CameraChromosome bestChromosome;

  private CameraChromosome currentChromosome;

  private void Awake() {
    this.cameraConfigService = new CameraConfigService();
    this.cameraConfigService.instantiateCameras(CAMERA_COUNT);
    Utils.disableGlassColliders();
  }

  private void Start() {
    Debug.Log("Initalizing Genetic Algorithm...");

    var selection = new EliteSelection();
    var selection2 = new TournamentSelection();
    var crossover = new UniformCrossover();
    var mutation = new FlipBitMutation();
    this.fitness = new CoverageFitness();
    CameraAreaService cameraAreaService = new CameraAreaService();
    var chromosome = new CameraChromosome(
      cameraAreaService.getAllPossiblePositions(),
      this.cameraConfigService.getPanAngles(),
      this.cameraConfigService.getTiltAngles(),
      CAMERA_COUNT
    );
    var population = new Population (minSize: 100, 100, chromosome);

    this.ga = new GeneticAlgorithm(population, this.fitness, selection2, crossover, mutation);
    this.ga.MutationProbability = 0.2f;
    this.ga.Termination = new FitnessStagnationTermination(10); 
    // new FitnessThresholdTermination(100);//new GenerationNumberTermination(20);


    this.ga.GenerationRan += delegate
    {
      var bestChromo = ((CameraChromosome)this.ga.Population.CurrentGeneration.BestChromosome);
      Debug.Log($"Generation: {this.ga.GenerationsNumber} - Score: {bestChromo.Score}");
      var generationData = new GenerationData();
      generationData.bestScore = bestChromo.Score;
      generationData.generationNumber = this.ga.GenerationsNumber;
      generationData.bestPriorityCoverage = bestChromo.PriorityCoverage;
      generationData.bestPrivacyCoverage = bestChromo.PrivacyCoverage;
      LogService.generationData.Add(generationData);
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
      logBestChromosome();
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

  public void logBestChromosome() {
    var bestChromosome = (CameraChromosome)this.ga.BestChromosome;
    if (bestChromosome != null) {
      this.bestChromosome = bestChromosome;
      Debug.Log($"Best chromossome: {this.bestChromosome.Fitness}"); 
      LogService.bestPriorityCoverage = this.bestChromosome.PriorityCoverage;
      LogService.bestPrivacyCoverage = this.bestChromosome.PrivacyCoverage;
      LogService.logToCSV();
    }
  }

  public void setBestChromosomeInScene() {
    var bestChromosome = (CameraChromosome)this.ga.BestChromosome;
    if (bestChromosome != null) {
      this.bestChromosome = bestChromosome;
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