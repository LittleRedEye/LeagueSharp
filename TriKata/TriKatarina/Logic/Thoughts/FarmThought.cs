using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    class FarmThought : Thought
    {
        public override bool ShouldActualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            return context.Plugin.Config.Item("FarmKey").GetValue<KeyBind>().Active;
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            var qFarm = context.Plugin.Config.Item("QFarm").GetValue<bool>();
            var wFarm = context.Plugin.Config.Item("WFarm").GetValue<bool>();
            var eFarm = context.Plugin.Config.Item("EFarm").GetValue<bool>();
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x != null && x.IsValid && x.IsEnemy))
            {

                //var wDmg = DamageLib.getDmg(minion, DamageLib.SpellType.W);
                var wDmg = ObjectManager.Player.GetSpellDamage(minion, SpellSlot.W);
                var eDmg = ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E);

                if (minion.IsValidTarget(context.Plugin.W.Range))
                {
                    if (qFarm && wFarm)
                    {
                        if (KatarinaUtilities.GetRangedHealthCheck(ObjectManager.Player, minion, SpellSlot.Q, 1000f, Katarina.Instance.Q.Speed) && context.Plugin.Q.IsReady())
                        {
                            KatarinaUtilities.CastQ(minion);
                        }
                        else if (context.Plugin.W.IsReady() && minion.Health <= wDmg*0.75)
                        {
                            KatarinaUtilities.CastW();
                        }
                    }
                    else if (qFarm && context.Plugin.Q.IsReady() && KatarinaUtilities.GetRangedHealthCheck(ObjectManager.Player, minion, SpellSlot.Q, 1000f, 1400f))
                    {
                        KatarinaUtilities.CastQ(minion);
                    }
                    else if (wFarm && context.Plugin.W.IsReady() && minion.Health <= wDmg)
                    {
                        KatarinaUtilities.CastW();
                    }
                }
                else
                {
                    if (qFarm && KatarinaUtilities.GetRangedHealthCheck(ObjectManager.Player, minion, SpellSlot.Q, 1000f, Katarina.Instance.Q.Speed) && minion.IsValidTarget(context.Plugin.Q.Range))
                    {
                        KatarinaUtilities.CastQ(minion);
                    }
                    if (eFarm && minion.Health <= eDmg*0.75 && minion.IsValidTarget(context.Plugin.E.Range))
                    {
                        KatarinaUtilities.CastE(minion);
                    }
                }
            }
        }
    }
}
