using System;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject Current_food {get; private set;}
    [SerializeField] private Texture2D food_texture;
    [SerializeField] private Game game;
    private Vector3 space;
    /**
    <summary>
    Function, which generates a food entity to the field within bounds of the game.
    </summary>
     **/
    public void GenerateFood(GameObject parent){
        game.Start();
        Current_food = new GameObject();
        Current_food.transform.SetParent(parent.transform);
        SpriteRenderer sr = Current_food.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 2; // make food object be on top of everything (so always visible)
        sr.sprite = Sprite.Create(food_texture, new Rect(0, 0, food_texture.width, food_texture.height), new Vector2(0.5f, 0.5f));
        Snake snake = game.GetSnake();
        
        space = new Vector3(
        UnityEngine.Random.Range(1, (int)MathF.Abs(game.Bounds.z - game.Bounds.x)) + game.Bounds.x, 
        UnityEngine.Random.Range(1, (int)MathF.Abs(game.Bounds.w - game.Bounds.y)) + game.Bounds.y) / (snake.Size.x * snake.Speed);

        space.x = ((int) space.x) * snake.Size.x * snake.Speed;
        space.y = ((int) space.y) * snake.Size.y * snake.Speed;
        space.z = 0;
        Current_food.name = "food";

        Current_food.transform.position = space;
        sr.material.color = Color.red;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
