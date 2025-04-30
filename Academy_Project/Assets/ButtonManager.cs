using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ButtonManager : MonoBehaviour
{
    public CanvasGroup targetCanvasGroup;
    public float tweenDuration = 0.5f;
    [SerializeField] private GameObject selectGamePanel;
    public float moveAmount = 10f;     // 위아래로 움직이는 거리
    public float duration = 1f;        // 애니메이션 한 주기의 시간
    public LoopType loopType = LoopType.Yoyo; // 애니메이션 반복 방식 (Yoyo: 갔다가 되돌아옴)
    public Ease easeType = Ease.InOutSine; // 움직임의 부드러움 정도

    private Vector3 initialPosition;
    [SerializeField] Button[] butttons;
    public void openSelectGamePanel()
    {
        targetCanvasGroup.interactable = false;
        targetCanvasGroup.blocksRaycasts = true; // 필요에 따라 레이캐스트도 막음

        // 트위닝 애니메이션 시작 (DOTween 예시)
        selectGamePanel.transform.DOScale(1f, 0.75f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            targetCanvasGroup.interactable = true;
            targetCanvasGroup.blocksRaycasts = true;
        });
    }

    private void Start()
    {
        foreach (Button btn in butttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            initialPosition = rect.localPosition;

            // DOTween을 사용하여 위아래로 움직이는 애니메이션 생성 및 루프 설정
            rect.DOLocalMoveY(initialPosition.y + moveAmount, duration)
                .SetLoops(-1, loopType) // -1은 무한 루프
                .SetEase(easeType);
        }
    }
}
