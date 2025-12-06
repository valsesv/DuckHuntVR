using UnityEngine;
using DG.Tweening;

/// <summary>
/// Target motion script that uses DOTween to animate target movement.
/// Moves back and forth using a simple move amount with yoyo loop.
/// </summary>
public class TargetMotion : MonoBehaviour
{
    [Header("Motion Settings")]
    [SerializeField] private Vector3 moveAmount = Vector3.right;
    [SerializeField] private float duration = 2f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;

    private Tween _motionTween;
    private Vector3 _startPosition;
    private bool _isPlaying = false;

    private void Awake()
    {
        _startPosition = transform.position;
    }

    private void Start()
    {
        StartMotion();
    }

    private void OnDestroy()
    {
        if (_motionTween != null)
        {
            _motionTween.Kill();
        }
    }

    /// <summary>
    /// Starts the target motion.
    /// </summary>
    public void StartMotion()
    {
        if (_isPlaying)
        {
            return;
        }

        CreateMotionTween();
        _motionTween.Play();
        _isPlaying = true;
    }

    private void CreateMotionTween()
    {
        // Kill existing tween if any
        _motionTween?.Kill();

        // Reset to start position
        transform.position = _startPosition;

        // Create the move tween with relative movement
        _motionTween = transform.DOMove(_startPosition + moveAmount, duration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo)
            .OnComplete(() => _isPlaying = false);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the movement path
        Gizmos.color = Color.yellow;
        Vector3 startPos = Application.isPlaying ? _startPosition : transform.position;
        Vector3 endPos = startPos + moveAmount;

        Gizmos.DrawWireSphere(startPos, 0.2f);
        Gizmos.DrawWireSphere(endPos, 0.2f);
        Gizmos.DrawLine(startPos, endPos);
    }
}

