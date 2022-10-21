using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class Minion_Spawner : MonoBehaviour
{
    //Responsible for creating all of our enemies and spawning them in the scene

    public Transform Target; //require a Player/Target to follow **see what to do after with towers
    public int NumberOfMinionsToSpawn = 5;
    public float SpawnDelay = 1f;
    public List<Minion_Behaviours> MinionPrefabs = new List<Minion_Behaviours>(); //reference of the prefabs you want to spaewn

   // public Transform DwarfSpawnPoint;      // I want to use this 2 lines to be able to...
   // public Transform SkeletonSpawnPoint;     //spawn each team in each base

    public SpawnMethod MinionSpawnMethod = SpawnMethod.Melee;
    // public SpawnMethod MinionSpawnRanged = SpawnMethod.Ranged;

    private NavMeshTriangulation Triangulation;

    private Dictionary<int, ObjectPool> MinionObjectPool = new Dictionary<int, ObjectPool>(); //how we manage the object pool for each minion

    // public float SpawnWaveDelay = 60f; //to be used for waves after


    private void Awake() 
    {
        //loop through all the minions that we have and create an object pool instance for each minion prefab
        for (int i = 0; i < MinionPrefabs.Count; i++)
        {
            MinionObjectPool.Add(i, ObjectPool.CreateInstance(MinionPrefabs[i], NumberOfMinionsToSpawn));
        }
    }

    private void Start()
    {
        Triangulation = NavMesh.CalculateTriangulation();
        StartCoroutine(SpawnMinions());
    }

    private IEnumerator SpawnMinions()
    {
        WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);

        int SpawnedMinions = 0; //count for how manu minions we spawned so far

        while(SpawnedMinions < NumberOfMinionsToSpawn)
        {
           if(MinionSpawnMethod == SpawnMethod.Melee) //spawn minions
            {
                SpawnMeleeMinion(SpawnedMinions);
            }
           else if (MinionSpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomMinion();
            }

            SpawnedMinions++;

            yield return Wait;
        }
    }

    private void SpawnMeleeMinion(int SpawnedMinions)
    {
        int SpawnIndex = SpawnedMinions % MinionPrefabs.Count;

        DoSpawnMinion(SpawnIndex);
    }

    private void SpawnRandomMinion()
    {
        DoSpawnMinion(Random.Range(0, MinionPrefabs.Count));
    }

    private void DoSpawnMinion(int SpawnIndex)
    {
        PoolableObject poolableObject = MinionObjectPool[SpawnIndex].GetObject();

        if(poolableObject != null)
        {
            Minion_Behaviours minions = poolableObject.GetComponent<Minion_Behaviours>();
           
            int VertexIndex = Random.Range(0, 1); //find the right spot for spawning changing the number
           // int VertexIndex = Random.Range(0, Triangulation.vertices.Length);

            NavMeshHit Hit;
            if(NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out Hit, 2f,-1))
            {
                minions.Agent.Warp(Hit.position);
                //minions needs to get enabled and start chasing now
                minions.Movement.Target = Target;
                minions.Agent.enabled = true;
                minions.Movement.StartChasing();
            }
        }
    }


    //choosing how to spawn enemies
    public enum SpawnMethod
    {
        Melee,
      //Ranged,
        Random 
    }
}
