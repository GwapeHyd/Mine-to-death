using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);
    
    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minY = -100f; 
    [SerializeField] private float maxY = 10f;  

    private void LateUpdate()
    {
        if (target == null) return;
       
        Vector3 desiredPosition = target.position + offset;
       
        if (useBounds)
        {
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        transform.position = smoothedPosition;
    }
}