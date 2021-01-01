using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private AudioClip explosionSound;

    [SerializeField]
    private float maxExplosionDamage = 100f;

    [SerializeField]
    private float safeDistance = 5f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bomb"))
        {
            // Creates explosion particle system
            GameObject explode = Instantiate(explosion, transform.position, transform.rotation);
            AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position);

            // Checks damage taken for all paladins
            foreach (GameObject paladin in GameObject.FindGameObjectsWithTag("Paladin"))
            {
                ExplosionCheck(paladin);
            }

            // Checks damage taken for the player
            ExplosionCheck(GameObject.FindGameObjectWithTag("Player"));

            gameObject.SetActive(false);
            Destroy(collision.gameObject);

            Destroy(gameObject, 0.5f);
            Destroy(explode, 1f);
        }
    }

    // Checks whether or not the given game object should take damage.
    // The game object must have a health amount and a TakeDamage(float) method,
    // and be either a paladin or the player.
    private void ExplosionCheck(GameObject obj)
    {
        float dist = Vector3.Distance(obj.transform.position, transform.position);
        if (dist < 5)
        {
            if (obj.tag == "Paladin")
                obj.GetComponent<EnemyBehavior>().TakeDamage(maxExplosionDamage * DamagePerentage(dist));
            else if (obj.tag == "Player")
                obj.GetComponent<PlayerHealth>().TakeDamage(maxExplosionDamage * DamagePerentage(dist));
        }
    }

    // Determines the percentage of maxExplosionDamage the object will
    // take based on its distance from the barrel.
    private float DamagePerentage(float dist)
    {
        return ((-1 * dist) / safeDistance) + 1;
    }
}
