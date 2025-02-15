using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    private GameObject parent;
    private float totalTimer = 0f;
    private int highScore = 0;
    private Dictionary<string, Func<bool>> _inputs = new Dictionary<string, Func<bool>>();
    [SerializeField] private Snake snake;
    [SerializeField] public Vector4 Bounds;
    [SerializeField] private float updateTimer = 0f;
    [SerializeField] private bool isGameRunning = true;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private FoodSpawner spawner;
    [SerializeField] private HighscoreFileEditer fileEditer;
    [SerializeField] private BorderGenerator borderGenerator;
    public int Highscore {
        get {return highScore;}
    }

    public Snake GetSnake(){
        return snake;
    }
    public GameObject GetParentObject(){
        return parent;
    }
    public void RestartGame(){
        snake.ClearSnake();
        highScore = 0;
        snake.SetSnakeHeadPosition(new Vector3(Bounds.x, Bounds.y));
        isGameRunning = true;
    }

    public bool IsInMarginOfError(Vector3 a, Vector3 b, float error){
        return Math.Abs((a.x - b.x)/a.x) < error && Math.Abs((a.y - b.y)/a.y) < error;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        if(!snake.IsSnakeHeadGenerated()) return;
        parent = new GameObject(); 
        parent.name = "Gameboard";
        snake.GenerateSnakeHead(parent);
        // set input system
        _inputs.Add("", () => {return true;}); // default action
        _inputs.Add("w", () => {snake.SetDirectionVector(new Vector2(0, 1) * snake.Size.y); return true;});
        _inputs.Add("s", () => {snake.SetDirectionVector(new Vector2(0, -1) * snake.Size.y); return true;});
        _inputs.Add("a", () => {snake.SetDirectionVector(new Vector2(-1, 0) * snake.Size.x); return true;});
        _inputs.Add("d", () => {snake.SetDirectionVector(new Vector2(1, 0) * snake.Size.x); return true;});
        snake.SetDirectionVector(new Vector2(1, 0) * snake.Size.y); // set to be default direction
        // calculate real bounds
        Bounds.x = (int)(Bounds.x/snake.Size.x) * snake.Size.x;
        Bounds.y = (int)(Bounds.y/snake.Size.y) * snake.Size.y;
        Bounds.z = (int)(Bounds.z/snake.Size.x) * snake.Size.x;
        Bounds.w = (int)(Bounds.w/snake.Size.y) * snake.Size.y;
        // set snake location
        snake.SetSnakeHeadPosition(new Vector3(Bounds.x, Bounds.y));
        highScore = fileEditer.FetchBestHighscore();
        highScoreText.text = highScore.ToString();  
        spawner.GenerateFood(parent);
    }

    bool HasGameEnded(){
        if(!isGameRunning){
            // set current highscore
            if(snake.GetSnakeBodyCount() > highScore){
                highScore = snake.GetSnakeBodyCount();
                highScoreText.text = highScore.ToString();
            }
            RestartGame();
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if(HasGameEnded()) return;
        totalTimer += Time.deltaTime;
        // input system
        if(_inputs.ContainsKey(Input.inputString))
            _inputs[Input.inputString]();
        // update snake
        if(totalTimer > updateTimer){
            snake.UpdateSnake(out isGameRunning, Bounds);
            totalTimer -= updateTimer;
        }
        // snake eating the food logic
        if(spawner.Current_food != null && IsInMarginOfError(snake.GetSnakeHeadPosition(), spawner.Current_food.transform.position, 0.0005f)){
            snake.GenerateSnakeBody(parent);
            Destroy(spawner.Current_food);
            spawner.GenerateFood(parent);
            scoreText.text = snake.GetSnakeBodyCount().ToString();
        }
    }
}
