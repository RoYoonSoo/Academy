using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene1()
    {
        SceneManager.LoadScene("Game1Scene");
    }
    public void LoadScene2()
    {
        SceneManager.LoadScene("Game2Scene");
    }
}
