using System;
using System.Linq;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private GameObject _collectibleEffect;
    [SerializeField] private AudioClip _gemAudioClip;

    private CollectibleCounter _collectibleCounter;


    private AudioSource _gemAudio;

    private void Awake()
    {
        var instances = FindObjectsOfType<LevelResetHandler>(false);
        var levelResetHandlesWhichAlsoHappensToBeTheAudioSource =
            instances.FirstOrDefault(instance => instance.IsStaticInstance);
        if (levelResetHandlesWhichAlsoHappensToBeTheAudioSource is { })
        {
            levelResetHandlesWhichAlsoHappensToBeTheAudioSource.enabled = true;

            _gemAudio = levelResetHandlesWhichAlsoHappensToBeTheAudioSource.GetComponent<AudioSource>(); //BAD COUPLING
        }

        _gemAudio.loop = false;
        _gemAudio.spatialBlend = 0f;
    }

    private void Start()
    {
        _collectibleCounter = FindObjectOfType<CollectibleCounter>();
        var levelResetHandler = FindObjectOfType<LevelResetHandler>();
        levelResetHandler.onLevelReload += ResetCollectible;
    }

    void ResetCollectible()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collectibleEffect != null)
        {
            var effect = Instantiate(_collectibleEffect, transform.position, Quaternion.Euler(-90f, 0f, 0f),
                transform.parent);
            effect.GetComponent<ParticleSystem>().Play();

            Destroy(effect, 10f);
        }

        var tweenAnimator = GetComponent<ShrinkAndExpand>();
        if (tweenAnimator != null)
        {
            tweenAnimator.StopTween();
        }

        _collectibleCounter.onCollectible?.Invoke();
        _gemAudio.PlayOneShot(_gemAudioClip);
        //Destroy(this.gameObject);
        gameObject.SetActive(false);
    }
}