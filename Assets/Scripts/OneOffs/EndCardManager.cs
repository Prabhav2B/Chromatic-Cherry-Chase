using System.Collections;
using System.Collections.Generic;
using BTAI;
using TMPro;
using UnityEngine;

public class EndCardManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField linkText;

    public string LinkText
    {
        set => linkText.text = value;
    }

    // Start is called before the first frame update
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}