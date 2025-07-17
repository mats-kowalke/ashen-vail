using System;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public float detectionRadius = 10f;
    private Transform player;

    public void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("PlayerTag").transform;
    }

    public bool PlayerInRange()
    {
        var distance = Vector3.Distance(this.transform.position, this.player.position);
        return distance <= detectionRadius;
    }
    
    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(this.transform.position, this.player.position);
    }

    public Vector3 GetDirectionToPlayer()
    {
        return this.transform.position - this.player.position;
    }
    
}