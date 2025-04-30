using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Game1Manager : MonoBehaviour
{
    [SerializeField] Image Panel;

    private void Awake()
    {
        Panel.DOFade(0.0f, 0.75f);
    }
    public void SceneManage()
    {
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });
    }
}
