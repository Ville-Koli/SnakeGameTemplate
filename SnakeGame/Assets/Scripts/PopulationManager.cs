using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject snakeGamePrefab;
    private List<GameObject> individuals = new List<GameObject>();
    private GeneticAlgorithm.Population population;
    private List<GeneticAlgorithm.FeedForwardNeuralNetwork> autoPlayers = new List<GeneticAlgorithm.FeedForwardNeuralNetwork>();
    [SerializeField] private int amount;
    [SerializeField] private int size;
    [SerializeField] private int rowMax;
    [SerializeField] private float generationTimer = 10f;
    [SerializeField] private float inputCooldown = 0.2f;
    private int generationCount = 0;
    private float inputCooldownTimer = 0f;
    private float totalTimer = 0f;
    public int deadCount = 0;
    public bool writeNetwork = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        population = new GeneticAlgorithm.Population(GeneticAlgorithm.neatConfig);
        int yCounter = 0;
        int xCounter = 1;
        for(int i = 1; i < amount + 1; ++i){
            individuals.Add(Instantiate(snakeGamePrefab));
            Game game = individuals[i - 1].GetComponent<Game>();
            game.Bounds += new Vector4(xCounter, -yCounter, xCounter, -yCounter)*size;
            game.Start();
            game.GetParentObject().name = "Gameboard " + (i - 1);
            individuals[i - 1].name = "Initializer " + (i - 1);

            if(i % rowMax == 0){
                yCounter++;
                xCounter = 0;
            }
            xCounter++;
        }
        for(int i = 0; i < population.population.Count; ++i){
            autoPlayers.Add(GeneticAlgorithm.create_from_genome(population.population[i].genome));
        }
        Debug.Log(Application.persistentDataPath);
    }

    void CallInputsForIndividuals(){
        for(int i = 0; i < individuals.Count; ++i){
            if(i < autoPlayers.Count){
                List<double> input = individuals[i].GetComponent<InputData>().GetData().ToList();
                List<double> output = autoPlayers[i].activate(input);
                individuals[i].GetComponent<InputSystem>().CallInputFromList(output);
            }else{
                break;
            }
        }
    }

    void GenerateNextGeneration(){
        if(writeNetwork){
            GeneticAlgorithm.NetworkSaver.write_networks(Path.Combine(Application.persistentDataPath, "savedNetworks.txt"), autoPlayers[0]);
            Debug.Log(Application.persistentDataPath);
            writeNetwork = false;
        }
        // set fitnesses
        for(int i = 0; i < population.population.Count; ++i){
            population.population[i].fitness = individuals[i].GetComponent<Game>().GetFitness() + 1;
        }
        // REWRITE THIS FOR GENOME_B
        Debug.Log("AFT PLAYER COUNT : " + population.population.Count);
        double bestFitness = population.population.Max(elem => elem.fitness);
        Debug.Log("NEW GENERATION " + generationCount + " ! best fitness: " + bestFitness + " neuron count: " + population.population[0].genome.neurons.Count + " link count: " + population.population[0].genome.links.Count + " max neuron count: " + population.population.Max(elem => elem.genome.neurons.Count) + " max link count: " + population.population.Max(elem => elem.genome.links.Count));
        population.population = population.reproduce();
        Debug.Log("new players count: " + population.population.Count);
        for(int i = 0; i < population.population.Count; ++i){
            autoPlayers[i] = GeneticAlgorithm.create_from_genome(population.population[i].genome);
        }
        ++generationCount;
    }

    void ResetAllIndividuals(){
        foreach(var game in individuals){
            game.GetComponent<Game>().RestartGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        totalTimer += Time.deltaTime;
        inputCooldownTimer += Time.deltaTime;
        if(inputCooldownTimer > inputCooldown){
            CallInputsForIndividuals();
            inputCooldownTimer -= inputCooldown;
        }
        if(deadCount >= population.population.Count - 1){
            GenerateNextGeneration();
            ResetAllIndividuals();
            totalTimer = 0;
            deadCount = 0;            
        }
        if(totalTimer > generationTimer){
            GenerateNextGeneration();
            ResetAllIndividuals();
            totalTimer -= generationTimer;
            deadCount = 0;
        }
    }
}
