using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Looped shrinking tweener animation 
/// </summary>
public class ShrinkAndExpand : MonoBehaviour
{
    [SerializeField] private Vector3 shrinkScale = new Vector3(0.75f, 0.75f, 0.75f);
    [SerializeField] private float shrinkDuration = 0.55f;
    [SerializeField] private Vector3 expandScale = new Vector3(1.25f, 1.25f, 1.25f);
    [SerializeField] private float expandDuration = 0.55f;
    private Transform _halo;

    private void Awake()
    {
        _halo = this.GetComponent<Transform>();
    }

    private void Start()
    {
        Invoke(nameof(SquishStart), Random.Range(0f, 1f));
    }
    
void SquishStart()
    {
        _halo.DOScale(shrinkScale, shrinkDuration)
            .OnComplete(SquishRelease);
    }

    void SquishRelease()
    {
        _halo.DOScale(expandScale, expandDuration).OnComplete(SquishStart);;
    }

    public void StopTween()
    {
        _halo.DOKill();
    }
}
