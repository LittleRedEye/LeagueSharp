using System.Linq;
using LeagueSharp.Common;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    class AcquireTargetThought : ParallelThought
    {
        public override bool ShouldActualize(object contextObj)
        {
            return true;
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;

            var target = SimpleTs.GetTarget(context.Plugin.E.Range, SimpleTs.DamageType.Magical);

            if (target != null)
                context.Target = context.Targets.FirstOrDefault(x=>x.Unit.NetworkId == target.NetworkId);
        }
    }
}
