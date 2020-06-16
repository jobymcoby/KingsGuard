using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public ProjectileController projectilePrefab;
    [SerializeField] public Transform projectilePosition0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        ProjectileController projectile =
            Instantiate(projectilePrefab, projectilePosition0.position, Quaternion.identity);


        projectile.projectileSpeed = 4;

        if (this.transform.localScale.x < 0)
        {
            projectile.projectileDirection = Vector2.left;
        }
        if(this.transform.localScale.x > 0)
        {
            projectile.projectileDirection = Vector2.right;
            projectile.transform.localScale = -projectile.transform.localScale;
        }

    }
}
