using System.IO;
using UnityEngine;

public class SaveGameState : MonoBehaviour
{
    public Game game;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnApplicationQuit()
    {
        string loc = Application.persistentDataPath;
        string path = Path.Combine(loc, "SaveData.txt");
        if(!File.Exists(path)){
            File.Create(path);
        }
        if(game.highScore <= game.GetSnakeBodyCount()){
            using(StreamWriter fs = new StreamWriter(path)){
                fs.Write(game.GetSnakeBodyCount());
                fs.Close();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
