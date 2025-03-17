using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    private GameObject parent;
    private float totalTimer = 0f;
    public float analysisTimer = 0f;
    private int highScore = 0;
    private List<(float time, float size)> data = new List<(float time, float size)>();
    public bool printData = false;
    [SerializeField] private Snake snake;
    [SerializeField] public Vector4 Bounds;
    [SerializeField] private float updateTimer = 0f;
    [SerializeField] private bool isGameRunning = true;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private FoodSpawner spawner;
    [SerializeField] private HighscoreFileEditer fileEditer;
    [SerializeField] private PopulationManager pm;
    public int isNextTileSnakeForward = 0;
    public int isNextTileSnakeLeft = 0;
    public int isNextTileSnakeRight = 0;
    public int Highscore {
        get {return highScore;}
    }
    public float UpdateTimer {get {return updateTimer;}}
    public int[] GetIsNextTileSnake(){
        return new int[3] {isNextTileSnakeForward, isNextTileSnakeLeft, isNextTileSnakeRight};
    }
    /**
    <summary> Function, which returns the snake </summary>
    <returns> current snake instance </returns>
    **/
    public Snake GetSnake(){
        return snake;
    }

    public float GetFitness(){
        return snake.GetSnakeBodyCount();
    }
    /**
    <summary> Function, which 
    returns the parent object of all objects related to
    current instance of the game 
    </summary>
    <returns> returns the gameboard object </returns>
    **/
    public GameObject GetParentObject(){
        return parent;
    }
    /**
    <summary> Function, which restarts the game </summary>
    **/
    public void RestartGame(){
        snake.ClearSnake();
        highScore = 0;
        analysisTimer = 0f;
        snake.SetSnakeHeadPosition(new Vector3(Bounds.x, Bounds.y) + snake.Size);
        isGameRunning = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        if(!snake.IsSnakeHeadGenerated()) return;
        parent = new GameObject(); 
        parent.name = "Gameboard";
        snake.GenerateSnakeHead(parent);
        // calculate real bounds from bounds
        Bounds.x = (int)(Bounds.x / snake.Size.x) * snake.Size.x;
        Bounds.y = (int)(Bounds.y / snake.Size.y) * snake.Size.y;
        Bounds.z = (int)(Bounds.z / snake.Size.x) * snake.Size.x;
        Bounds.w = (int)(Bounds.w / snake.Size.y) * snake.Size.y;
        // set snake location
        snake.SetSnakeHeadPosition(new Vector3(Bounds.x, Bounds.y) + snake.Size);
        highScore = fileEditer.FetchBestHighscore();
        highScoreText.text = highScore.ToString();  
        spawner.GenerateFood(parent);
    }
    /**
    <summary> Function, which checks whether game has ended and restarts the game if it has ended</summary>
    **/
    bool HasGameEnded(){
        if(!isGameRunning){
            // set current highscore
            if(snake.GetSnakeBodyCount() > highScore){
                highScore = snake.GetSnakeBodyCount();
                highScoreText.text = highScore.ToString();
            }
            //RestartGame();
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if(printData){
            string dataString = "";
            foreach(var elem in data){
                dataString += $"({(int)elem.time}, {elem.size}), ";
            }
            Debug.Log(dataString);
            printData = false;
        }
        if(HasGameEnded()){
            return;
        }
        totalTimer += Time.deltaTime;
        analysisTimer += Time.deltaTime;
        // update snake
        if(totalTimer > updateTimer){
            snake.UpdateSnake(out isGameRunning, out isNextTileSnakeForward, out isNextTileSnakeLeft, out isNextTileSnakeRight, Bounds);
            totalTimer -= updateTimer;
        }
        // snake eating the food logic
        if(spawner.Current_food != null && Logic.IsInMarginOfError(snake.GetSnakeHeadPosition(), spawner.Current_food.transform.position, 0.0005f)){
            snake.GenerateSnakeBody(parent);
            Destroy(spawner.Current_food);
            spawner.GenerateFood(parent);
            scoreText.text = snake.GetSnakeBodyCount().ToString();
            data.Add((analysisTimer, snake.GetSnakeBodyCount()));
        }
        if(!isGameRunning){ pm.deadCount += 1; }
    }
}
