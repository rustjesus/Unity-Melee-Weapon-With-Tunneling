using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMG_Inventory
{
    public class MeleeWeapon : MonoBehaviour
    {
        [HideInInspector] public Animator anim;
        //change these min/max values in PlayerAttack.cs
        [HideInInspector] public float maxDamageToDo;
        [HideInInspector] public float minDamageToDo;
        private PlayerAttacks attacks;
        [SerializeField] private bool isPlayer = true;
        public static bool usingMeleeWeapon = false;
        private Vector3 lastPosition;
        [SerializeField] private float hitRadius = 0.1f;
        [SerializeField] private LayerMask hitLayers; // Set this to include enemy layers
        private HashSet<Collider> hitColliders = new HashSet<Collider>(); // to prevent multiple hits in one swing

        private void OnEnable()
        {
            usingMeleeWeapon = true;
            lastPosition = transform.position;
        }
        private void OnDisable()
        {
            usingMeleeWeapon = false;
        }

        private void Update()
        {
            if (!isPlayer || attacks == null)
                return;

            if (attacks.attacking)
            {
                GetComponent<Collider>().enabled = true;

                // Tunneling prevention logic
                Vector3 currentPosition = transform.position;
                Vector3 direction = currentPosition - lastPosition;
                float distance = direction.magnitude;

                if (distance > 0f)
                {
                    RaycastHit[] hits = Physics.SphereCastAll(lastPosition, hitRadius, direction.normalized, distance, hitLayers);

                    foreach (RaycastHit hit in hits)
                    {
                        Collider other = hit.collider;
                        if (other != null && !hitColliders.Contains(other))
                        {
                            Health health = other.GetComponent<Health>();
                            if (health != null)
                            {
                                health.TakeDamage(Random.Range(minDamageToDo, maxDamageToDo), true, false);
                                hitColliders.Add(other);
                            }
                        }
                    }
                }

                lastPosition = currentPosition;
            }
            else
            {
                GetComponent<Collider>().enabled = false;
                hitColliders.Clear(); // reset for next attack
            }
        }
    }

}
