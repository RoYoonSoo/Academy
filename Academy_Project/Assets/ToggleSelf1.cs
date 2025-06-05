using UnityEngine;
using UnityEngine.UI;

public class ToggleSelf1 : MonoBehaviour
{
    private Toggle myToggle;
    [SerializeField] LineManager1 managerScript;
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
        managerScript.OnToggleValueChanged(isOn, name, gameObject);
    }
}