using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;
using System.Collections;

public class CoverageGA : MonoBehaviour
{
  private GeneticAlgorithm ga;
  private CoverageFitness fitness;

  private void Start() {
    Debug.Log("Initalizing Genetic Algorithm...");

    var selection = new EliteSelection();
    var crossover = new UniformCrossover();
    var mutation = new ReverseSequenceMutation();
    this.fitness = new CoverageFitness();
    var chromosome = new CameraChromosome(new Vector3(0, 0, 0), new Vector3(1,1,1));
    var population = new Population (50, 70, chromosome);

    this.ga = new GeneticAlgorithm(population, this.fitness, selection, crossover, mutation);
    this.ga.Termination = new GenerationNumberTermination(100);
    this.ga.GenerationRan += delegate
    {
        double score = ((CameraChromosome)this.ga.BestChromosome).Score;
        Debug.Log($"Generation: {this.ga.GenerationsNumber} - Score: ${score}");
    };
    Debug.Log("GA running...");

    CameraController cam = Camera.main.GetComponent<CameraController>();
    cam.setChromossome(chromosome);

    StartCoroutine(waiter());
  }

  IEnumerator waiter()
  {
      //Wait for 4 seconds
      yield return new WaitForSeconds(4);
      this.ga.Start();
  }

  void Update()
  {
    CameraChromosome c;
    this.fitness.ChromosomesToEvaluate.TryTake(out c);
    CameraController cam = Camera.main.GetComponent<CameraController>();
    cam.setChromossome(c);

  }
}