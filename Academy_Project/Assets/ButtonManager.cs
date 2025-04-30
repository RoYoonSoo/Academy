using UnityEngine;
using DG.Tweening;
public class ButtonManager : MonoBehaviour
{
    public CanvasGroup targetCanvasGroup;
    public float tweenDuration = 0.5f;
    [SerializeField] private GameObject selectGamePanel;

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
}
