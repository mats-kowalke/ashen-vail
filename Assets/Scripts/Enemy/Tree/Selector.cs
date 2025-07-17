using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Node
    {

        public Selector(List<Node> children) : base(children) {}
        
        public override State Evaluate()
        {
            foreach (Node child in this.children)
            {
                State childState = child.Evaluate();
                switch (childState)
                {
                    case State.FAILIURE:
                        continue;
                    case State.SUCCESS:
                        return State.SUCCESS;
                    case State.RUNNING:
                        return State.RUNNING;
                    default:
                        continue;
                }
            }
            return State.FAILIURE;
        }
    }
}