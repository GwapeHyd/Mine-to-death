using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Vortex : MonoBehaviour
{
    [Header("Vortex settings")]
    public float radius = 5f;        
    public float pullSpeed = 8f;         
    public float swirlStrength = 4f;     
    public float duration = 2f;  
    public string playerTag = "Player";   
    public GameObject endMenuUI;    
    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        StartCoroutine(AttractPlayerCoroutine(rb, other.transform));
    }

    private IEnumerator AttractPlayerCoroutine(Rigidbody2D rb, Transform playerTransform)
    {
        float elapsed = 0f;
        endMenuUI.SetActive(true);
        GameManager.Instance.gameOver = true;

        while (elapsed < duration)
        {
            Vector2 directionToCenter = (Vector2)(transform.position - playerTransform.position);
            float distance = directionToCenter.magnitude;

            if (distance < 0.1f) break;

            Vector2 pullDirection = directionToCenter.normalized;
            Vector2 swirlDirection = new Vector2(-pullDirection.y, pullDirection.x);

            float pullFactor = Mathf.Clamp01((radius - distance) / radius);
            Vector2 pullForce = pullDirection * pullSpeed * pullFactor;
            Vector2 swirlForce = swirlDirection * swirlStrength * pullFactor;

            rb.linearVelocity = pullForce + swirlForce;

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

}