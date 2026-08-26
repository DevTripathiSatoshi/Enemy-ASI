using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Doom_Dude.EnemyASI
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float healthRegenAmount = 10f;
        [SerializeField] private float healthRegenInterval = 5f; // Regenerates every 5 seconds
        [SerializeField] private float destroyDelayAfterDeath = 10f; // Disappears after 10 seconds

        [Header("Events")]
        public UnityEvent OnTakeDamage;
        public UnityEvent OnDeath;

        private float currentHealth;
        private bool isDead = false;
        private Coroutine regenCoroutine;

        public bool IsDead => isDead;
        public float CurrentHealth => currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
            regenCoroutine = StartCoroutine(HealthRegenerationRoutine());
        }

        public void TakeDamage(float amount)
        {
            if (isDead) return;

            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            
            OnTakeDamage?.Invoke();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            OnDeath?.Invoke();

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }

            // Disable colliders so it doesn't block the player or take further hits
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }

            Destroy(gameObject, destroyDelayAfterDeath);
        }

        private IEnumerator HealthRegenerationRoutine()
        {
            while (!isDead)
            {
                yield return new WaitForSeconds(healthRegenInterval);

                if (currentHealth < maxHealth)
                {
                    currentHealth += healthRegenAmount;
                    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                }
            }
        }
    }
}
