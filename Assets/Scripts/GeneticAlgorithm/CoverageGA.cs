using UnityEngine;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;

public class CoverageGA : MonoBehaviour {
  private GeneticAlgorithm ga;
  private CoverageFitness fitness;
  private Thread gaThread;
  public CameraConfigService cameraConfigService;
  private CameraChromosome bestChromosome;
  private CameraChromosome currentChromosome;
  public int testCamCount = 1;
  public const int testMaxCamCount = 15;
  public float testWPriv = 0;
  public const float testMaxWPriv = 1;
  public int populationGA = 10;
  private CoverageService coverageService = new CoverageService();

  CameraChromosome chromosomeToEval;
  private void Awake() {}

  private void Start() {
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = 9999;
    Utils.initializeColliders();
    initializeCameras();
  }

  private void Update() {
    if (this.ga is null) return;
    if (this.ga.State == GeneticAlgorithmState.Started) {
      if(this.fitness != null && this.fitness.ChromosomesToEvaluate.Count > 0 && (this.currentChromosome == null || this.currentChromosome.Evaluated == true)) {
        CameraChromosome c;
        this.fitness.ChromosomesToEvaluate.TryTake(out c);
        if (c != null) {
          setChromosomeInCameras(c);
          coverageService.resetCullingInfo();          
          this.currentChromosome = c;
          return;
        }
      }

    } else if (this.ga.State == GeneticAlgorithmState.TerminationReached && this.bestChromosome == null) {
      Debug.Log("GA terminated..."); 
      setRandomDataScoreToLog();
      logBestChromosome();
      setBestChromosomeInScene();
      captureCameraImages();
    }
  }

  private void FixedUpdate() {
    if (this.currentChromosome is not null && this.currentChromosome.Evaluated == false) {
      calculateChromossomeScore(this.currentChromosome);
    }
  }

  private void calculateChromossomeScore(CameraChromosome c) {
    if (c != null && c.Evaluated == false) {
      TotalCoverageData totalCovData = coverageService.getTotalCoverageData();
      c.PriorityCoverage = totalCovData.PriorityCoverage;
      c.MultiPriorityCoverage = totalCovData.MultiPriorityCoverage;
      c.PrivacyCoverage = totalCovData.PrivacyCoverage;
      c.Score = totalCovData.Score;
      // Debug.Log($"{c.ID} - {c.Score}");
      c.Evaluated = true;
    }
  }

  private void initializeGA() {
    Debug.Log(message: "Initalizing Genetic Algorithm...");
    this.initializeCameras();
    var selection = new RouletteWheelSelection();
    var crossover = new UniformCrossover();
    var mutation = new ReverseSequenceMutation();
    this.fitness = new CoverageFitness();
    CameraAreaService cameraAreaService = new CameraAreaService();
    var chromosome = new CameraChromosome(
      cameraAreaService.getAllPossiblePositions(),
      this.cameraConfigService.getPanAngles(),
      this.cameraConfigService.getTiltAngles(),
      Constants.CAM_COUNT
    );
    var population = new Population (minSize: 100, maxSize: 100, chromosome);

    this.ga = new GeneticAlgorithm(population, this.fitness, selection, crossover, mutation);
    this.ga.MutationProbability = 0.2f;
    this.ga.Termination = new FitnessStagnationTermination(15);//new GenerationNumberTermination(150); //new FitnessStagnationTermination(15); //new GenerationNumberTermination(expectedGenerationNumber: 3);
    // new FitnessThresholdTermination(100);//new GenerationNumberTermination(20);
    this.ga.GenerationRan += delegate {
      var bestChromo = ((CameraChromosome)this.ga.Population.CurrentGeneration.BestChromosome);
      Debug.Log($"Generation: {this.ga.GenerationsNumber} - Score: {bestChromo.Score}");
      var generationData = new GenerationData();
      generationData.bestScore = bestChromo.Score;
      generationData.generationNumber = this.ga.GenerationsNumber;
      generationData.bestPriorityCoverage = bestChromo.PriorityCoverage;
      generationData.bestPrivacyCoverage = bestChromo.PrivacyCoverage;
      generationData.bestMultiPrioCoverage = bestChromo.MultiPriorityCoverage;
      generationData.avgTimeCalcCoverage = LogService.sumGenCalcCovTime / LogService.contCalcCovCalls;
      Debug.Log($"AVG TIME: {generationData.avgTimeCalcCoverage} / SUM: {LogService.sumGenCalcCovTime} / CONT: {LogService.contCalcCovCalls}");
      LogService.resetTimeCalcCov();
      LogService.generationData.Add(generationData);

    };
  }

  private void initializeCameras() {
    this.cameraConfigService = new CameraConfigService();
    this.cameraConfigService.instantiateCameras(Constants.CAM_COUNT);
  }

  private void setChromosomeInCameras(CameraChromosome c) {
    var cameras = Camera.allCameras;
    foreach (var cam in cameras) {
      CameraController camController = cam.GetComponent<CameraController>();
      camController.setChromossome(c);
    }
  }

  private void captureImagesFromCameras(string fileText) {
    var cameras = Camera.allCameras;
    foreach (var cam in cameras) {
      CameraController camController = cam.GetComponent<CameraController>();
      camController.captureImage(Constants.testName, $"{Constants.testName} - {Constants.CAM_COUNT} CAMS - W MULTICAM {this.testWPriv}", $"{Constants.testName}-{Constants.CAM_COUNT} CAMS-{fileText} - W MULTICAM {this.testWPriv}");
    }
  }

  public void logBestChromosome() {
    var bestChromosome = (CameraChromosome)this.ga.BestChromosome;
    if (bestChromosome != null) {
      this.bestChromosome = bestChromosome;
      Debug.Log($"Best chromossome: {this.bestChromosome.Fitness}"); 
      LogService.bestScore = this.bestChromosome.Score;
      LogService.bestPriorityCoverage = this.bestChromosome.PriorityCoverage;
      LogService.bestPrivacyCoverage = this.bestChromosome.PrivacyCoverage;
      LogService.bestMultiPrioCoverage = this.bestChromosome.MultiPriorityCoverage;
      LogService.logToCSV(this, Constants.testName);
    }
  }

  public void setBestChromosomeInScene() {
    var bestChromosome = (CameraChromosome)this.ga.BestChromosome;
    if (bestChromosome != null) {
      this.bestChromosome = bestChromosome;
      setChromosomeInCameras(this.bestChromosome);
    }
  }

  public void captureCameraImages() {
    StartCoroutine( CaptureOnlyPrioAreas() );
  }

  private void setRandomDataScoreToLog() {
    cameraConfigService.randomPositionCameras();
    TotalCoverageData totalCovDataRandom = MainController.getTotalCoverageData();
    LogService.randomCovData = totalCovDataRandom;
  }

  private IEnumerator CaptureOnlyPrioAreas() {
    Constants.SHOW_COV_AREAS = true;
    Constants.SHOW_PRIV_AREAS = false;
    Constants.SHOW_CAM_AREAS = false;
      // process pre-yield
    yield return new WaitForSecondsRealtime(0.1f);
    captureImagesFromCameras("A");
    Constants.SHOW_COV_AREAS = true;
    Constants.SHOW_PRIV_AREAS = true;
    Constants.SHOW_CAM_AREAS = true;
    yield return StartCoroutine( CaptureOnlyPrivAreas() );
  }

  private IEnumerator CaptureOnlyPrivAreas() {
    Constants.SHOW_COV_AREAS = false;
    Constants.SHOW_PRIV_AREAS = true;
    Constants.SHOW_CAM_AREAS = false;
      // process pre-yield
    yield return new WaitForSecondsRealtime(0.1f);
    captureImagesFromCameras("B");
    Constants.SHOW_COV_AREAS = true;
    Constants.SHOW_PRIV_AREAS = true;
    Constants.SHOW_CAM_AREAS = true;
    yield return StartCoroutine( CaptureOnlyScene() );
  }

  private IEnumerator CaptureOnlyScene() {
    Constants.SHOW_COV_AREAS = false;
    Constants.SHOW_PRIV_AREAS = false;
    Constants.SHOW_CAM_AREAS = false;
      // process pre-yield
    yield return new WaitForSecondsRealtime(0.1f);
    captureImagesFromCameras("C");
    Constants.SHOW_COV_AREAS = true;
    Constants.SHOW_PRIV_AREAS = true;
    Constants.SHOW_CAM_AREAS = true;
    yield return StartCoroutine( CaptureAllAreas() );
  }

  private IEnumerator CaptureAllAreas() {
    Constants.SHOW_COV_AREAS = true;
    Constants.SHOW_PRIV_AREAS = true;
    Constants.SHOW_CAM_AREAS = true;
      // process pre-yield
    yield return new WaitForSecondsRealtime(0.1f);
    captureImagesFromCameras("D");
    Constants.SHOW_COV_AREAS = true;
    Constants.SHOW_PRIV_AREAS = true;
    Constants.SHOW_CAM_AREAS = true;

    if (Constants.TEST_ROUTINE) {
      testRoutineW();
    }
  }

  public IEnumerator startGA() {
    if (this.gaThread == null) {
      this.bestChromosome = null;
      initializeGA();
      this.gaThread = new Thread(() => this.ga.Start());
      yield return new WaitForSecondsRealtime(0.5f);
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

  public void startGARoutine() {
    StartCoroutine(this.startGA());
  }

  public void testRoutine() {
    stopGA();
    if (this.testCamCount > testMaxCamCount) return;
    Constants.CAM_COUNT = this.testCamCount;
    StartCoroutine(startGA());
    this.testCamCount++;
  }

  public void testRoutineW() {
    stopGA();
    if (this.populationGA > 150) return;
    // Constants.WEIGHT_PRIV = this.testWPriv;

      Debug.Log($"POPULATION: {this.populationGA}");
    StartCoroutine(startGA());
    this.populationGA = this.populationGA + 10;
  }
}