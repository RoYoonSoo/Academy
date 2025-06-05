using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } // 어디서든 접근 가능하도록 static Instance 선언
    [Header("Audio Settings")]
    public AudioSource uiSoundSource;   // UI 사운드를 재생할 AudioSource 컴포넌트
    public AudioClip buttonClickClip;   // 버튼 클릭 시 재생할 오디오 클립
    private List<Button> registeredButtons = new List<Button>(); // 등록된 버튼 목록
    private void Awake()
    {
        // 싱글톤 인스턴스 확인
        if (Instance == null)
        {
            Instance = this; // 인스턴스가 없으면 이 GameManager를 Instance로 설정
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
            InitializeAudioSource(); // AudioSource 초기화
        }
        else
        {
            // 이미 다른 GameManager 인스턴스가 존재하면 현재 게임 오브젝트 파괴
            Destroy(gameObject);
        }
    }
     private void InitializeAudioSource()
    {
        // uiSoundSource가 할당되지 않았다면 GameManager GameObject에 추가
        if (uiSoundSource == null)
        {
            uiSoundSource = gameObject.AddComponent<AudioSource>();
            uiSoundSource.playOnAwake = false; // 시작 시 자동 재생 안 함
            uiSoundSource.loop = false; // 반복 재생 안 함
        }
    }

    // 이 메서드는 GameManager 인스턴스가 활성화될 때 한 번 호출됩니다.
    // 씬 로드 이벤트를 구독합니다.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트에 OnSceneLoaded 메서드 연결
    }

    // 이 메서드는 GameManager 인스턴스가 비활성화되거나 파괴될 때 호출됩니다.
    // 씬 로드 이벤트를 구독 해제하여 메모리 누수를 방지합니다.
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 연결 해제
    }

    // 씬이 로드될 때마다 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SearchAndAddButtonSound();
    }

    // 모든 버튼을 찾아서 클릭 사운드 리스너를 추가하는 메서드
    private void SearchAndAddButtonSound()
    {
        // 기존에 등록된 버튼의 리스너를 정리합니다.
        // 이렇게 하지 않으면 씬 전환 시 동일 버튼에 리스너가 중복으로 추가될 수 있습니다.
        foreach(Button btn in registeredButtons)
        {
            btn.onClick.RemoveListener(PlayButtonClickSound);
        }
        registeredButtons.Clear(); // 리스트 비우기

        Button[] allButtons = FindObjectsOfType<Button>(true); 

        foreach (Button button in allButtons)
        {
            // 이미 이전에 추가되었을 수 있으므로 중복 추가 방지를 위해 한 번 더 제거.
            // AddListener는 동일한 리스너가 추가되는 것을 자동으로 막아주지만, 명시적으로 제거하는게 더 확실합니다.
            button.onClick.RemoveListener(PlayButtonClickSound); 
            
            // 버튼 클릭 이벤트에 사운드 재생 메서드를 연결합니다.
            button.onClick.AddListener(PlayButtonClickSound);
            registeredButtons.Add(button); // 등록된 버튼 목록에 추가
        }
        Debug.Log($"총 {allButtons.Length}개의 버튼을 찾았고, {registeredButtons.Count}개에 사운드 리스너를 추가했습니다.");
    }

    // 버튼 클릭 시 호출될 사운드 재생 메서드
    private void PlayButtonClickSound()
    {
        if (uiSoundSource != null && buttonClickClip != null)
        {
            uiSoundSource.PlayOneShot(buttonClickClip); // 클립을 한 번 재생
        }
        else
        {
            // Debug.LogWarning("UI 사운드 소스 또는 오디오 클립이 할당되지 않았습니다.");
        }
    }
}