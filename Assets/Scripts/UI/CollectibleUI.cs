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

        var collectibles = _collectibleCounter.CollectibleCount < 10
            ? "0" + _collectibleCounter.CollectibleCount
            : _collectibleCounter.CollectibleCount.ToString(CultureInfo.InvariantCulture);


        _collectibleText.text = collectibles;
    }

    private void OnDisable()
    {
        _collectibleCounter.onCollectible -= TextPopA;
    }

    // Update is called once per frame
    void Update()
    {
        _collectibles = _collectibleCounter.CollectibleCount < 10
            ? "0" + _collectibleCounter.CollectibleCount
            : _collectibleCounter.CollectibleCount.ToString(CultureInfo.InvariantCulture);


        _collectibleText.text = _collectibles;
        
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