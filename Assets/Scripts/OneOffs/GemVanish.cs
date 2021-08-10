using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GemVanish : MonoBehaviour
{
    [SerializeField] private GameObject largeGem;

    private SpriteRenderer _gem;
    private void Awake()
    {
        var _trigger = GetComponent<BoxCollider2D>();
        _trigger.isTrigger = true;

        _gem = largeGem.GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _gem.DOColor(Color.clear, 3f).OnComplete(DestroyOnComplete);
    }

    private void DestroyOnComplete()
    {
        Destroy(this);
    }
}
