using UnityEngine;

public class Diagram : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleA;
    [SerializeField] private CircleCollider2D circleB;
    private Vector3 mousePos;
    private Camera cam;

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

}
