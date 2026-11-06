using UnityEngine;

// Assignment 03 Part C: a wide trigger below the platforms.
[RequireComponent(typeof(Collider))]
public class FallZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null)
            GameManager.Instance.PlayerFell();
    }
}
