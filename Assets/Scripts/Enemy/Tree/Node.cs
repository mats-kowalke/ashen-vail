using System.Collections.Generic;

namespace BehaviorTree
{
    public class Node
    {
        public enum State
        {
            RUNNING,
            SUCCESS,
            FAILIURE,
            UNKNOWN
        }

        protected State state;
        public Node parent;
        protected internal List<Node> children;

        public Node()
        {
            this.children = new List<Node>();
            this.parent = null;
            this.state = State.UNKNOWN;
        }

        public Node(List<Node> children)
        {
            this.children = children;
            foreach (Node child in children) child.parent = this;
            this.parent = null;
            this.state = State.UNKNOWN;
        }

        public Node(Node parent)
        {
            this.children = new List<Node>();
            this.parent = parent;
            this.state = State.UNKNOWN;
        }

        public void Attach(Node child)
        {
            child.parent = this;
            this.children.Add(child);
        }

        public virtual State Evaluate() => State.FAILIURE;

    }

}
