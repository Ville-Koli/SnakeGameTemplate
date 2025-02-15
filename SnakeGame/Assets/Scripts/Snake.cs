using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private List<GameObject> _snakeBody = new List<GameObject>();
    private GameObject _snakeHead;
    [SerializeField] private Texture2D snakeTexture;
    [SerializeField] private float speed;
    [SerializeField] private float snakeSizeScalar;
    [SerializeField] private Vector3 _directionVector = new Vector2(0, 1);
    [SerializeField] public Vector3 Size {get; set;}
    public float Speed {
        get {return speed;}
        set {speed = value;}
    }
    /**
    <summary> Function which generates the snake head </summary>
    <param name="parent"> parent object of the generated snake bodypart </param>
    **/
    public void GenerateSnakeHead(GameObject parent){
        _snakeHead = new GameObject();
        _snakeHead.name = "snakeHead";
        _snakeHead.transform.SetParent(parent.transform);
        // make snake head gameobject from given information
        SpriteRenderer sr = _snakeHead.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(snakeTexture, new Rect(0, 0, snakeTexture.width, snakeTexture.height), new Vector2(0.5f, 0.5f));
        sr.material.color = Color.green;
        _snakeHead.transform.localScale = new Vector3(1,1,1)*snakeSizeScalar;
        sr.sortingOrder = 1;
        Size = sr.bounds.size;
    }
    /**
    <summary> Function, which updates snake head and its body parts
    and checks for collisions between head and body parts. If collision detected, it sets
    notCollision to false.</summary>
    <param name="Bounds"> game area bounds </param>
    <param name="notCollision"> notCollision will be updated to false, whenever snake collides with itself </param>
    **/
    public void UpdateSnake(out bool notCollision, Vector4 Bounds){
        notCollision = true;
        if(_snakeBody.Count > 0){
            _snakeBody[^1].transform.position = _snakeHead.transform.position;
        }
        // update snake head
        _snakeHead.transform.position = Logic.KeepInBounds(_snakeHead.transform.position + _directionVector * speed, Bounds);

        // update snake body
        for(int i = 1; i < _snakeBody.Count; ++i){
            _snakeBody[i - 1].transform.position = _snakeBody[i].transform.position;
            // check for collision
            if(_snakeHead.transform.position == _snakeBody[i - 1].transform.position
               && i - 1 != _snakeBody.Count - 1){
               notCollision = false;
               break;
            }
        }
    }
    /**
    <summary> Function, which returns length of snakes body list </summary>
    <returns> amount of elements in snake body </returns>
    **/
    public int GetSnakeBodyCount(){
        return _snakeBody.Count;
    }
    /**
    <summary> Function, which sets the direction vector </summary>
    <param name="dir"> new direction vector </param>
    **/
    public void SetDirectionVector(Vector3 dir){
        _directionVector = dir;
    }
    /**
    <summary> Function, which returns snake heads position </summary>
    <returns> snake heads position </returns>
    **/
    public Vector3 GetSnakeHeadPosition(){
        return _snakeHead.transform.position;
    }
    /**
    <summary> Function, which sets snake head position to a new position</summary>
    <param name="position"> new position </param>
    **/
    public void SetSnakeHeadPosition(Vector3 position){
        _snakeHead.transform.position = position;
    }
    /**
    <summary> Function, which clears snake body 
    by destroying all game objects and then clearing 
    list from invalid references
    </summary>
    **/
    public void ClearSnake(){
        foreach(var body in _snakeBody){
            Destroy(body);
        }
        _snakeBody.Clear();
    }

    /**
    <summary> Function which generates a new body part for the snake </summary>
    <param name="parent"> parent object of the generated snake bodypart </param>
    **/
    public void GenerateSnakeBody(GameObject parent){
        GameObject newBodyPart = Instantiate(_snakeHead);
        newBodyPart.name = "Snake Bodypart " + _snakeBody.Count;
        newBodyPart.transform.SetParent(parent.transform);
        _snakeBody.Add(newBodyPart);
    }
    /**
    <summary> Function, which checks whether snake head has been succesfully generated </summary>
    <returns> bool, which represents whether snake head is null </returns>
    **/
    public bool IsSnakeHeadGenerated(){
        return _snakeHead == null;
    }
}
