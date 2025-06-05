using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;
using System;

public class SentenceManager2 : MonoBehaviour
{
    Stack<GameObject> stack = new Stack<GameObject>();
    [SerializeField] private Transform wordsParent; // 단어 박스들의 부모 Transform
    [SerializeField] private float spacingX = 10f; // 단어 박스 간의 세로 간격
    [SerializeField] private float spacingY = 10f; // 단어 박스 간의 세로 간격
    private List<GameObject> displayedWordBoxes = new List<GameObject>(); // 현재 배치된 단어 박스들을 관리하는 리스트
    private List<GameObject> wordList = new List<GameObject>(); // 현재 배치된 단어 박스들을 관리하는 리스트
    List<Vector3> originalPosition = new List<Vector3>();
    [SerializeField] GameObject Words;
    [SerializeField] GameObject Sentence;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform parentObjectToSort; // 정렬할 자식 오브젝트들을 가진 부모 오브젝트
    List<int> numbers = new List<int>(); // 문제를 푼 답 
    List<int> realAnswerNumbers = new List<int>() { 5,3,1,0,7,4,10,11,9,6,8,2};// 진짜 답 
    [SerializeField] GameObject Star;
    [SerializeField] Image Panel;

    private void OnEnable()
    {
        Panel.DOFade(0.0f, 0.75f);
    }

    void Start()
    {
        for (int i = 0; i < Words.transform.childCount; i++)
        {
            originalPosition.Add(Words.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
            Debug.Log(Words.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
        }
    }


    // 이 함수를 버튼의 OnClick() 이벤트에 연결하세요.
    public void PutWord()
    {
        GameObject selectedWordBox = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (selectedWordBox != null)
        {
            stack.Push(selectedWordBox);
            Debug.Log("Pushed " + selectedWordBox.name + " to stack. Stack count: " + stack.Count);
            RefreshDisplay();
        }
    }

    private void RefreshDisplay()
    {
        // 기존에 배치된 단어 박스들의 위치를 업데이트
        UpdateDisplayedWordPositions();
        if (Sentence.transform.childCount == 12) LogSortedChildNumbers();
    }

    private void UpdateDisplayedWordPositions()
    {
        // 스택의 내용을 리스트로 복사 (순서 유지를 위해)
        List<GameObject> currentStackWords = new List<GameObject>(stack);

        // 기존에 배치된 단어 박스들을 관리
        for (int i = 0; i < currentStackWords.Count; i++)
        {
            GameObject wordBox = currentStackWords[i];
            if (!displayedWordBoxes.Contains(wordBox))
            {
                // 새로 스택에 들어온 단어 박스 처리
                displayedWordBoxes.Add(wordBox);
                wordBox.transform.SetParent(wordsParent, true); // 부모 설정
            }

            // 위치 업데이트
            RectTransform wordRect = wordBox.GetComponent<RectTransform>();
            if (wordRect != null)
            {
                // 아래에서 위로 쌓이도록 y 위치 계산
                float yPosition = i * (wordRect.rect.height + spacingY);
                wordRect.localPosition = new Vector3(wordRect.localPosition.x, yPosition, 0f);
            }
        }

        // 스택에서 제거된 단어 박스는 displayedWordBoxes에서도 제거
        displayedWordBoxes.RemoveAll(wordBox => !currentStackWords.Contains(wordBox));
    }

    public void ClearDisplayedWords()
    {
        List<Transform> childrenToMove = new List<Transform>();
        for (int i = 0; i < Sentence.transform.childCount; i++)
        {
            childrenToMove.Add(Sentence.transform.GetChild(i));
        }

        foreach (Transform child in childrenToMove)
        {
            child.SetParent(Words.transform);
            RectTransform rt = child.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(337.1f, 118.9f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
        }

        SortChildrenByNameByNumber();
        for (int i = 0; i < Words.transform.childCount; i++)
        {
            Words.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = originalPosition[i];
        }

        // displayedWordBoxes 리스트를 초기화합니다.
        displayedWordBoxes.Clear();
        // 스택도 초기화하는 것이 논리적으로 맞을 수 있습니다.
        stack.Clear();
    }

    private void DisplayWords()
    {
        if (wordsParent == null)
        {
            Debug.LogError("wordsParent가 설정되지 않았습니다.");
            return;
        }

        // 기존 자식 오브젝트가 있다면 모두 삭제 (ClearDisplayedWords()와 동일 기능)
        foreach (Transform child in wordsParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < wordList.Count; i++)
        {
            GameObject wordObject = wordList[i]; // 스택에서 가져온 실제 단어 오브젝트

            // wordsParent 아래에 복제본을 생성하여 배치합니다.
            GameObject displayedWord = Instantiate(wordObject, wordsParent);
            displayedWord.SetActive(true); // 혹시 비활성화되어 있다면 활성화

            RectTransform wordRect = displayedWord.GetComponent<RectTransform>();
            if (wordRect != null)
            {
                int row = i / 3;
                int col = i % 3;

                float xPosition = col * (wordRect.rect.width + spacingX);
                float yPosition = -row * (wordRect.rect.height + spacingY);

                wordRect.localPosition = new Vector3(xPosition, yPosition, 0f);
            }
        }
    }
    public void SortChildrenByNameByNumber()
    {
        if (parentObjectToSort == null)
        {
            Debug.LogError("정렬할 부모 오브젝트가 설정되지 않았습니다.");
            return;
        }

        // 자식 오브젝트들을 리스트로 가져옵니다.
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parentObjectToSort)
        {
            children.Add(child);
        }

        // 오브젝트 이름에서 숫자를 파싱하여 정렬합니다.
        children = children.OrderBy(child =>
        {
            string name = child.name;
            string numberPart = new string(name.Where(char.IsDigit).ToArray());

            if (int.TryParse(numberPart, out int number))
            {
                return number;
            }
            else
            {
                // 숫자가 없거나 파싱에 실패한 경우, 원래 이름 순서를 유지하거나 다른 기준으로 정렬할 수 있습니다.
                // 여기서는 최대 정수값을 반환하여 숫자가 있는 오브젝트 뒤에 정렬되도록 합니다.
                return int.MaxValue;
            }
        }).ToList();

        // 정렬된 순서대로 자식들의 순서를 재설정합니다.
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }

        Debug.Log("자식 오브젝트들을 이름의 숫자 순서대로 정렬했습니다.");
    }


    public List<int> GetChildNumbersInOrder()
    {
        if (Sentence == null)
        {
            Debug.LogError("sentences 부모 오브젝트가 설정되지 않았습니다.");
            return new List<int>(); // 빈 리스트 반환
        }

        List<KeyValuePair<int, string>> numberedObjects = new List<KeyValuePair<int, string>>();

        // 자식 오브젝트들을 순회하며 이름에서 숫자를 파싱합니다.
        for (int i = 0; i < Sentence.transform.childCount; i++)
        {
            string name = Sentence.transform.GetChild(i).name;
            string numberPart = new string(name.Where(char.IsDigit).ToArray());

            if (int.TryParse(numberPart, out int number))
            {
                numberedObjects.Add(new KeyValuePair<int, string>(number, name));
            }
            else
            {
                // 숫자가 없거나 파싱에 실패한 경우, 0 또는 다른 기본값을 사용할 수 있습니다.
                // 여기서는 0으로 처리합니다. 필요에 따라 다른 로직을 구현하세요.
                numberedObjects.Add(new KeyValuePair<int, string>(0, name));
            }
        }

        // // 파싱한 숫자를 기준으로 리스트를 정렬합니다.
        // numberedObjects = numberedObjects.OrderBy(pair => pair.Key).ToList();
        // Debug.Log(numberedObjects);
        // 정렬된 순서대로 숫자만 추출하여 결과 리스트를 만듭니다.
        List<int> sortedNumbers = numberedObjects.Select(pair => pair.Key).ToList();

        return sortedNumbers;
    }

    // 사용 예시 (버튼 클릭 등으로 호출할 수 있습니다.)
    public void LogSortedChildNumbers()
    {
        numbers = GetChildNumbersInOrder();
        if (numbers.Count == 12)
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                Debug.Log(numbers[i] + "  " + realAnswerNumbers[i]);
                if (numbers[i] != realAnswerNumbers[i])
                {
                    Debug.Log("틀림");
                    return;
                }
            }
            Debug.Log("맞음");
            GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][GameManager.Instance.levelSelected] = new Tuple<int, bool>(GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][GameManager.Instance.levelSelected].Item1, true);
            Star.transform.DOScale(2.25f, 1.0f).SetEase(Ease.OutBack);
        }
        return;
    }

    public void SceneManage()
    {
        Panel.DOFade(1.0f, 0.75f).OnComplete(() =>
        {
            SceneManager.LoadScene("LevelScene");
        });
    }
}