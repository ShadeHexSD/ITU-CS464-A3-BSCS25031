using UnityEngine;

// Assignment 03 Part A: a second physics force, separate from knockback.
[RequireComponent(typeof(Collider))]
public class BouncePad : MonoBehaviour
{
    [SerializeField] float bounceStrength = 8f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;

        // Cancel downward falling speed so this pad always gives a visible bounce.
        Vector3 v = body.linearVelocity;
        if (v.y < 0f) body.linearVelocity = new Vector3(v.x, 0f, v.z);
        body.AddForce(Vector3.up * bounceStrength, ForceMode.VelocityChange);
    }
}
