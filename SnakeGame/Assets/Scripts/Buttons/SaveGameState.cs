using System.IO;
using UnityEngine;

public class SaveGameState : MonoBehaviour
{
    [SerializeField] private Game game;
    [SerializeField] private HighscoreFileEditer fileEditer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnApplicationQuit()
    {
        fileEditer.WriteNewHighscore(game.GetSnake().GetSnakeBodyCount(), game.Highscore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
