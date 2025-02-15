using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject snakeGamePrefab;
    private List<GameObject> individuals = new List<GameObject>();
    [SerializeField] private int amount;
    [SerializeField] private int size;
    [SerializeField] private int rowMax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int yCounter = 0;
        int xCounter = 1;
        for(int i = 1; i < amount; ++i){
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
