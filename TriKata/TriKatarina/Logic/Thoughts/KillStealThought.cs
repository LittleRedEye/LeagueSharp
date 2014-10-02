using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Triton.Constants;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    public enum KillStealResult
    {
        CantKill,
        QKill,
        WKill,
        EKill,
        QweKill,
        QwKill,
        QeKill,
        WeKill,
        UltKs,
        FullComboKs,
        FullComboItemsKs,
        ItemsKs,

    }

    class KillStealThought : ParallelThought
    {
        public override bool ShouldActualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            return context.Plugin.Config.Item("KillSteal").GetValue<bool>() && context.Targets.Any(x => CalculateKillSteal(x) != KillStealResult.CantKill);
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext) contextObj;

            var ks = default(KillStealResult);
            var target = context.Targets.FirstOrDefault(x => (ks = CalculateKillSteal(x)) != KillStealResult.CantKill);

            switch (ks)
            {
                case KillStealResult.QKill:
                    KatarinaUtilities.CastQ(target);
                    break;
                case KillStealResult.WKill:
                    KatarinaUtilities.CastW();
                    break;
                case KillStealResult.EKill:
                    KatarinaUtilities.CastE(target);
                    break;
                case KillStealResult.QwKill:
                    KatarinaUtilities.CastW();
                    break;
                case KillStealResult.QeKill:
                    KatarinaUtilities.CastE(target);
                    break;
                case KillStealResult.QweKill:
                    KatarinaUtilities.CastE(target);
                    break;
                case KillStealResult.WeKill:
                    KatarinaUtilities.CastW();
                    break;
                case KillStealResult.UltKs:
                    KatarinaUtilities.CastR();
                    break;
                case KillStealResult.FullComboKs:
                    KatarinaUtilities.CastE(target);
                    KatarinaUtilities.CastQ(target);
                    KatarinaUtilities.CastW(target);
                    KatarinaUtilities.CastR();
                    break;
                case KillStealResult.FullComboItemsKs:
                    UseItems(target);
                    break;
                case KillStealResult.ItemsKs:
                    UseItems(target);
                    break;
            }
        }

        public KillStealResult CalculateKillSteal(Target enemy)
        {
            if (enemy.Unit.Health <= enemy.DamageContext.QDamage && enemy.Unit.IsValidTarget(Katarina.Instance.Q.Range) && Katarina.Instance.Q.IsReady())
                return KillStealResult.QKill;

            if (enemy.Unit.Health <= enemy.DamageContext.WDamage && enemy.Unit.IsValidTarget(Katarina.Instance.W.Range) && Katarina.Instance.W.IsReady())
                return KillStealResult.WKill;

            if (enemy.Unit.Health <= enemy.DamageContext.EDamage && enemy.Unit.IsValidTarget(Katarina.Instance.E.Range) && Katarina.Instance.E.IsReady())
                return KillStealResult.EKill;

            if (enemy.Unit.Health <= (enemy.DamageContext.QDamage + enemy.DamageContext.WDamage) && enemy.Unit.IsValidTarget(Katarina.Instance.W.Range) && Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady())
                return KillStealResult.QwKill;

            if (enemy.Unit.Health <= (enemy.DamageContext.QDamage + enemy.DamageContext.EDamage) && enemy.Unit.IsValidTarget(Katarina.Instance.Q.Range) && Katarina.Instance.Q.IsReady() && Katarina.Instance.E.IsReady())
                return KillStealResult.QeKill;

            if (enemy.Unit.Health <= (enemy.DamageContext.WDamage + enemy.DamageContext.EDamage) && enemy.Unit.IsValidTarget(Katarina.Instance.E.Range) && Katarina.Instance.E.IsReady() && Katarina.Instance.W.IsReady())
                return KillStealResult.WeKill;

            if (enemy.Unit.Health <= (enemy.DamageContext.QDamage + enemy.DamageContext.WDamage + enemy.DamageContext.EDamage) && enemy.Unit.IsValidTarget(Katarina.Instance.Q.Range) && Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady() && Katarina.Instance.E.IsReady())
                return KillStealResult.QweKill;

            if (Katarina.Instance.Config.Item("KsUseUlt").GetValue<bool>())
            {
                if (enemy.Unit.Health <= (enemy.DamageContext.QDamage + enemy.DamageContext.WDamage + enemy.DamageContext.EDamage + enemy.DamageContext.RDamage) && enemy.Unit.IsValidTarget(Katarina.Instance.E.Range) && Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady() && Katarina.Instance.E.IsReady() && Katarina.Instance.R.IsReady())
                    return KillStealResult.FullComboKs;

                if (enemy.Unit.Health <= enemy.DamageContext.RDamage && ObjectManager.Player.Distance(enemy.Unit, true) <= Katarina.Instance.R.Range * Katarina.Instance.R.Range - 100)
                    return KillStealResult.UltKs;
            }

            else if (Katarina.Instance.Config.Item("KsUseItems").GetValue<bool>())
            {
                if (enemy.Unit.Health <= (enemy.DamageContext.QDamage + enemy.DamageContext.WDamage + enemy.DamageContext.EDamage + enemy.DamageContext.RDamage + enemy.DamageContext.ItemDamage) && enemy.Unit.IsValidTarget(Katarina.Instance.E.Range) && Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady() && Katarina.Instance.E.IsReady() && Katarina.Instance.R.IsReady())
                    return KillStealResult.FullComboItemsKs;
                
                if (enemy.Unit.Health <= (enemy.DamageContext.QDamage + enemy.DamageContext.WDamage + enemy.DamageContext.EDamage + enemy.DamageContext.ItemDamage) && enemy.Unit.IsValidTarget(Katarina.Instance.E.Range) && Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady() && Katarina.Instance.E.IsReady())
                    return KillStealResult.ItemsKs;
            }

            return KillStealResult.CantKill;
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
