using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LevelScene : MonoBehaviour
{
    public Sprite[] levelImages;
    public string[] levelSceneNames;
    public Button[] buttons = new Button[4];
    public TextMeshProUGUI text;
    public Image panel; // Change from Sprite to Image to allow DOFade
    bool clicked1 = false;
    bool clicked2 = false;
    bool clicked3 = false;

    bool isChanging = false; // 레벨 버튼이 변경 중인지 여부를 나타내는 변수
    bool isChanging2 = false; // 레벨 버튼이 변경 중인지 여부를 나타내는 변수
    bool isChanging3 = false; // 레벨 버튼이 변경 중인지 여부를 나타내는 변수

    void Awake()
    {
        panel.DOFade(0.0f, 0.75f);
    }
    void OnEnable()
    {
        // 초기화 작업이 필요하다면 여기에 작성
        for (int i = 0; i < 3; i++)
        {
            if (GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][i].Item2 == true)
            {
                buttons[i + 1].GetComponent<Image>().sprite = levelImages[0]; // 레벨 완료 시 버튼 이미지 변경
            }
            else
            {
                buttons[i + 1].GetComponent<Image>().sprite = levelImages[1]; // 레벨 1 완료 시 버튼 이미지 변경
            }
        }
    }
    void Start()
    {
        switch (GameManager.Instance.currentObject)
        {
            case 0:
                text.text = "Plant Cell";
                break;
            case 1:
                text.text = "Animal Cell";
                break;
            case 2:
                text.text = "Blood Cell";
                break;
            default:
                text.text = "Blood Cell";
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void level1Clicked()
    {
        GameManager.Instance.levelSelected = 0; // 선택된 레벨을 0으로 설정
        clicked1 = !clicked1;
        // buttons[0].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[0].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[1].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[1].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[2].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[2].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);


        if (clicked1 == true && isChanging == false)
        {
            buttons[0].transform.GetChild(1).gameObject.GetComponent<Image>().enabled = clicked1;
            buttons[0].transform.GetChild(2).gameObject.GetComponent<Image>().enabled = clicked1;
            isChanging = true;
            buttons[0].transform.GetChild(1).gameObject.transform.DOScale(new Vector3(1.31f, 1.2f, 1), 1.0f);
            buttons[0].transform.GetChild(2).gameObject.transform.DOScale(new Vector3(1.31f, 1.2f, 1), 1.0f).OnComplete(() =>
            {
                isChanging = false;
            });
        }
        else if (clicked1 == false && isChanging == false)
        {
            buttons[0].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[0].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[1].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[1].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[2].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[2].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        }
    }
    public void level2Clicked()
    {
        if (GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][0].Item2 == false)
        {
            Debug.Log("레벨 1을 먼저 완료해야 합니다.");
            return; // 레벨 1이 완료되지 않은 경우 레벨 2로 이동하지 않음
        }
        GameManager.Instance.levelSelected = 1; // 선택된 레벨을 0으로 설정
        clicked2 = !clicked2;

        // buttons[0].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[0].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[1].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[1].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[2].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[2].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);

        if (clicked2 == true && isChanging2 == false)
        {
            buttons[1].transform.GetChild(1).gameObject.GetComponent<Image>().enabled = clicked2;
            buttons[1].transform.GetChild(2).gameObject.GetComponent<Image>().enabled = clicked2;
            isChanging2 = true;
            buttons[1].transform.GetChild(1).gameObject.transform.DOScale(new Vector3(-1.31f, 1.2f, 1), 1.0f);
            buttons[1].transform.GetChild(2).gameObject.transform.DOScale(new Vector3(-1.31f, 1.2f, 1), 1.0f).OnComplete(() =>
            {
                isChanging2 = false;
            });
        }
        else if (clicked2 == false && isChanging2 == false)
        {
            buttons[0].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[0].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[1].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[1].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[2].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[2].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        }
    }
    public void level3Clicked()
    {
        if (GameManager.Instance.finishedDictionary[GameManager.Instance.currentObject][1].Item2 == false)
        {
            Debug.Log("레벨 2를 먼저 완료해야 합니다.");
            return; // 레벨 2가 완료되지 않은 경우 레벨 3로 이동하지 않음
        }
        GameManager.Instance.levelSelected = 2; // 선택된 레벨을 0으로 설정
        clicked3 = !clicked3;

        // buttons[0].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[0].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[1].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[1].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[2].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        // buttons[2].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);


        if (clicked3 == true && isChanging3 == false)
        {
            buttons[2].transform.GetChild(1).gameObject.GetComponent<Image>().enabled = clicked3;
            buttons[2].transform.GetChild(2).gameObject.GetComponent<Image>().enabled = clicked3;
            isChanging3 = true;
            buttons[2].transform.GetChild(1).gameObject.transform.DOScale(new Vector3(1.31f, 1.2f, 1), 1.0f);
            buttons[2].transform.GetChild(2).gameObject.transform.DOScale(new Vector3(1.31f, 1.2f, 1), 1.0f).OnComplete(() =>
            {
                isChanging3 = false;
            });
        }
        else if (clicked3 == false && isChanging3 == false)
        {
            buttons[0].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[0].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[1].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[1].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[2].transform.GetChild(1).gameObject.transform.DOScale(Vector3.zero, 0.0f);
            buttons[2].transform.GetChild(2).gameObject.transform.DOScale(Vector3.zero, 0.0f);
        }
    }

    public void level1SceneChange()
    {
        GameManager.Instance.levelSelected = 0;

        panel.DOFade(1.0f, 0.75f).OnComplete(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelSceneNames[3 * GameManager.Instance.currentObject + GameManager.Instance.levelSelected]); // 첫 번째 레벨 씬 로드
        });
    }

    public void level2SceneChange()
    {
        GameManager.Instance.levelSelected = 1;

        panel.DOFade(1.0f, 0.75f).OnComplete(() =>
{
    UnityEngine.SceneManagement.SceneManager.LoadScene(levelSceneNames[3 * GameManager.Instance.currentObject + GameManager.Instance.levelSelected]); // 첫 번째 레벨 씬 로드

});
    }
    public void level3SceneChange()
    {
        GameManager.Instance.levelSelected = 2;
        panel.DOFade(1.0f, 0.75f).OnComplete(() =>
{
    UnityEngine.SceneManagement.SceneManager.LoadScene(levelSceneNames[3 * GameManager.Instance.currentObject + GameManager.Instance.levelSelected]); // 첫 번째 레벨 씬 로드

});
    }

    public void backScene()
    {
        panel.DOFade(1.0f, 0.75f).OnComplete(() =>
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("MiddleScene"); // 첫 번째 레벨 씬 로드
});
    }
}
