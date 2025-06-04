using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Diagram : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleA;
    [SerializeField] private CircleCollider2D circleB;
    private Vector3 mousePos;
    private Camera cam;
    public Image Panel;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 screenPos = Input.mousePosition;

        bool inA = circleA.OverlapPoint(screenPos);
        bool inB = circleB.OverlapPoint(screenPos);
    }
    public void sceneChange()
    {
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
    });
    }
}
