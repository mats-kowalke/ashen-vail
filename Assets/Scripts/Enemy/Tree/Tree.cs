using UnityEngine;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node root;

        protected virtual void Start()
        {
            this.root = this.SetupTree();
        }

        protected virtual void Update()
        {
            this.root?.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}