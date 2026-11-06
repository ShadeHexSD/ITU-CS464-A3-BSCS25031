using UnityEngine;

public enum PowerUpType { Speed, Shield }

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpType type = PowerUpType.Speed;
    [SerializeField] float multiplier = 1.6f;
    [SerializeField] float duration = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (type == PowerUpType.Speed)
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller == null) return;
            controller.BoostSpeed(multiplier, duration);
        }
        else
        {
            Health health = other.GetComponent<Health>();
            if (health == null) return;
            health.shielded = true;
        }

        Destroy(gameObject);
    }
}