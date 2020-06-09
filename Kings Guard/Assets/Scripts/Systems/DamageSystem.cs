using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class DamageSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.ForEach((ref HealthComponent healthComponent) =>
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                healthComponent.currentHealth -= 5;
                Debug.Log(healthComponent.currentHealth);
            }
        });    
    }
}
