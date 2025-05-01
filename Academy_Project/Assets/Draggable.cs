using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform areaA;
    [SerializeField] private RectTransform areaB;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPos;

    private enum DropArea { None, AOnly, BOnly, Intersect }

    private Dictionary<DropArea, string> areaObjectMap = new Dictionary<DropArea, string>
    {
        { DropArea.AOnly, "ABC" },
        { DropArea.BOnly, "F" },
        { DropArea.Intersect, "DE" }
    };

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 mouseScreenPos = Input.mousePosition;

        bool inA = IsInsideCircle(areaA, mouseScreenPos);
        bool inB = IsInsideCircle(areaB, mouseScreenPos);

        DropArea dropArea = DropArea.None;

        if (inA && inB) dropArea = DropArea.Intersect;
        else if (inA) dropArea = DropArea.AOnly;
        else if (inB) dropArea = DropArea.BOnly;

        if (dropArea == DropArea.None)
        {
            Debug.Log("어느 영역에도 없음 → 되돌림");
            rectTransform.anchoredPosition = originalPos;
        }
        else
        {
            string objNames = areaObjectMap[dropArea];
            Debug.Log($"[{dropArea}] 위치에 드롭됨 → 해당 오브젝트: {objNames}");
        }
    }

    private bool IsInsideCircle(RectTransform area, Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            area, screenPoint, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out Vector2 localPoint);

        float radius = area.rect.width * 0.5f;
        return localPoint.magnitude < radius;
    }
}
