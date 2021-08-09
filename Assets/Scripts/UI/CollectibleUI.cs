using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectibleUI : MonoBehaviour
{
    private CollectibleCounter _collectibleCounter;
    private TMP_Text _collectibleText;
    private string _collectibles;

    void Start()
    {
        _collectibleText = GetComponent<TMP_Text>();
        _collectibleCounter = FindObjectOfType<CollectibleCounter>();
        _collectibleCounter.onCollectible += TextPopA;


        _collectibleText.text = $"{_collectibleCounter.CollectibleCount:00}";;
    }

    private void OnDisable()
    {
        _collectibleCounter.onCollectible -= TextPopA;
    }

    // Update is called once per frame
    private void Update()
    {
        _collectibleText.text = $"{_collectibleCounter.CollectibleCount:00}";;
        
    }

    void TextPopA()
    {
        _collectibleText.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), .1f).OnComplete(TextPopB);
    }

    void TextPopB()
    {
        _collectibleText.transform.DOScale(new Vector3(1.00f, 1.00f, 1.00f), .3f);
    }
}