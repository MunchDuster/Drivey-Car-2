using UnityEngine;

[RequireComponent(typeof(AITeacher))]
public class ShowTopBots : MonoBehaviour
{
	public int showBots;


	private AITeacher teacher;
	private BotInput[] topBots;


	// Start is called before the first frame update
	void Start()
	{
		teacher = GetComponent<AITeacher>();
		topBots = new BotInput[showBots];
	}


	// Update is called once per frame
	void Update()
	{
		if (teacher.BatchBots.Length <= 0 || teacher.BatchBots[0] == null) return;
		//Hide the old best bots
		foreach (BotInput botInput in topBots)
		{
			botInput.transform.Find("Meshes").gameObject.SetActive(false);
		}

		//Get the new best bots
		for (int i = 0; i < teacher.BatchBots.Length; i++)
		{
			bool hasbeenplaced = false;
			for (int j = 0; j < showBots && !hasbeenplaced; j++)
			{
				if (teacher.BatchBots[i].network.GetFitness() > topBots[j].network.GetFitness())
				{
					hasbeenplaced = true;

					Insert(ref topBots, j, teacher.BatchBots[j]);
				}
			}
		}
		//show the new best bots
		foreach (BotInput botInput in topBots)
		{
			botInput.transform.Find("Meshes").gameObject.SetActive(true);
		}

	}
	void Insert(ref BotInput[] array, int index, BotInput bot)
	{
		for (int i = index + 1; i < array.Length; i++)
		{
			array[i] = array[i - 1];
		}
		array[index] = bot;
	}

	public void SetList()
	{
		Debug.Log("Setting list");
		for (int i = 0; i < topBots.Length; i++)
		{
			topBots[i] = teacher.BatchBots[i];
		}
	}
}
