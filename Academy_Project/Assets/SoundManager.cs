using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioSource uiSoundSource;
    public AudioClip buttonClickClip;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float uiSoundVolume = 1f;

    private List<Button> registeredButtons = new List<Button>();
    private bool isAudioInitialized = false;
    private Queue<AudioClip> soundQueue = new Queue<AudioClip>();
    private bool isPlayingQueuedSound = false;

    // 버튼 클릭 시마다 오디오 초기화 확인 (더 안전한 방법)
    private void EnsureAudioInitialized()
    {
        if (!isAudioInitialized)
        {
            StartCoroutine(InitializeAudioOnDemand());
        }
    }

    private IEnumerator InitializeAudioOnDemand()
    {
        if (uiSoundSource != null && !isAudioInitialized)
        {
            // 버튼 클릭 시점에서 오디오 활성화
            uiSoundSource.volume = 0.01f; // 거의 들리지 않는 볼륨으로 먼저 재생
            uiSoundSource.clip = buttonClickClip;
            uiSoundSource.Play();

            yield return new WaitForSeconds(0.1f);

            uiSoundSource.Stop();
            uiSoundSource.volume = uiSoundVolume;
            isAudioInitialized = true;

            Debug.Log("버튼 클릭 시점에서 모바일 오디오 시스템 활성화 완료");

            // 이제 실제 사운드 재생
            StartCoroutine(PlaySoundCoroutine());
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSource()
    {
        StartCoroutine(Yield());
    }

    IEnumerator Yield()
    {
        yield return new WaitForSeconds(0.2f);
        uiSoundSource.volume = 1.0f;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 후 약간의 지연을 두고 버튼 검색
        StartCoroutine(DelayedButtonSearch());
    }

    private IEnumerator DelayedButtonSearch()
    {
        yield return new WaitForSeconds(0.1f);
        SearchAndAddButtonSound();
    }

    private void SearchAndAddButtonSound()
    {
        // 기존 리스너 제거
        foreach (Button btn in registeredButtons)
        {
            if (btn != null)
                btn.onClick.RemoveListener(PlayButtonClickSound);
        }
        registeredButtons.Clear();

        // 모든 버튼 찾기 (비활성화된 것들도 포함)
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button button in allButtons)
        {
            button.onClick.RemoveListener(PlayButtonClickSound);
            button.onClick.AddListener(PlayButtonClickSound);
            registeredButtons.Add(button);
        }

        Debug.Log($"총 {allButtons.Length}개의 버튼에 사운드 리스너 추가");
    }

    // 모바일에 최적화된 버튼 클릭 사운드 재생 (오디오 초기화 포함)
    private void PlayButtonClickSound()
    {
        if (uiSoundSource != null && buttonClickClip != null)
        {
            // 오디오가 초기화되지 않았다면 먼저 초기화
            if (!isAudioInitialized)
            {
                EnsureAudioInitialized();
                return; // 초기화 완료 후 자동으로 사운드 재생됨
            }

            StartCoroutine(PlaySoundCoroutine());
        }
        else
        {
            Debug.LogWarning("UI 사운드 소스 또는 오디오 클립이 할당되지 않았습니다.");
        }
    }

    private IEnumerator PlaySoundCoroutine()
    {
        // 이미 재생 중이면 즉시 정지하고 새로운 사운드 재생
        if (isPlayingQueuedSound)
        {
            uiSoundSource.Stop();
        }

        isPlayingQueuedSound = true;

        // UI 사운드 바로 재생
        uiSoundSource.clip = buttonClickClip;
        uiSoundSource.volume = uiSoundVolume;
        uiSoundSource.time = 0f;

        // 모바일에서 확실히 재생되도록 강제 설정
        uiSoundSource.enabled = true;
        uiSoundSource.gameObject.SetActive(true);

        uiSoundSource.Play();

        Debug.Log($"UI 버튼 사운드 재생 - Volume: {uiSoundSource.volume}, IsPlaying: {uiSoundSource.isPlaying}, Clip: {buttonClickClip.name}");

        // 클립 재생 완료까지 대기
        yield return new WaitForSeconds(buttonClickClip.length + 0.1f);

        isPlayingQueuedSound = false;
    }

    // PlayOneShot을 사용하는 더 간단한 대안 (모바일 최적화)
    public void PlayButtonClickSoundSimple()
    {
        if (uiSoundSource != null && buttonClickClip != null && isAudioInitialized)
        {
            // 강제로 오디오 소스 활성화
            uiSoundSource.enabled = true;
            uiSoundSource.gameObject.SetActive(true);

            // PlayOneShot으로 간단하게 재생
            uiSoundSource.PlayOneShot(buttonClickClip, uiSoundVolume);

            Debug.Log($"간단 재생 - Volume: {uiSoundVolume}, 클립: {buttonClickClip.name}");
        }
        else
        {
            Debug.LogWarning($"재생 실패 - AudioSource: {uiSoundSource != null}, Clip: {buttonClickClip != null}, Initialized: {isAudioInitialized}");
        }
    }

    // // 오디오 시스템 강제 초기화 (디버그용)
    // [ContextMenu("Force Initialize Audio")]
    // public void ForceInitializeAudio()
    // {
    //     StartCoroutine(InitializeAudioOnFirstInteraction());
    // }

    // 테스트용 메서드
    [ContextMenu("Test UI Sound")]
    public void TestUISound()
    {
        PlayButtonClickSound();
    }

    // 오디오 상태 체크 (디버그용)
    [ContextMenu("Check Audio Status")]
    public void CheckAudioStatus()
    {
        Debug.Log($"Audio Initialized: {isAudioInitialized}");
        Debug.Log($"UI Source Enabled: {uiSoundSource != null && uiSoundSource.enabled}");
        Debug.Log($"UI Source Volume: {(uiSoundSource != null ? uiSoundSource.volume : 0)}");
        Debug.Log($"Button Clip Available: {buttonClickClip != null}");
        Debug.Log($"Registered Buttons: {registeredButtons.Count}");
    }
}