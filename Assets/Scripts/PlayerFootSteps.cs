using System.Collections;
using UnityEngine;

public class PlayerFootSteps : MonoBehaviour
{
    [SerializeField] private AudioClip footstepClip;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        StartCoroutine(FootstepRoutine());
    }

    IEnumerator FootstepRoutine()
    {
        while (true)
        {
            if (playerController != null && Mathf.Abs(playerController.GetMoveInput()) > 0.1f && playerController.IsGrounded())
            {
                AudioManager.Instance.PlaySound(footstepClip, 0.5f);
                yield return new WaitForSeconds(0.35f); // Adjust delay between footsteps as needed
            }
            else
            {
                yield return null;
            }
        }
    }
}
