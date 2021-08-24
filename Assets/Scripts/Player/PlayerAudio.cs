using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private AudioClip dashAudio;
    [SerializeField] private AudioClip walkAudio;
    [SerializeField] private AudioClip landAudio;
    [SerializeField] private AudioClip switchAudio;

    private AudioSource _audioSource;
    
    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.spread = 1f;
    }
    
    public void PlayJumpAudio()
    {
        _audioSource.PlayOneShot(jumpAudio);
    }
    
    public void PlayDashAudio()
    {
        _audioSource.PlayOneShot(dashAudio);
    }
    
    public void PlayLandAudio()
    {
        _audioSource.PlayOneShot(landAudio);
    }
    
    public void PlaySwitchAudio()
    {
        _audioSource.PlayOneShot(switchAudio);
    }
    
    public void PlayWalkAudio()
    {
        _audioSource.loop = true;
        _audioSource.clip = walkAudio;
        _audioSource.Play();
    }
    
    public void PauseWalkAudio()
    {
        _audioSource.Pause();
    }
}
