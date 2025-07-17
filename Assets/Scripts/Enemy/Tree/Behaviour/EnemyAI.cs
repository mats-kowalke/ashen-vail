using System.Collections.Generic;

namespace BehaviorTree
{
    public class EnemyAI : Tree
    {
        protected override Node SetupTree()
        {
            return new Selector(new List<Node>
            {
                new Sequence(new List<Node>{
                    new CheckPlayerInAttackRange(this.transform),
                    new TaskAttack(this.transform)
                }),
                new Sequence(new List<Node>
                {
                    new CheckAggro(this.transform),
                    new TaskChase(this.transform)
                }),
                new TaskWander(this.transform)
            });
        }
    }
}