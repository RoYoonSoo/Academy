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
    private bool isFirstInteraction = false;

    private void Update()
    {
        if (!isFirstInteraction && Input.GetMouseButtonDown(0))
        {
            ActivateAudioOnMobile();
        }
    }

    private void ActivateAudioOnMobile()
    {
        if (uiSoundSource != null)
        {
            // 더 확실한 오디오 활성화 방법
            uiSoundSource.volume = 0f;
            uiSoundSource.PlayOneShot(AudioClip.Create("silence", 1, 1, 44100, false));
            uiSoundSource.volume = uiSoundVolume;
            isFirstInteraction = true;
            Debug.Log("첫 터치로 오디오 시스템 활성화됨");
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
        if (uiSoundSource == null)
        {
            uiSoundSource = gameObject.AddComponent<AudioSource>();
        }
        
        // UI 사운드 전용 AudioSource 설정
        uiSoundSource.playOnAwake = false;
        uiSoundSource.loop = false;
        uiSoundSource.volume = uiSoundVolume;
        uiSoundSource.priority = 0; // 최고 우선순위
        uiSoundSource.spatialBlend = 0f; // 2D 사운드
        
        Debug.Log($"UI AudioSource 초기화 완료 - Volume: {uiSoundSource.volume}");
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
        SearchAndAddButtonSound();
    }

    private void SearchAndAddButtonSound()
    {
        foreach (Button btn in registeredButtons)
        {
            if (btn != null)
                btn.onClick.RemoveListener(PlayButtonClickSound);
        }
        registeredButtons.Clear();

        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button button in allButtons)
        {
            button.onClick.RemoveListener(PlayButtonClickSound);
            button.onClick.AddListener(PlayButtonClickSound);
            registeredButtons.Add(button);
        }

        Debug.Log($"총 {allButtons.Length}개의 버튼에 사운드 리스너 추가");
    }

    // 개선된 버튼 클릭 사운드 재생 메서드
private void PlayButtonClickSound()
{
    if (uiSoundSource != null && buttonClickClip != null)
    {
        // 볼륨 설정을 uiSoundSource에 미리 해두었으므로, PlayOneShot은 클립만 받거나 볼륨을 명시적으로 전달
        // uiSoundSource.volume = uiSoundVolume; // 이 줄은 InitializeAudioSource에서 이미 설정되므로 중복될 수 있음.
        uiSoundSource.PlayOneShot(buttonClickClip); // uiSoundSource의 현재 볼륨으로 재생

        Debug.Log($"UI 버튼 사운드 PlayOneShot 직접 호출 - 클립: {buttonClickClip.name}");
    }
    else
    {
        Debug.LogWarning("UI 사운드 소스 또는 오디오 클립이 할당되지 않았습니다.");
    }
}
    
    // 코루틴을 사용한 안정적인 UI 사운드 재생
    private IEnumerator PlayUISoundCoroutine()
    {
        // 현재 재생 중인 클립 백업
        AudioClip previousClip = uiSoundSource.clip;
        bool wasPlaying = uiSoundSource.isPlaying;
        
        // UI 사운드 재생
        uiSoundSource.clip = buttonClickClip;
        uiSoundSource.volume = uiSoundVolume;
        uiSoundSource.Play();
        
        Debug.Log($"UI 버튼 사운드 재생 시작 - Volume: {uiSoundSource.volume}, IsPlaying: {uiSoundSource.isPlaying}");
        
        // 클립이 끝날 때까지 대기
        yield return new WaitForSeconds(buttonClickClip.length);
        
        // 이전 상태 복원 (필요한 경우)
        uiSoundSource.clip = previousClip;
        if (wasPlaying && previousClip != null)
        {
            uiSoundSource.Play();
        }
    }
    
    // 대안: PlayOneShot을 더 안정적으로 사용하는 방법
    public void PlayButtonClickSoundAlternative()
    {
        if (uiSoundSource != null && buttonClickClip != null)
        {
            // 볼륨을 명시적으로 설정하고 PlayOneShot 사용
            float originalVolume = uiSoundSource.volume;
            uiSoundSource.volume = uiSoundVolume;
            
            uiSoundSource.PlayOneShot(buttonClickClip, uiSoundVolume);
            
            Debug.Log($"PlayOneShot 호출 - Volume: {uiSoundVolume}, AudioSource Volume: {uiSoundSource.volume}");
            
            // 원래 볼륨 복원
            uiSoundSource.volume = originalVolume;
        }
    }
    
    // 테스트용 메서드 - 에디터나 디버그에서 직접 호출 가능
    [ContextMenu("Test UI Sound")]
    public void TestUISound()
    {
        PlayButtonClickSound();
    }
}