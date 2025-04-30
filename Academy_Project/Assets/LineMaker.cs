using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour
{
    public static ManagerScript Instance { get; private set; }
    public GameObject connectionPrefab; // UI Image prefab for connections
    public float fillSpeed = 2.0f; // Speed of fill animation
    
    private List<int> clickedNumbers = new List<int>();
    private Dictionary<int, int> answerDict = new Dictionary<int, int>()
    {
        { 1, 7 },
        { 2, 8 },
        { 3, 9 },
        { 4, 10 },
        { 5, 6 }
    };
    [SerializeField] Canvas canvas;
    private Dictionary<string, GameObject> connectionLines = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("ManagerScript 싱글톤이 이미 존재합니다.");
            Destroy(gameObject);
        }
    }

    public void OnToggleValueChanged(bool isOn, string name, GameObject gameObject)
    {
        Debug.Log("토글 상태 변경 (싱글톤): " + isOn + name);

        int clickedNumber = ExtractFirstNumber(name);
        if (clickedNumber != -1)
        {
            clickedNumbers.Add(clickedNumber);

            if (clickedNumbers.Count == 2)
            {
                int num1 = clickedNumbers[0];
                int num2 = clickedNumbers[1];

                // 5 이하인 숫자끼리 클릭하거나 5 초과인 숫자끼리 클릭한 경우 답 체크 안함
                if (!((num1 <= 5 && num2 <= 5) || (num1 > 5 && num2 > 5)))
                {
                    bool isCorrect = false;

                    if (answerDict.ContainsKey(num1) && answerDict[num1] == num2)
                    {
                        isCorrect = true;
                        answerDict.Remove(num1); // 정답을 맞춘 숫자는 딕셔너리에서 제거
                    }
                    else if (answerDict.ContainsKey(num2) && answerDict[num2] == num1)
                    {
                        isCorrect = true;
                        answerDict.Remove(num2); // 정답을 맞춘 숫자는 딕셔너리에서 제거
                    }

                    if (isCorrect)
                    {
                        Debug.Log("정답입니다. " + num1 + " and " + num2 + " are connected.");

                        GameObject obj1 = GameObject.Find("Image (" + num1 + ")");
                        GameObject obj2 = GameObject.Find("Image (" + num2 + ")");
                        if (obj1 != null && obj2 != null)
                        {
                            CreateConnectionLine(obj1.transform, obj2.transform, num1, num2);
                        }
                    }
                    else
                    {
                        Debug.Log("오답입니다. " + num1 + " and " + num2 + " are not connected.");
                    }
                }
                else
                {
                    Debug.Log("같은 그룹의 숫자를 클릭했습니다. 답 체크를 하지 않습니다.");
                }

                clickedNumbers.Clear(); // 클릭 기록 초기화
            }
        }
    }

    private void CreateConnectionLine(Transform obj1, Transform obj2, int num1, int num2)
    {
        // Create unique key for this connection
        string connectionKey = (num1 < num2) ? $"{num1}-{num2}" : $"{num2}-{num1}";
        
        // Check if this connection already exists
        if (connectionLines.ContainsKey(connectionKey))
        {
            Debug.Log("Connection already exists!");
            return;
        }
        
        // Instantiate the connection image prefab
        GameObject connectionObj = Instantiate(connectionPrefab, canvas.transform);
        RectTransform rectTransform = connectionObj.GetComponent<RectTransform>();
        Image connectionImage = connectionObj.GetComponent<Image>();
        
        // Get positions in canvas space
        Vector2 pos1 = obj1.transform.GetChild(1).position;
        Vector2 pos2 = obj2.transform.GetChild(1).position;
        
        // Calculate distance and angle
        float distance = Vector2.Distance(pos1, pos2);
        Vector2 direction = (pos2 - pos1).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Set position, rotation and scale
        rectTransform.position = pos1;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
        rectTransform.pivot = new Vector2(0, 0.5f);
        
        // Set initial fill amount to zero
        connectionImage.fillAmount = 0f;
        
        // Store the connection object
        connectionLines.Add(connectionKey, connectionObj);
        
        // Start fill animation
        StartCoroutine(FillConnectionLine(connectionImage));
    }

    private IEnumerator FillConnectionLine(Image connectionImage)
    {
        float fillAmount = 0f;
        
        while (fillAmount < 1f)
        {
            fillAmount += Time.deltaTime * fillSpeed;
            connectionImage.fillAmount = fillAmount;
            yield return null;
        }
        
        connectionImage.fillAmount = 1f;
    }

    public static int ExtractFirstNumber(string objectName)
    {
        foreach (char c in objectName)
        {
            if (char.IsDigit(c))
            {
                return int.Parse(c.ToString());
            }
        }
        return -1;
    }
}