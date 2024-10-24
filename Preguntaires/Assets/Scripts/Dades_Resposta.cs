using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dades_Resposta : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI infoTextObject;
    [SerializeField] Image toggle;

    [Header("Sprites")]
    [SerializeField] Sprite uncheckedToggle;
    [SerializeField] Sprite checkedToggle;

    [Header("References")]
    [SerializeField] GameEvents events;

    private RectTransform _rect;
    public RectTransform Rect
    {
        get 
        {
           if(_rect == null)
            {
                _rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            }
           return _rect;
        }
    }

    private int _answerIndex = -1;
    public int answerIndex { get { return _answerIndex; } }

    private bool Checked = false;

    public void UpdateDades(string info, int index)
    {
        infoTextObject.text = info;
        _answerIndex = index;
    }
    public void Reset()
    {
        Checked = true;
        UpdateUI();
    }
    public void SwitchState()
    {
        Checked = !Checked;
        UpdateUI();

        if(events.UpdateQuestionAnswer != null)
        {
            events.UpdateQuestionAnswer(this);
        }
    }
    void UpdateUI()
    {
        toggle.sprite = (Checked) ? checkedToggle : uncheckedToggle;
    }
}
