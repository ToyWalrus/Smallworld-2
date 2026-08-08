using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class RegionText : MonoBehaviour
{
    private TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void SetText(string message)
    {
        text.text = message;
    }
}
