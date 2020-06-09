using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class GameEntities : MonoBehaviour {

    private void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity entity = entityManager.CreateEntity(typeof(HealthComponent));

        entityManager.SetComponentData(entity, new HealthComponent { currentHealth = 20, maxHealth = 25});

    }
}
