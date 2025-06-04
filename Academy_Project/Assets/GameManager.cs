using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 어디서든 접근 가능하도록 static Instance 선언
    public int currentObject = -1; // 현재 레벨을 저장하는 변수
    public int levelSelected = 0; // 선택된 레벨을 저장하는 정적 변수
    public Dictionary<int, List<Tuple<int, bool>>> finishedDictionary = new Dictionary<int, List<Tuple<int, bool>>>(); // 레벨 완료 여부를 저장하는 딕셔너리
    private void Awake()
    {
        // 싱글톤 인스턴스 확인
        if (Instance == null)
        {
            Instance = this; // 인스턴스가 없으면 이 GameManager를 Instance로 설정
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else
        {
            // 이미 다른 GameManager 인스턴스가 존재하면 현재 게임 오브젝트 파괴
            Destroy(gameObject);
        }
        for (int i = 0; i < 3; i++)
        {
            finishedDictionary[i] = new List<Tuple<int, bool>>();
            for (int j = 0; j < 3; j++)
            {
                finishedDictionary[i].Add(new Tuple<int, bool>(j, false)); // 레벨 완료 여부 초기화
            }
        }
    }
}