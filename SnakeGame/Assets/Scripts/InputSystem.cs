using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    private Dictionary<string, Func<bool>> _inputs = new Dictionary<string, Func<bool>>();
    [SerializeField] private InputData data;
    [SerializeField] private Snake snake;
    public bool allowPlayerControl = true;
    public float[] recentInputFromPlayer = new float[5];
    public List<float> dataOnInputPress;
    public void CallInput(string input){
        if(_inputs.ContainsKey(input))
            _inputs[input]();
    }

    public void CallInputFromList(List<float> values){
        float maxvalue = 0;
        int maxInd = -1;
        for(int i = 0; i < values.Count; ++i){
            if(maxvalue < values[i]){
                maxvalue = values[i];
                maxInd = i; 
            }
        }
        CallInput(maxInd);
    }
    public void CallInputFromList(List<double> values){
        double maxvalue = 0;
        int maxInd = -1;
        if(values != null){
            for(int i = 0; i < values.Count; ++i){
                if(maxvalue < values[i]){
                    maxvalue = values[i];
                    maxInd = i; 
                }
            }
        }
        CallInput(maxInd);
    }

    public void CallInput(int input){
        switch(input){
            case 0:
                _inputs["w"]();
                break;
            case 1:
                _inputs["s"]();
                break;
            case 2:
                _inputs["a"]();
                break;
            case 3:
                _inputs["d"]();
                break;
            case 4:
                _inputs[""]();
                break;
        }
    }
    public void SetInputVector(string input){
        switch(input){
            case "w":
                recentInputFromPlayer = new float[5]{1, 0, 0, 0, 0};
                break;
            case "s":
                recentInputFromPlayer = new float[5]{0, 1, 0, 0, 0};
                break;
            case "a":
                recentInputFromPlayer = new float[5]{0, 0, 1, 0, 0};
                break;
            case "d":
                recentInputFromPlayer = new float[5]{0, 0, 0, 1, 0};
                break;
        }
    }
    public Func<bool> GetInput(string input){
        if(_inputs.ContainsKey(input))
            return _inputs[input];
        return () => {return true;};
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        if(_inputs.Count > 0) return;
        // set input system
        _inputs.Add("", () => {return true;}); // default action
        _inputs.Add("w", () => {snake.SetDirectionVector(new Vector2(0, 1) * snake.Size.y); return true;});
        _inputs.Add("s", () => {snake.SetDirectionVector(new Vector2(0, -1) * snake.Size.y); return true;});
        _inputs.Add("a", () => {snake.SetDirectionVector(new Vector2(-1, 0) * snake.Size.x); return true;});
        _inputs.Add("d", () => {snake.SetDirectionVector(new Vector2(1, 0) * snake.Size.x); return true;});
        snake.SetDirectionVector(new Vector2(1, 0) * snake.Size.y); // set to be default direction
    }

    // Update is called once per frame
    void Update()
    {
        if(!allowPlayerControl) return;
        // input system
        if(_inputs.ContainsKey(Input.inputString)){
             _inputs[Input.inputString]();
             if(Input.inputString != ""){
                SetInputVector(Input.inputString);
                dataOnInputPress = data.GetDataAsFloat().ToList();
             }
        }
    }
}
