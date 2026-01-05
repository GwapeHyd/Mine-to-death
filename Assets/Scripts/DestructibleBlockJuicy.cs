using System.Collections;
using UnityEngine;

public class DestructibleBlockJuicy : DestructibleBlock
{
    [Header("Juice Effects")]
    [SerializeField] private float hitShakeDuration = 0.1f;
    [SerializeField] private float hitShakeAmount = 0.1f;
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector3 originalPosition;

    private new void Start()
    {
        base.Start();
        originalPosition = transform.position;
    }

    private new void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        // Effet de shake quand touché
        StartCoroutine(ShakeEffect());
    }

    private IEnumerator ShakeEffect()
    {
        float elapsed = 0f;

        while (elapsed < hitShakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = hitShakeAmount * shakeCurve.Evaluate(elapsed / hitShakeDuration);
            
            transform.position = originalPosition + (Vector3)Random.insideUnitCircle * strength;
            
            yield return null;
        }

        transform.position = originalPosition;
    }
}