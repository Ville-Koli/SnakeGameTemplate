using System;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject current_food;
    public Texture2D food_texture;
    public Game game;
    private Vector3 space;
    /**
    <summary>
    Function, which generates a food entity to the field within bounds of the game.
    </summary>
     **/
    public void GenerateFood(){
        game.Start();
        current_food = new GameObject();
        SpriteRenderer sr = current_food.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 2; // make food object be on top of everything (so always visible)
        sr.sprite = Sprite.Create(food_texture, new Rect(0, 0, food_texture.width, food_texture.height), new Vector2(0.5f, 0.5f));

        space = new Vector3(
        UnityEngine.Random.Range(1, (int)MathF.Abs(game.bounds.z - game.bounds.x)) + game.bounds.x, 
        UnityEngine.Random.Range(1, (int)MathF.Abs(game.bounds.w - game.bounds.y)) + game.bounds.y)/ (game.size.x * game.speed);

        space.x = ((int) space.x) * game.size.x * game.speed;
        space.y = ((int) space.y) * game.size.y * game.speed;

        current_food.transform.position = space;
        sr.material.color = Color.red;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateFood();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
