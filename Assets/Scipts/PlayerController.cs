using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    Rigidbody rb;
    Vector3 move;
    float speedMultiplier = 1f;
    float noControlUntil;
    Coroutine boost;

    void Awake() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        move = new Vector3(h, 0f, v);
    }

    void FixedUpdate()
    {

        Vector3 direction = Vector3.ClampMagnitude(move, 1f)
            * speed * speedMultiplier;

        if (Time.time >= noControlUntil)
            rb.linearVelocity = new Vector3(
                direction.x, rb.linearVelocity.y, direction.z);
    }

    // Called only when a hazard has actually damaged the player.
    public void Knockback(Vector3 push, float seconds)
    {
        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(0f, v.y, 0f);
        rb.AddForce(push, ForceMode.Impulse);
        noControlUntil = Time.time + seconds;
    }

    // Clear movement before/after respawning.
    public void StopMovement(float seconds = 0f)
    {
        move = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        noControlUntil = Time.time + seconds;
    }


    public void BoostSpeed(float multiplier, float seconds)
    {
        if (boost != null) StopCoroutine(boost);
        boost = StartCoroutine(Boost(multiplier, seconds));
    }

    IEnumerator Boost(float multiplier, float seconds)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(seconds);
        speedMultiplier = 1f;
        boost = null;
    }
}
