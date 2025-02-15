using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public Button start_button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_button.onClick.AddListener(() => {SceneManager.LoadScene("SampleScene");});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
