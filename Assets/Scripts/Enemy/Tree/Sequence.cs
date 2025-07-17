using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Node
    {

        public Sequence(List<Node> children) : base(children) {}
        
        public override State Evaluate()
        {
            bool childRunning = false;
            foreach (Node child in this.children)
            {
                State childResult = child.Evaluate();
                switch (childResult)
                {
                    case State.FAILIURE:
                        return State.FAILIURE;
                    case State.SUCCESS:
                        continue;
                    case State.RUNNING:
                        childRunning = true;
                        continue;
                    default:
                        return State.UNKNOWN;
                }
            }
            return childRunning ? State.RUNNING : State.SUCCESS;
        }
    }
}