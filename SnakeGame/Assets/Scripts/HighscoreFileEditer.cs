using System.IO;
using UnityEngine;

public class HighscoreFileEditer : MonoBehaviour
{
    public int FetchBestHighscore(){
        int highScore = 0;
        // fetch best highscore
        string loc = Application.persistentDataPath;
        string path = Path.Combine(loc, "SaveData.txt");
        if(File.Exists(path)){
            using(StreamReader streamReader = new StreamReader(path)){
                bool didParse = int.TryParse(streamReader.ReadLine(), out int result);
                highScore = result;
            }
        }
        return highScore;
    }
    public void WriteNewHighscore(int newHighscore, int oldHighscore){
        string loc = Application.persistentDataPath;
        string path = Path.Combine(loc, "SaveData.txt");
        if(!File.Exists(path)){
            File.Create(path);
        }
        if(oldHighscore <= newHighscore){
            using(StreamWriter fs = new StreamWriter(path)){
                fs.Write(newHighscore);
                fs.Close();
            }
        }
    }

}
