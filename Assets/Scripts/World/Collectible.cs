using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private GameObject _collectibleEffect;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collectibleEffect != null)
        {
            var effect = Instantiate(_collectibleEffect, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform.parent);
            effect.GetComponent<ParticleSystem>().Play();
            
            Destroy(effect, 10f);
        }

        var tweenAnimator = GetComponent<ShrinkAndExpand>();
        if (tweenAnimator != null)
        {
            tweenAnimator.StopTween();
        }
        Destroy(this.gameObject);
    }
}
