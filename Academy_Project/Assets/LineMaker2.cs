using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using DG.Tweening;
using System;

public class LineManager2 : MonoBehaviour
{
    public Image linePrefab;
    public Transform canvas;
    public float lineDrawSpeed = 2f;
    public Dictionary<int, int> answerDict = new Dictionary<int, int>()
    {
        {1, 7}, {2, 6}, {3, 10}, {4, 8}, {5, 9},
        {7, 1}, {6, 2}, {10, 3}, {8, 4}, {9, 5}
    };

    private List<int> clickedNumbers = new List<int>();
    private bool firstClickIsLeft = false;
    [SerializeField] Image correctImg;
    [SerializeField] Image wrongImg;
    public int correctNum = 0;

    public void OnToggleValueChanged(bool isOn, string name, GameObject gameObject)
    {
        Debug.Log("토글 상태 변경: " + isOn + name);

        int clickedNumber = ExtractFirstNumber(name);
        if (clickedNumber != -1)
        {
            if (clickedNumbers.Count == 0) // 첫 번째 클릭
            {
                if (clickedNumber <= 5)
                {
                    firstClickIsLeft = true;
                    clickedNumbers.Add(clickedNumber);
                }
                else
                {
                    Debug.Log("첫 번째 클릭은 왼쪽 그룹 (5 이하)에서 이루어져야 합니다.");
                    Toggle toggle = gameObject.GetComponent<Toggle>();
                    if (toggle != null) toggle.isOn = false;
                    return;
                }
            }
            else if (clickedNumbers.Count == 1) // 두 번째 클릭
            {
                clickedNumbers.Add(clickedNumber);

                int num1 = clickedNumbers[0];
                int num2 = clickedNumbers[1];

                if (!((num1 <= 5 && num2 <= 5) || (num1 > 5 && num2 > 5)))
                {
                    bool isCorrect = false;
                    int correctKey = -1;

                    if (answerDict.ContainsKey(num1) && answerDict[num1] == num2)
                    {
                        isCorrect = true;
                        correctKey = num1;
                        answerDict.Remove(num1);
                    }
                    else if (answerDict.ContainsKey(num2) && answerDict[num2] == num1)
                    {
                        isCorrect = true;
                        correctKey = num2;
                        answerDict.Remove(num2);
                    }

                    if (isCorrect)
                    {
                        Debug.Log("정답입니다. " + num1 + " and " + num2 + " are connected.");
                        GameObject obj1 = GameObject.Find("Image (" + num1 + ")");
                        GameObject obj2 = GameObject.Find("Image (" + num2 + ")");
                        if (obj1 != null && obj2 != null)
                        {
                            correctImg.transform.DOScale(4.0f, 1.0f).SetEase(Ease.OutBack).OnComplete(() =>
                            {
                                correctImg.transform.DOScale(0.0f, 0.75f).OnComplete(() =>
                                {
                                    CreateConnectionLine(obj1.transform.GetChild(1), obj2.transform.GetChild(1), num1, num2);
                                });
                            });
                        }
                        obj1.GetComponent<Image>().color = Color.green;
                        obj2.GetComponent<Image>().color = Color.green;
                        correctNum++;
                        if (correctNum == 5)
                        {
                            Debug.Log("모든 연결이 완료되었습니다!");
                            GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][GameManager.Instance.levelSelected] = new Tuple<int, bool>(GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][GameManager.Instance.levelSelected].Item1, true);
                            correctImg.transform.DOScale(2.25f, 1.0f).SetEase(Ease.OutBack);
                        }
                    }
                    else
                    {
                        Debug.Log("오답입니다. " + num1 + " and " + num2 + " are not connected.");
                        wrongImg.transform.DOScale(4.0f, 1.0f).SetEase(Ease.OutBack).OnComplete(() =>
                        {
                            wrongImg.transform.DOScale(0.0f, 0.75f).OnComplete(() =>
                            {
                                Toggle toggle1 = GameObject.Find("Toggle (" + num1 + ")")?.GetComponent<Toggle>();
                                Toggle toggle2 = GameObject.Find("Toggle (" + num2 + ")")?.GetComponent<Toggle>();
                                if (toggle1 != null) toggle1.isOn = false;
                                if (toggle2 != null) toggle2.isOn = false;
                            });
                        });
                    }
                }
                else
                {
                    Debug.Log("같은 그룹의 숫자를 클릭했습니다. 답 체크를 하지 않습니다.");
                    Toggle toggle1 = GameObject.Find("Toggle (" + num1 + ")")?.GetComponent<Toggle>();
                    Toggle toggle2 = GameObject.Find("Toggle (" + num2 + ")")?.GetComponent<Toggle>();
                    if (toggle1 != null) toggle1.isOn = false;
                    if (toggle2 != null) toggle2.isOn = false;
                }

                clickedNumbers.Clear();
                firstClickIsLeft = false;
            }
        }
    }

    private int ExtractFirstNumber(string name) => new string(name.Where(char.IsDigit).ToArray()).Length > 0 && int.TryParse(new string(name.Where(char.IsDigit).ToArray()), out int number) ? number : -1;

    public void CreateConnectionLine(Transform start, Transform end, int num1, int num2)
    {
        Image lineObject = Instantiate(linePrefab, canvas);
        lineObject.transform.SetSiblingIndex(1);
        lineObject.type = Image.Type.Filled;
        lineObject.fillMethod = Image.FillMethod.Horizontal;
        lineObject.fillOrigin = 0; // Left
        lineObject.fillAmount = 0f;
        lineObject.color = Color.black;

        RectTransform lineRect = lineObject.GetComponent<RectTransform>();

        // 초기 위치 및 회전 설정
        lineRect.position = start.position;
        Vector3 direction = end.position - start.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);

        // 초기 길이 설정
        float distance = Vector3.Distance(start.position, end.position);
        lineRect.sizeDelta = new Vector2(distance, linePrefab.rectTransform.sizeDelta.y); // 프리팹의 초기 굵기 사용

        StartCoroutine(DrawLine(lineImage: lineObject, targetPosition: end.position, speed: lineDrawSpeed));
    }

    IEnumerator DrawLine(Image lineImage, Vector3 targetPosition, float speed)
    {
        float timer = 0f;
        float duration = 1f / speed; // fillAmount는 0에서 1로 변하므로 duration은 속도의 역수

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            lineImage.fillAmount = progress;
            yield return null;
        }
        lineImage.fillAmount = 1f;
    }
}