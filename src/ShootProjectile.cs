using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private GameObject bombPrefab;
    [SerializeField]
    private GameObject freezeArrowPrefab;
    [SerializeField]
    private float maxProjectileSpeed = 30f;

    [SerializeField]
    private Image reticle;
    [SerializeField]
    private Color reticleEnemyColor;
    [SerializeField]
    private Color reticleBarrelColor;

    private GameObject currentProjectilePrefab;
    private Color reticleMissColor;
    private GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        reticleMissColor = reticle.color;
        currentProjectilePrefab = arrowPrefab;
        parent = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FireArrow(float drawTime)
    {
        GameObject projectile = Instantiate(currentProjectilePrefab, transform.position + transform.forward, transform.rotation);
        projectile.transform.Rotate(Vector3.right, 90f);

        // Determines how far the player drew back the arrow.
        float drawPercent = drawTime / parent.GetComponent<PlayerController>().GetMaxDrawTime();

        // Saves drawback on arrow gameObject
        if (currentProjectilePrefab.CompareTag("Projectile") || currentProjectilePrefab.CompareTag("FreezeArrow"))
            projectile.GetComponent<ArrowDamage>().SetDrawBackPercent(drawPercent);

        // Shoots based on drawback
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * maxProjectileSpeed * drawPercent, ForceMode.VelocityChange);

        // Moves it under the projectile parent gameObject.
        projectile.transform.SetParent(GameObject.FindGameObjectWithTag("ProjectileParent").transform);
    }


    private void FixedUpdate()
    {
        ReticleEffect();
    }

    private void ReticleEffect()
    {
        RaycastHit hit;
        bool hitObject = Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity);
        if (hitObject && (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Paladin")))
        {
            //currentProjectilePrefab = arrowPrefab;
            reticle.color = Color.Lerp(reticle.color, reticleEnemyColor, Time.deltaTime * 2);
            reticle.transform.localScale = Vector3.Lerp(reticle.transform.localScale, new Vector3(.7f, .7f, 1f), Time.deltaTime * 2);
        }
        else if (hitObject && hit.collider.CompareTag("Barrel"))
        {
            //currentProjectilePrefab = bombPrefab;
            reticle.color = Color.Lerp(reticle.color, reticleBarrelColor, Time.deltaTime * 2);
            reticle.transform.localScale = Vector3.Lerp(reticle.transform.localScale, new Vector3(.7f, .7f, 1f), Time.deltaTime * 2);
        }
        else
        {
            //currentProjectilePrefab = arrowPrefab;
            reticle.color = Color.Lerp(reticle.color, reticleMissColor, Time.deltaTime * 2);
            reticle.transform.localScale = Vector3.Lerp(reticle.transform.localScale, Vector3.one, Time.deltaTime * 2);
        }
    }

    // Projectile number should be 0, 1, or 2.
    public void SetProjectile(int projectileNumber)
    {
        if (projectileNumber == 0)
            currentProjectilePrefab = arrowPrefab;
        else if (projectileNumber == 1)
            currentProjectilePrefab = bombPrefab;
        else
            currentProjectilePrefab = freezeArrowPrefab;

    }
}
