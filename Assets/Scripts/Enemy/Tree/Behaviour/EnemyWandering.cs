using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class EnemyWandering : MonoBehaviour
    {
        public float wanderRadius = 15f;
        public float navMeshSampleDistance = 10f;
        public float waitTimeAtPoint = 2f;
        public float lookAroundTime = 2f;
        public float rotateSpeed = 90f;
        
        private NavMeshAgent agent;
        private Animator animator;
        private Coroutine wanderCoroutine;
        private bool isWandering = false;
        private bool isLookingAround = false;
        private bool isMovingToDestination = false;

        public bool IsLookingAround => isLookingAround;
        public bool IsMovingToDestination => isMovingToDestination;

        public bool HasReachedDestination
        {
            get
            {
                if (this.agent.pathPending) return false;
            
                if (this.agent.remainingDistance <= this.agent.stoppingDistance + 0.2f)
                {
                    if (!this.agent.hasPath || this.agent.velocity.sqrMagnitude < 0.01f)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        public void Start()
        {
            this.agent = this.GetComponent<NavMeshAgent>();
            this.animator = this.GetComponentInChildren<Animator>();
        }
        
        public void StartWander()
        {
            if (!this.isWandering)
            {
                this.agent.isStopped = false;
                this.agent.ResetPath();
            
                this.isWandering = true;

                if (this.wanderCoroutine != null)
                {
                    StopCoroutine(this.wanderCoroutine);
                }
                wanderCoroutine = StartCoroutine(WanderRoutine());
            }
        }
        
        public void StopWander()
        {
            if (this.wanderCoroutine != null)
            {
                StopCoroutine(this.wanderCoroutine);
                this.wanderCoroutine = null;
            }
        
            this.isWandering = false;
            this.isLookingAround = false;
            this.isMovingToDestination = false;
            
            this.animator.SetBool("isWandering", false);
        }
        
        private IEnumerator WanderRoutine()
        {
            while (isWandering)
            {
                FindNewDestination();

                float waitTime = 0f;
                const float maxWaitTime = 10f;
            
                while (!HasReachedDestination && waitTime < maxWaitTime)
                {
                    waitTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                Debug.Log("Stopping");
                
                agent.ResetPath();
                agent.isStopped = true;
                
                isMovingToDestination = false;
                isLookingAround = true;
                
                animator.SetBool("isWandering", false);
                
                Debug.Log("Waiting");
                yield return new WaitForSeconds(waitTimeAtPoint);
            
                
                agent.isStopped = false;
                Debug.Log("Continue Wandering");
            }
        }
        
        private void FindNewDestination()
        {
            int attempts = 0;
            const int maxAttempts = 30;

            while (attempts < maxAttempts)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection.y = 0f;
                Vector3 target = transform.position + randomDirection;

                if (NavMesh.SamplePosition(target, out NavMeshHit hit, navMeshSampleDistance,
                        NavMesh.AllAreas))
                {
                    if (Vector3.Distance(transform.position, hit.position) > 2f)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(hit.position);
                        isMovingToDestination = true;
                        animator.SetBool("isWandering", true);
                        Debug.Log(hit.position);
                        return;
                    }
                }

                attempts++;
            }
            isMovingToDestination = false;
        }
        
    }
}