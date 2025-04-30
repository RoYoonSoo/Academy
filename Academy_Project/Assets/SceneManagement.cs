using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] Image Panel;

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
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
        {
            SceneManager.LoadScene("Game1Scene");
        });
    }
    public void LoadScene2()
    {
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
        {
            SceneManager.LoadScene("Game2Scene");
        });
    }
}
