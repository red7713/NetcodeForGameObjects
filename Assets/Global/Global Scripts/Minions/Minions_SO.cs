using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ScriptableObject that holds the BASE STATS for the minion. These can then be modified at object creation time to buff up the minions
/// and to reset their stats if they died or were modified at runtime.
/// </summary>

[CreateAssetMenu(fileName = "Minions Configuration", menuName = "ScriptableObject/Minions Configuration")]
public class Minions_SO : ScriptableObject
{
    // Minions Stats
    public int Health = 100;

    // NavMeshAgent Configs
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8;
    public float AngularSpeed = 120;
    // -1 means everything
    public int AreaMask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;
}
