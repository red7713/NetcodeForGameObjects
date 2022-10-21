using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion_Behaviours : PoolableObject
    // the minions behaviour are extending the poolable class for us to be able to use
    // in our object pool
{
    public Minion_Move Movement;
    public NavMeshAgent Agent;
    public Minions_SO MinionsSO;
    public int Health = 100; //this will be used in the future*

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration(); //whenever the agent becomes enabled we set up the configuration
        //it's better to use OnEnable instead of start because we're using objects from the pool and once they die
        //they will be disabled and when spawned again, enabled 
    }

    public override void OnDisable()
    {
        base.OnDisable();

        Agent.enabled = false;
    }

    public virtual void SetupAgentFromConfiguration()
    {
        Agent.acceleration = MinionsSO.Acceleration;
        Agent.angularSpeed = MinionsSO.AngularSpeed;
        Agent.areaMask = MinionsSO.AreaMask;
        Agent.avoidancePriority = MinionsSO.AvoidancePriority;
        Agent.baseOffset = MinionsSO.BaseOffset;
        Agent.height = MinionsSO.Height;
        Agent.obstacleAvoidanceType = MinionsSO.ObstacleAvoidanceType;
        Agent.radius = MinionsSO.Radius;
        Agent.speed = MinionsSO.Speed;
        Agent.stoppingDistance = MinionsSO.StoppingDistance;

        Movement.UpdateRate = MinionsSO.AIUpdateInterval;

        Health = MinionsSO.Health;

    }




}
