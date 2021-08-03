
using CharTween;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// This script handles animation for TextMeshPro components.
/// This has been mostly adapted from the CharTweener repo.
/// </summary>
public class TextAnimation : MonoBehaviour
{
    [SerializeField] private TMP_Text _UIText;
    private TutorialUIUpdate _updater;
    private void Start()
    {
        StartTextAnim_1();
    }
    private void OnEnable()
    {
        _updater = GetComponentInParent<TutorialUIUpdate>(); 
        _updater.onControlSchemeChange += StartTextAnim_1;
    }
    
    private void OnDisable()
    {
        _updater.onControlSchemeChange -= StartTextAnim_1;
    }
    private void StartTextAnim_1( PlayerManager.ControlScheme _ = PlayerManager.ControlScheme.Gamepad)
    {
        var text = _UIText;
        var tweener = text.GetCharTweener();    
        for (var i = 0; i < tweener.CharacterCount; ++i)
        {
            // Move characters in a circle
            var circleTween = tweener.DOMoveY(i, 0.05f, 0.5f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);

            // Oscillate character color between yellow and white
            var colorTween = tweener.DOColor(i, Color.yellow, 0.5f)
                .SetLoops(-1, LoopType.Yoyo);

            // Offset animations based on character index in string
            var timeOffset = Mathf.Lerp(0, 1, i / (float)(tweener.CharacterCount - 1));
            //circleTween.fullPosition = timeOffset;
            colorTween.fullPosition = timeOffset;
        }
    }
}
