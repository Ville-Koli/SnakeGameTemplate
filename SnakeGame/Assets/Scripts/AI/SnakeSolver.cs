using System;
using System.Collections;
using UnityEngine;

public class InputSequence{
    private string[] inputs = new string[]{"d", "s", "d", "w"};
    private int current = 0;
    public string Next(){
        current = (current + 1) % inputs.Length;
        return inputs[current];
    }
    public string Get(){
        return inputs[current];
    }
}



public class SnakeSolver : MonoBehaviour
{
    [SerializeField] private Snake snake;
    [SerializeField] private Game game;
    [SerializeField] private InputSystem inputSystem;
    private InputSequence inputSequence;
    private bool hasBeenAtTop = false;
    bool IsAtTop(Snake snake, Vector4 bounds){
        if(snake.GetSnakeHeadPosition().y >= bounds.w - snake.Size.y){
            return true;
        }
        return false;
    }
    bool IsAtBottom(Snake snake, Vector4 bounds){
        if(snake.GetSnakeHeadPosition().y <= bounds.y){
            return true;
        }
        return false;
    }
    IEnumerator TurnAround(){
        inputSystem.CallInput(inputSequence.Get());
        yield return new WaitForSeconds(game.UpdateTimer);
        inputSystem.CallInput(inputSequence.Next());
        inputSequence.Next();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game.Start();
        inputSystem.Start();
        inputSequence = new InputSequence();
        inputSystem.CallInput("w");
    }

    // Update is called once per frame
    void Update()
    {
        if(IsAtTop(snake, game.Bounds) && !hasBeenAtTop){
            StartCoroutine(TurnAround());
            hasBeenAtTop = true;
        }else if(IsAtBottom(snake, game.Bounds) && hasBeenAtTop){
            StartCoroutine(TurnAround());
            hasBeenAtTop = false;
        }
    }
}
