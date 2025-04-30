using UnityEngine;
using UnityEngine.UI;

public class ToggleSelf : MonoBehaviour
{
    private Toggle myToggle;

    void Start()
    {
        myToggle = GetComponent<Toggle>();
        if (myToggle != null)
        {
            myToggle.onValueChanged.AddListener(isOn => OnMyToggleValueChanged(isOn, gameObject.name, gameObject));
        }
    }

    void OnMyToggleValueChanged(bool isOn, string name, GameObject gameObject)
    {
        if (ManagerScript.Instance != null)
        {
            ManagerScript.Instance.OnToggleValueChanged(isOn, name, gameObject);
        }
        else
        {
            Debug.LogError("ManagerScript Instance가 null입니다.");
        }
    }
}