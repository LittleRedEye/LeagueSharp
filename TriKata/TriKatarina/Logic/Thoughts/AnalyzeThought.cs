using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Triton.Constants;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    
    class AnalyzeThought : ParallelThought
    {
        public override bool ShouldActualize(object contextObj)
        {
            return true;
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            // || !ObjectManager.Player.IsStunned || !ObjectManager.Player.IsPacified || !ObjectManager.Player.IsDead || !ObjectManager.Player.CanCast || !ObjectManager.Player.CanAttack

            if (!ObjectManager.Player.IsChannelingImportantSpell())
                context.CastingUlt = false;

            context.Targets = ObjectManager.Get<Obj_AI_Hero>().Where(x=>x.IsEnemy).Select(x => new Target(x)).ToList();

            context.Targets.ForEach(x=>x.CalculateDamage());
        }
    }
}
