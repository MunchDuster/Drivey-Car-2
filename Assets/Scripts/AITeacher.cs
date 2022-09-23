using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AITeacher : MonoBehaviour
{
	[Header("Batch and generation settings")]
	public int botsPerBatch = 100;
	public int batches = 20;
	public float timePerBatch = 15;
	public float maxTimePerBatch = 35;
	public float timeIncreasePerGen = 1;
	public int toppersTakenPerBatch = 10;
	public int toppersTakenPerGeneration = 10;

	[Header("Bot settings")]
	public int botMiddleLayers = 2;
	public int botMiddleLayerNeurons = 10;
	[Range(0f, 1f)]
	public float mutationChance = .001f;
	[Header("References")]
	public Transform spawnPoint;
	public Transform botParent;

	public GameObject prefab;
	public SetupBotInput botSetupper;

	[Header("UI")]
	public TMP_Text generationText;
	public TMP_Text batchText;
	public TMP_Text botsText;
	public TMP_Text timeText;

	[Header("Events")]
	public UnityEvent<int> OnNextGeneration;
	public UnityEvent<int> OnNextBatch;
	
	//private vars

	public BotInput[] BatchBots;
	private NeuralNetwork[][] GenerationNets;

	public NeuralNetwork OverallBestNet;
	private NeuralNetwork[] GenerationBestNets;

	private int batch;
	private int generation;

	private float batchRunTime;
	private float roundStartTime = 10000000000;//Set to big number so that round doesn't start before use starts it

	private int[] netLayers;
	GameSettings gameSettings;

	//for the ui
	public int getBatch()
	{
		return batch;
	}
	public int getGeneration()
	{
		return generation;
	}
	
	//Start is called before the first frame
	private void Start()
	{

		//get game settings
		gameSettings = GameSettings.current;
		Debug.Log("gameSettings " + gameSettings);
		if (!gameSettings.isAIActive)
		{
			Debug.Log("YEET");
			Destroy(gameObject);
		}

	}
	// Update is called once per frame
	void Update()
	{

		//if the time since the roundhas started is more than the allowed time, start new round.
		if (Time.time - roundStartTime >= batchRunTime)
		{
			NextRound();
		}
		else
		{
			float timeleft = (batchRunTime - (Time.time - roundStartTime));
			timeText.text = "Time left: " + makeDigits(timeleft, 1).ToString();
		}
	}
	float makeDigits(float num, int digits)
	{
		return ((int)num * Mathf.Pow(10, digits)) / Mathf.Pow(10, digits);
	}


	//For the start button onpress
	public void BeginTeaching()
	{
		batchRunTime = timePerBatch;

		//setup layering structure for neural networks
		netLayers = new int[botMiddleLayers + 2];

		netLayers[0] = prefab.GetComponent<BotInput>().getInputLength();
		for (int i = 0; i < botMiddleLayers; i++)
		{
			netLayers[i + 1] = botMiddleLayerNeurons;
		}
		netLayers[botMiddleLayers + 1] = 2;

		//Lists init
		BatchBots = new BotInput[batches];
		GenerationNets = new NeuralNetwork[batches][];

		OverallBestNet = GenerateRandomNet();
		GenerationBestNets = new NeuralNetwork[toppersTakenPerGeneration];

		//Init generation
		if (gameSettings.startWithSaved)
		{
			SaveData loadedData = SaveSystem.Load(gameSettings.loadIndex);
			if (loadedData != null)
			{
				//Load success, use loaded nets
				OverallBestNet = loadedData.overallBestNet;
				//Set gen and batch
				generation = loadedData.generation;
				batch = loadedData.batch;
				//Mutate and go
				GenerateNewNets();
			}
			else
			{
				//Load failed, generate fresh batch
				GenerateRandomGeneration();
			}
		}
		else
		{
			//Start gen from fresh
			GenerateRandomGeneration();
		}

		//Start batch
		roundStartTime = Time.time;
		SpawnBots();

		updateText();

		//Fill the generation best
		for (int i = 0; i < toppersTakenPerGeneration; i++)
		{
			GenerationBestNets[i] = GenerationNets[batch][i];
		}
	}

	//Creates a generation with random nets
	void GenerateRandomGeneration()
	{
		//Init gen and batch
		generation = 0;
		batch = 0;

		for (int b = 0; b < batches; b++)
		{
			NeuralNetwork[] batchNets = new NeuralNetwork[botsPerBatch];
			for (int i = 0; i < botsPerBatch; i++)
			{
				//genereate a random network
				NeuralNetwork net = GenerateRandomNet();
				batchNets[i] = net;
			}
			GenerationNets[b] = batchNets;
		}
	}
	//Creates a single random net
	NeuralNetwork GenerateRandomNet()
	{
		//make the neural network
		NeuralNetwork net = new NeuralNetwork(netLayers);
		//modify it
		MutateNet(ref net,1f);
		//return it
		return net;
	}
	//Starts next batch or generation
	void NextGeneration()
	{
		//Reset batch number
		batch = 0;
		//increment generation number
		generation++;

		Debug.Log("GENERATION " + generation);


		//increase batch run time
		batchRunTime = Mathf.Clamp(timePerBatch + timeIncreasePerGen * generation, 0, maxTimePerBatch);

		//Update Overall Bests
		CompareGenerationToOverall();

		OnNextGeneration.Invoke(generation);
		

		//Clear generation best list
		System.Array.Clear(GenerationBestNets, 0, GenerationBestNets.Length);

		//Populate Generation nets
		GenerateNewNets();

		//Fill the generation best
		for (int i = 0; i < toppersTakenPerGeneration; i++)
		{
			GenerationBestNets[i] = GenerationNets[0][i];
		}
	}
	
	void NextRound()
	{
		SortBatch();
		for (int i = 0; i < toppersTakenPerBatch; i++)
		{
			Debug.Log("BeshOF Bash: " + GenerationNets[batch][i].GetFitness());
		}
		CompareBatchToGeneration();

		batch++;
		if (batch + 1 > batches)
		{
			//start new generation
			NextGeneration();
		}
		else
		{
			OnNextBatch.Invoke(batch);
		}

		//start new batch
		Debug.Log("Starting Batch " + batch);

		updateText();

		roundStartTime = Time.time;
		ResetBots();
	}
	void updateText()
	{
		generationText.text = "Generation: " + (generation + 1);
		batchText.text = "Batch: " + (batch + 1) + "/" + batches;
		botsText.text = "Bots per batch: " + botsPerBatch;
	}

	//generate a new generation of nets mutated off the best overalls
	void GenerateNewNets()
	{
		for (int b = 0; b < batches; b++)
		{
			NeuralNetwork[] batchNets = new NeuralNetwork[botsPerBatch];

			for (int i = 0; i < botsPerBatch; i++)
			{
				//copy a the overall best
				NeuralNetwork net = new NeuralNetwork(OverallBestNet);//lerpNets(OverallBestNet[Random.Range(0, toppersTakenOverall)],OverallBestNet[Random.Range(0, toppersTakenOverall)],Random.Range(0f,1f));

				//mutate
				MutateNet(ref net, mutationChance);

				//add to batch
				batchNets[i] = net;
			}
			GenerationNets[b] = batchNets;
		}
	}
	//goes through batch and get top (toppersTakenPerBatch) scoring nets
	void SortBatch()
	{
		//organise batch
		for (int i = 0; i < toppersTakenPerBatch; i++)
		{
			int replacementIndex = -1;
			float hightestSoFar = BatchBots[i].network.GetFitness();
			for (int indexInBatch = i + 1; indexInBatch < botsPerBatch; indexInBatch++)
			{
				if (BatchBots[indexInBatch].network.GetFitness() > hightestSoFar)
				{
					hightestSoFar = BatchBots[indexInBatch].network.GetFitness();
					replacementIndex = indexInBatch;
				}
			}
			if (replacementIndex != -1)
			{
				BotInput temp = BatchBots[replacementIndex];
				BatchBots[replacementIndex] = BatchBots[i];
				BatchBots[i] = temp;


			}
			Debug.Log("Batch BOT " + i + ": " + BatchBots[i].network.GetFitness());
		}
	}
	//goes through generation best and compare to overall bests
	void CompareBatchToGeneration()
	{
		for (int g = 0; g < toppersTakenPerGeneration; g++)
		{
			bool placed = false;
			for (int b = 0; b < botsPerBatch && !placed; b++)
			{
				if (BatchBots[b].network.GetFitness() > GenerationBestNets[g].GetFitness())
				{
					placed = true;
					Insert(ref GenerationBestNets, g, BatchBots[b].network);
				}
			}
		}
	}
	//insert a value into an array at a specific index
	void Insert(ref NeuralNetwork[] array, int index, NeuralNetwork net)
	{
		for (int i = index + 1; i < array.Length; i++)
		{
			array[i] = array[i - 1];
		}
		array[index] = net;
	}
	//goes through best of batch and compare to generation bests
	void CompareGenerationToOverall()
	{
			for (int g = 0; g < toppersTakenPerGeneration; g++)
			{
				if (GenerationBestNets[g].GetFitness() > OverallBestNet.GetFitness())
				{
					OverallBestNet = GenerationBestNets[g];
				}
			}
		
		for (int g = 0; g < toppersTakenPerGeneration; g++)
		{
			Debug.Log("GENERATION BESTNET: " + GenerationBestNets[g].GetFitness());
		}
		Debug.Log("OVERALL BESTNET: " + OverallBestNet.GetFitness());
	}
	//spawns bots into map
	void SpawnBots()
	{
		Debug.Log("SPAWNING");
		BatchBots = new BotInput[botsPerBatch];

		for (int i = 0; i < botsPerBatch; i++)
		{
			NeuralNetwork net = GenerationNets[batch][i];

			//create gameObject
			GameObject newBotObj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, botParent);

			//assign net to bot and enable bot inputs to its car
			BotInput bot = newBotObj.GetComponent<BotInput>();
			bot.network = net;
			botSetupper.SetupBot(bot);
			bot.inputEnabled = true;

			//add bot to list
			BatchBots[i] = bot;
		}
	}
	void ResetBots()
	{
		for (int i = 0; i < botsPerBatch; i++)
		{
			NeuralNetwork net = GenerationNets[batch][i];			

			//assign net to bot and enable bot inputs to its car
			BotInput bot = BatchBots[i];
			bot.network = net;
			botSetupper.SetupBot(bot);

			//add bot to list
			BatchBots[i] = bot;

			//put the gameobject at the correct position
			bot.transform.position = spawnPoint.position;
			bot.transform.rotation = spawnPoint.rotation;
		}
	}

	//modify a neural network slightly (depending on chance)
	public void MutateNet(ref NeuralNetwork network, float chance)
	{
		//no chance of mutation, leave as be
		if (chance <= 0) return;

		//adjust weights
		float[][][] weights = network.GetWeights();
		for (int i = 0; i < weights.Length; i++)
		{
			for (int j = 0; j < weights[i].Length; j++)
			{
				for (int k = 0; k < weights[i][j].Length; k++)
				{
					float value = Random.Range(0f, 1f);
					if (value <= chance)
					{
						Modify(ref weights[i][j][k]);
					}
				}
			}
		}
		network.CopyWeights(weights);

		//adjust biases
		float[][] biases = network.GetBiases();
		for (int i = 0; i < biases.Length; i++)
		{
			for (int j = 0; j < biases[i].Length; j++)
			{
				float value = Random.Range(0f, 1f);
				if (value <= chance)
				{
					Modify(ref biases[i][j]);
				}
			}
		}
		network.CopyBiases(biases);
	}
	//modification used in mutateNet
	void Modify(ref float value)
	{
		float chance = Random.Range(0f, 1f);
		if (chance <= 0.2f)
			value += Random.Range(0f, 1f);
		else if (chance <= 0.4f)
			value -= Random.Range(-1f, 0f);
		else if (chance <= 0.6f)
			value *= Random.Range(1f, 2f);
		else if (chance <= 0.8f)
			value *= -1;
		else
			value /= Random.Range(1f, 2f);
	}
}
