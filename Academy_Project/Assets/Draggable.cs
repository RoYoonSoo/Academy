using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform areaA;
    [SerializeField] private RectTransform areaB;
    [SerializeField] private RectTransform intersectArea; // 겹치는 영역 슬롯

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPos;
    private Transform originalParent; // 드래그 전 부모 저장
    private DropArea currentArea = DropArea.None; // 현재 위치한 영역 추적

    private enum DropArea { None, AOnly, BOnly, Intersect }
    public Image correctImg;
    public Image Panel;

    // 각 영역에 배치된 오브젝트 상태를 추적하는 정적 Dictionary (모든 객체가 공유)
    private static Dictionary<DropArea, List<string>> areaObjectMap = new Dictionary<DropArea, List<string>>
    {
        { DropArea.AOnly, new List<string>() },
        { DropArea.BOnly, new List<string>() },
        { DropArea.Intersect, new List<string>() }
    };
    private static Dictionary<DropArea, List<string>> answerMap = new Dictionary<DropArea, List<string>>
    {
        { DropArea.AOnly, new List<string>(){"A","B"} },
        { DropArea.BOnly, new List<string>(){ "E"} },
        { DropArea.Intersect, new List<string>(){ "C", "D"}  }
    };
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalPos = rectTransform.anchoredPosition; // 초기 배치 위치 저장
        Panel.DOFade(0.0f, 0.75f);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 전 부모와 현재 영역 저장
        originalParent = transform.parent;

        // 드래그 시 부모를 canvas로 변경
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 초기 위치와의 거리 측정
        float returnThreshold = 50f;
        if (Vector2.Distance(rectTransform.anchoredPosition, originalPos) < returnThreshold)
        {
            ReturnToOriginalSlot();
            Debug.Log("초기 위치 근처로 드롭됨 → 원래 위치로 복귀");
            return;
        }

        Vector2 mouseScreenPos = Input.mousePosition;

        // 영역 확인
        bool inA = IsInsideCircle(areaA, mouseScreenPos);
        bool inB = IsInsideCircle(areaB, mouseScreenPos);

        DropArea dropArea = DropArea.None;
        RectTransform targetArea = null;

        if (inA && inB) { dropArea = DropArea.Intersect; targetArea = intersectArea; }
        else if (inA) { dropArea = DropArea.AOnly; targetArea = areaA; }
        else if (inB) { dropArea = DropArea.BOnly; targetArea = areaB; }

        // 드래그 전 영역 정보 임시 저장
        DropArea previousArea = currentArea;

        if (dropArea == DropArea.None)
        {
            ReturnToOriginalSlot();
            Debug.Log("어느 영역에도 없음 → 되돌림");
        }
        else
        {
            bool placed = TryPlaceInArea(targetArea, dropArea);
            if (!placed)
            {
                ReturnToOriginalSlot();
                Debug.Log("슬롯이 가득 참 → 되돌림");
            }
            else
            {
                Debug.Log($"[{dropArea}] 위치에 드롭됨 → 슬롯에 배치됨");

                // 이전 영역에서 객체 제거 (새 위치에 배치 성공 후)
                if (previousArea != DropArea.None && previousArea != dropArea)
                {
                    areaObjectMap[previousArea].Remove(gameObject.name);
                    Debug.Log($"[{previousArea}] 영역에서 {gameObject.name} 제거됨");
                }

                RearrangeSlots(targetArea, dropArea);
                CheckAnswer();
                LogAreaObjectMap(); // 상태 로깅
            }
        }
    }

    // 원래 슬롯으로 돌아가는 메서드
    private void ReturnToOriginalSlot()
    {
        rectTransform.anchoredPosition = originalPos;
    }

    // 특정 영역의 빈 슬롯에 이 오브젝트 배치
    private bool TryPlaceInArea(RectTransform area, DropArea dropArea)
    {
        for (int i = 0; i < area.childCount; i++)
        {
            Transform slot = area.GetChild(i);
            if (slot.childCount == 0)
            {
                transform.SetParent(slot);
                rectTransform.anchoredPosition = Vector2.zero;

                // 현재 영역 업데이트
                currentArea = dropArea;

                // 배치 후, 해당 영역에 오브젝트를 추가
                if (!areaObjectMap[dropArea].Contains(gameObject.name))
                {
                    areaObjectMap[dropArea].Add(gameObject.name);
                }
                return true;
            }
        }
        return false;
    }

    // 슬롯에서 빈칸 없이 정렬 (슬롯을 왼쪽부터 채우기)
    private void RearrangeSlots(RectTransform area, DropArea dropArea)
    {
        List<Transform> objects = new List<Transform>();

        // 오브젝트 수집
        for (int i = 0; i < area.childCount; i++)
        {
            Transform slot = area.GetChild(i);
            if (slot.childCount > 0)
                objects.Add(slot.GetChild(0));
        }

        // Dictionary 초기화 (해당 영역의 모든 객체 다시 추적)
        areaObjectMap[dropArea].Clear();

        // 기존 오브젝트 제거 및 임시 저장
        List<Transform> tempParents = new List<Transform>();
        foreach (Transform obj in objects)
        {
            tempParents.Add(obj.parent);
            obj.SetParent(null);
        }

        // 앞쪽 슬롯부터 재배치
        for (int i = 0; i < objects.Count && i < area.childCount; i++)
        {
            Transform obj = objects[i];
            obj.SetParent(area.GetChild(i));
            obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // Dictionary 업데이트
            UIDraggable draggable = obj.GetComponent<UIDraggable>();
            if (draggable != null)
            {
                draggable.currentArea = dropArea;
                if (!areaObjectMap[dropArea].Contains(obj.name))
                {
                    areaObjectMap[dropArea].Add(obj.name);
                }
            }
        }
    }

    // 상태 로깅 메서드
    private void LogAreaObjectMap()
    {
        // 전체 상태 확인
        foreach (var area in areaObjectMap)
        {
            Debug.Log($"{area.Key}: {string.Join(", ", area.Value)}");
        }
    }

    // 원형 영역 안에 있는지 판단
    private bool IsInsideCircle(RectTransform area, Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            area, screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint);

        float radius = area.rect.width * 0.5f;
        return localPoint.magnitude < radius;
    }

    // 현재 영역 상태 확인 메서드 (디버깅용)
    public string GetCurrentAreaName()
    {
        switch (currentArea)
        {
            case DropArea.AOnly: return "A만";
            case DropArea.BOnly: return "B만";
            case DropArea.Intersect: return "교집합";
            default: return "없음";
        }
    }

    // 모든 영역의 객체 상태 초기화 (에디터 또는 게임 시작 시 호출)
    public static void ResetAllAreaObjects()
    {
        if (areaObjectMap != null)
        {
            foreach (var key in areaObjectMap.Keys)
            {
                areaObjectMap[key].Clear();
            }
            Debug.Log("모든 영역 객체 정보 초기화됨");
        }
    }
    public bool CheckAnswer()
    {
        bool isEqual = true;

        foreach (DropArea area in answerMap.Keys)
        {
            // 각 영역에 대해 areaObjectMap과 answerMap 비교
            if (!areaObjectMap.ContainsKey(area) ||
                areaObjectMap[area].Count != answerMap[area].Count)
            {
                isEqual = false;
                break;
            }

            // 각 항목이 모두 포함되어 있는지 확인 (순서는 무시)
            foreach (string item in answerMap[area])
            {
                if (!areaObjectMap[area].Contains(item))
                {
                    isEqual = false;
                    break;
                }
            }

            if (!isEqual) break;
        }

        // 디버그 로그 출력
        Debug.Log($"맵 비교 결과: {isEqual}");

        if (isEqual)
        {
            GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][GameManager.Instance.levelSelected] = new Tuple<int, bool>(GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][GameManager.Instance.levelSelected].Item1, true);
            correctImg.transform.DOScale(2.25f, 1.0f).SetEase(Ease.OutBack);
        }

        return isEqual;
    }
    public void menu()
    {
        Panel.DOFade(0.0f, 0.75f).OnComplete(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene"); // 메인 메뉴 씬 로드
        });
    }
}