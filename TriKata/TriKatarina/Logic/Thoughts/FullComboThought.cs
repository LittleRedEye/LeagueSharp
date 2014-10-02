using System;
using System.Linq;
using LeagueSharp.Common;
using Triton;
using Triton.Constants;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    class FullComboThought : Thought
    {
        public override bool ShouldActualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            return context.Plugin.Config.Item("ComboKey").GetValue<KeyBind>().Active && context.Target != null && context.Target.Unit.IsValid && (!context.CastingUlt || context.Plugin.Config.Item("StopUlt").GetValue<bool>() || context.Targets.Any(x => x.IgniteCanKill));
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;

            if ((!context.CastingUlt || (context.Plugin.Config.Item("StopUlt").GetValue<bool>()) && context.Target.Unit.Health <
                                    (context.Target.DamageContext.QDamage + context.Target.DamageContext.WDamage +
                                     context.Target.DamageContext.EDamage)))
            {
                if (context.Plugin.Config.Item("ComboItems").GetValue<bool>())
                    UseItems(context.Target);

                if (context.Plugin.Q.IsReady())
                    KatarinaUtilities.CastQ(context.Target);

                if (context.Plugin.Config.Item("ComboDetonateQ").GetValue<bool>() &&
                    context.Target.Unit.Buffs.Any(x => x.Name == "katarinaqmark"))
                {
                    if (!context.Plugin.Q.IsReady())
                        KatarinaUtilities.CastE(context.Target);

                    if (!context.Plugin.E.IsReady())
                        KatarinaUtilities.CastW(context.Target);
                }
                else
                {
                    if (context.Plugin.E.IsReady())
                    KatarinaUtilities.CastE(context.Target);

                    if (!context.Plugin.E.IsReady())
                        KatarinaUtilities.CastW(context.Target);
                }

                if (context.Plugin.R.IsReady())
                    KatarinaUtilities.CastR();
            }

            var igniteTarget = context.Targets.Where(x => x.IgniteCanKill).OrderByDescending(x => x.Unit.Health).FirstOrDefault();
            if (igniteTarget != null)
                KatarinaUtilities.CastIgnite(igniteTarget);
        }

        private void UseItems(Target target)
        {
            if (Items.CanUseItem((int)ItemIds.DeathfireGrasp) && target.Unit.IsValidTarget(600))
                Items.UseItem((int)ItemIds.DeathfireGrasp, target.Unit);
            if (Items.CanUseItem((int)ItemIds.HextechGunblade) && target.Unit.IsValidTarget(600))
                Items.UseItem((int)ItemIds.HextechGunblade, target.Unit);
            if (Items.CanUseItem((int)ItemIds.BilgewaterCutlass) && target.Unit.IsValidTarget(450))
                Items.UseItem((int)ItemIds.BilgewaterCutlass, target.Unit);
            if (Items.CanUseItem((int)ItemIds.BladeOfTheRuinedKing) && target.Unit.IsValidTarget(450))
                Items.UseItem((int)ItemIds.BladeOfTheRuinedKing, target.Unit);
        }

    }
}
