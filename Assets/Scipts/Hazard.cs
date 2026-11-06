using System.Collections;
using UnityEngine;

// The same script works on spikes, guards and a timed crusher (Lecture 07).
[RequireComponent(typeof(Collider))]
public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    [SerializeField] float knockTime = 0.25f;

    [Header("Timed crusher (restTime = 0 for spikes and guards)")]
    [SerializeField] float restTime = 0f;
    [SerializeField] float warnTime = 0.7f;
    [SerializeField] float strikeTime = 0.4f;
    [SerializeField] GameObject warning;
    [SerializeField] GameObject strikeVisual;

    IEnumerator Start()
    {
        if (restTime <= 0f) yield break;

        Collider zone = GetComponent<Collider>();
        if (warning != null) warning.SetActive(false);
        if (strikeVisual != null) strikeVisual.SetActive(false);

        while (true)
        {
            zone.enabled = false;
            yield return new WaitForSeconds(restTime);

            // The assignment requires a visible warning of at least 0.5 s.
            if (warning != null) warning.SetActive(true);
            yield return new WaitForSeconds(Mathf.Max(0.5f, warnTime));

            if (warning != null) warning.SetActive(false);
            if (strikeVisual != null) strikeVisual.SetActive(true);
            zone.enabled = true;
            yield return new WaitForSeconds(strikeTime);

            if (strikeVisual != null) strikeVisual.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody body = other.attachedRigidbody;
        if (body == null || !body.CompareTag("Player")) return;

        Health health = body.GetComponent<Health>();
        if (health == null || !health.TakeDamage(damage)) return;

        Vector3 away = body.position - transform.position;
        away.y = 0f;
        if (away.sqrMagnitude < 0.001f)
        {
            away = body.transform.forward;
            away.y = 0f;
            if (away.sqrMagnitude < 0.001f) away = Vector3.forward;
        }

        PlayerController controller = body.GetComponent<PlayerController>();
        if (controller != null)
            controller.Knockback(away.normalized * knockback, knockTime);
    }
}
