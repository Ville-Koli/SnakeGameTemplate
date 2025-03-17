using UnityEngine;

public class InputData : MonoBehaviour
{
    [SerializeField] private Snake snake;
    [SerializeField] private FoodSpawner spawner;
    [SerializeField] private Game game;
    [SerializeField] private double [] input_data = new double[7];
    public double[] GetData(){
        return input_data;
    }
    public float[] GetDataAsFloat(){
        float[] input_data_f = new float[input_data.Length];
        for(int i = 0; i < input_data.Length; ++i){
            input_data_f[i] = (float) input_data[i];
        }
        return input_data_f;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //input_data[0] = Vector3.Angle(snake.DirectionVector.normalized, spawner.Current_food.transform.position.normalized - snake.GetSnakeHeadPosition().normalized)/180;
        //input_data[1] = Vector3.Distance(snake.GetSnakeHeadPosition(), spawner.Current_food.transform.position); 
        /**
        input_data[0] = snake.GetSnakeHeadPosition().x / game.Bounds.z;
        input_data[1] = snake.GetSnakeHeadPosition().y / game.Bounds.w;
        input_data[2] = spawner.Current_food.transform.position.x / game.Bounds.z;
        input_data[3] = spawner.Current_food.transform.position.y / game.Bounds.w;
        input_data[4] = snake.GetTailPosition().x / game.Bounds.z;
        input_data[5] = snake.GetTailPosition().y / game.Bounds.w;**/
        int[] tiles = game.GetIsNextTileSnake();
        input_data[0] = Vector3.Distance(snake.GetSnakeHeadPosition(), spawner.Current_food.transform.position); 
        input_data[1] = snake.DirectionVector.normalized.x;
        input_data[2] = snake.DirectionVector.normalized.y;
        input_data[3] = Vector3.Angle(snake.DirectionVector.normalized, spawner.Current_food.transform.position.normalized - snake.GetSnakeHeadPosition().normalized)/180;
        // new input
        input_data[4] = tiles[0];
        input_data[5] = tiles[1];
        input_data[6] = tiles[2];
    }
}
