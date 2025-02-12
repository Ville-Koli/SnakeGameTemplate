using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    private List<GameObject> _snakeBody = new List<GameObject>();
    private GameObject _snakeHead;
    public Texture2D snakeTexture;
    public Texture2D borderTexture;
    private Vector3 _directionVector = new Vector2(0, 1);
    public float speed = 1;
    public float snakeSize;
    private List<(KeyCode key, Func<bool> func)> _inputs = new List<(KeyCode, Func<bool>)>();
    public Vector4 bounds = new Vector4(0, 0, 10, 10);
    public float updateTimer = 0f;
    private float totalTimer = 0f;
    public Vector3 size;
    public FoodSpawner spawner;
    public bool isGameRunning = true;
    public int highScore = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    /**
    <summary> Function, which returns length of snakes body list </summary>
    <returns> amount of elements in snake body </returns>**/
    public int GetSnakeBodyCount(){
        return _snakeBody.Count;
    }

    /**
    <summary> Function, which updates snake head and its body parts
    and checks for collisions between head and body parts. If collision detected, it sets
    notCollision to false.
    </summary>
    **/
    void UpdateSnake(out bool notCollision){
        notCollision = true;
        if(_snakeBody.Count > 0){
            _snakeBody[^1].transform.position = _snakeHead.transform.position;
        }
        // update snake head
        _snakeHead.transform.position = Logic.KeepInBounds(_snakeHead.transform.position + _directionVector * speed, bounds);
        // update snake body
        for(int i = 1; i < _snakeBody.Count; ++i){
            _snakeBody[i - 1].transform.position = _snakeBody[i].transform.position;
            // check for collision
            if(_snakeHead.transform.position == _snakeBody[i - 1].transform.position
               && i - 1 != _snakeBody.Count - 1) 
               notCollision = false;
        }
    }
    /**
    <summary> Function which generates a new body part for the snake </summary>**/
    void GenerateSnakeBody(){
        GameObject newBodyPart = Instantiate(_snakeHead);
        newBodyPart.name = "S" + _snakeBody.Count;
        _snakeBody.Add(newBodyPart);
    }
    /**
    <summary> Function, which generates the borders for the snake game. </summary>
    **/
    void MakeGameBorders(float sizeT, float offset, float scalar){
        // Create the first border
        GameObject borderX = new GameObject();
        sizeT += offset;
        sizeT *= scalar;
        SpriteRenderer sr = borderX.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 3;
        sr.sprite = Sprite.Create(borderTexture, new Rect(0, 0, borderTexture.width, borderTexture.height), new Vector2(0.5f, 0.5f));
        sr.material.color = Color.black;
        borderX.transform.localScale = new Vector3(sizeT, 1, 1);
        borderX.transform.position = new Vector3(bounds.x * size.x + bounds.z * size.x/2, bounds.w * size.y + offset);
        // then we copy said border and edit its position and scale for the other borders
        GameObject borderX2 = Instantiate(borderX);
        borderX2.transform.position = new Vector3(bounds.x * size.x + bounds.z * size.x/2, bounds.y * size.y - offset);
        GameObject borderY1 = Instantiate(borderX);
        borderY1.transform.position = new Vector3(bounds.x * size.x - offset, bounds.y * size.y + bounds.w * size.y/2);
        borderY1.transform.localScale = new Vector3(1, sizeT, 1);
        GameObject borderY2 = Instantiate(borderX);
        borderY2.transform.position = new Vector3(bounds.z * size.x + offset, bounds.y * size.y + bounds.w * size.y/2);
        borderY2.transform.localScale = new Vector3(1, sizeT, 1);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        if(_snakeHead != null) return;

        _snakeHead = new GameObject();
        _snakeHead.name = "snakeHead";
        // make snake head gameobject from given information
        SpriteRenderer sr = _snakeHead.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(snakeTexture, new Rect(0, 0, snakeTexture.width, snakeTexture.height), new Vector2(0.5f, 0.5f));
        sr.material.color = Color.green;
        _snakeHead.transform.localScale = new Vector3(1,1,1)*snakeSize;
        sr.sortingOrder = 1;
        size = sr.bounds.size;
        // generate borders
        MakeGameBorders(bounds.w, 0.3f, 4);
        // set input system
        _inputs.Add((KeyCode.W, () => {_directionVector = new Vector2(0, 1) * sr.bounds.size.y; return true;}));
        _inputs.Add((KeyCode.S, () => {_directionVector = new Vector2(0, -1) * sr.bounds.size.y; return true;}));
        _inputs.Add((KeyCode.A, () => {_directionVector = new Vector2(-1, 0) * sr.bounds.size.x; return true;}));
        _inputs.Add((KeyCode.D, () => {_directionVector = new Vector2(1, 0) * sr.bounds.size.x; return true;}));
        _directionVector = new Vector2(1, 0) * sr.bounds.size.x;
        // calculate real bounds
        bounds.x = bounds.x * size.x;
        bounds.y = bounds.y * size.y;
        bounds.z = bounds.z * size.x;
        bounds.w = bounds.w * size.y;
        // set snake location
        _snakeHead.transform.position = new Vector3(bounds.x, bounds.y);
        // fetch best highscore
        string loc = Application.persistentDataPath;
        string path = Path.Combine(loc, "SaveData.txt");
        if(File.Exists(path)){
            using(StreamReader streamReader = new StreamReader(path)){
                bool didParse = int.TryParse(streamReader.ReadLine(), out int result);
                highScore = result;
                highScoreText.text = "High Score: " + highScore;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameRunning){
            // set current highscore
            if(_snakeBody.Count > highScore){
                highScore = _snakeBody.Count;
                highScoreText.text = "High Score: " + highScore;
            }
            return;
        } 

        totalTimer += Time.deltaTime;
        // input system
        foreach(var input in _inputs)
            if(Input.GetKeyDown(input.key))
                input.func();
        // update snake
        if(totalTimer > updateTimer){
            UpdateSnake(out isGameRunning);
            totalTimer -= updateTimer;
        }
        // snake eating the food logic
        if(_snakeHead.transform.position == spawner.current_food.transform.position){
            GenerateSnakeBody();
            Destroy(spawner.current_food);
            spawner.GenerateFood();
            scoreText.text = "Current Score: " + _snakeBody.Count;
        }
    }
}
