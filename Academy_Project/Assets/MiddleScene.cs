using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MiddleScene : MonoBehaviour
{
    public Image Panel;
    void OnEnable()
    {
        Panel.DOFade(0.0f, 0.75f);
    }
    public void LoadObject1()
    {
        GameManager.Instance.currentObject = 0;
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
  {
      UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
  });
    }
    public void LoadObject2()
    {
        GameManager.Instance.currentObject = 1;
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
});
    }
    public void LoadObject3()
    {
        GameManager.Instance.currentObject = 2;
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
});
    }

    public void goToMainMenu()
    {
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        });
    }
}
