using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button quit_button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quit_button.onClick.AddListener(() => {Application.Quit();});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
