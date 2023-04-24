using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InputWindow : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text titleText;

    [SerializeField] private Button okBtn;
    [SerializeField] private Button cancelBtn;

    private void Awake()
    {
        Hide();
    }

    public void Show(string titleString, string inputString, Action onCancel, Action<string> onSubmit){
        gameObject.SetActive(true);
        
        inputField.text = inputString;
        titleText.text = titleString;

        okBtn.onClick.AddListener(() => {
            onSubmit(inputField.text);
            Hide();
            }
        );
        cancelBtn.onClick.AddListener(() => {
            onCancel();
            Hide();
        });
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
}
