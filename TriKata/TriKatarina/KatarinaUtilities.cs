using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TriKatarina.Logic;
using TriKatarina.Logic.Thoughts;
using Triton;
using Triton.Constants;

namespace TriKatarina
{
    public static class KatarinaUtilities
    {
        private static readonly int _wardDistance = 300;

        public static float QTimeToHit { get; set; }
        public static float LastWardJump { get; set; }

        public static bool CastQ(Target target)
        {
            if (target == null || !target.Unit.IsValid || !Katarina.Instance.Q.IsReady() || !target.Unit.IsValidTarget(Katarina.Instance.Q.Range))
                return false;
            Katarina.Instance.Q.CastOnUnit(target.Unit, Katarina.Instance.Config.Item("packets").GetValue<bool>());
            return true;
        }

        public static bool CastQ(Obj_AI_Base target)
        {
            if (target == null || !target.IsValid || !Katarina.Instance.Q.IsReady() || !target.IsValidTarget(Katarina.Instance.Q.Range))
                return false;

            Katarina.Instance.Q.CastOnUnit(target, Katarina.Instance.Config.Item("packets").GetValue<bool>());

            return true;
        }


        public static bool CastE(Target target)
        {
            if (target == null || !target.Unit.IsValid || !Katarina.Instance.E.IsReady() || !target.Unit.IsValidTarget(Katarina.Instance.E.Range))
                return false;

            Katarina.Instance.E.CastOnUnit(target.Unit, Katarina.Instance.Config.Item("packets").GetValue<bool>());

            return true;
        }

        public static bool CastE(Obj_AI_Base target)
        {
            if (target == null || !target.IsValid || !Katarina.Instance.E.IsReady() || !target.IsValidTarget(Katarina.Instance.E.Range))
                return false;

            Katarina.Instance.E.CastOnUnit(target, Katarina.Instance.Config.Item("packets").GetValue<bool>());

            return true;
        }



        public static bool CastW(Target target)
        {
            if (target == null || !target.Unit.IsValid || !Katarina.Instance.W.IsReady() || !target.Unit.IsValidTarget(Katarina.Instance.W.Range))
                return false;

            return Katarina.Instance.W.Cast();

        }

        public static bool CastW()
        {
            if (!Katarina.Instance.W.IsReady())
                return false;

            return Katarina.Instance.W.Cast();
        }

        public static bool CastR()
        {
            if (Katarina.Instance.Q.IsReady() || Katarina.Instance.W.IsReady() || Katarina.Instance.E.IsReady() || !Katarina.Instance.R.IsReady() ||
                ObjectManager.Get<Obj_AI_Hero>().Count(x => x.IsValidTarget(Katarina.Instance.R.Range)) < 1)
                return false;

            return Katarina.Instance.R.Cast();
        }

        //public static bool GetRangedHealthCheck(Obj_AI_Base target, DamageLib.SpellType spellType, DamageLib.StageType stage, float precision, float speed)
        public static bool GetRangedHealthCheck(Obj_AI_Hero source, Obj_AI_Base target, SpellSlot slot, float precision, float speed)
        {
            return HealthPrediction.GetHealthPrediction(target,
                (int) ((ObjectManager.Player.Distance(target, false)*precision)/speed)) <
            //(0.75*DamageLib.getDmg(target, spellType, stage));
            (0.75 * Damage.GetSpellDamage(source, target, slot));
        }

        public static bool CastIgnite(Target target)
        {
            if (target == null || !target.Unit.IsValid || !target.Unit.IsValidTarget(600))
                return false;

            var ignite = ObjectManager.Player.SummonerSpellbook.Spells.FirstOrDefault(x => x.Name == "summonerdot");

            if (ignite == null)
                return false;

            return ObjectManager.Player.SummonerSpellbook.CastSpell(ignite.Slot, target.Unit);
        }

        public static bool WardJump(ThoughtContext context, float x, float y)
        {
            if (!context.Plugin.E.IsReady())
                return false;

            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(z => z.IsAlly))
            {
                if (IsValidJumpTarget(obj))
                {
                    context.Plugin.E.CastOnUnit(obj, true);
                    LastWardJump = Environment.TickCount + 2000;
                    return true;
                }
            }

            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>().Where(z => z.IsAlly))
            {
                if (IsValidJumpTarget(obj))
                {
                    context.Plugin.E.CastOnUnit(obj, true);
                    LastWardJump = Environment.TickCount + 2000;
                    return true;
                }
            }

            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>().Where(z => !z.IsAlly))
            {
                if (IsValidJumpTarget(obj))
                {
                    context.Plugin.E.CastOnUnit(obj, true);
                    LastWardJump = Environment.TickCount + 2000;
                    return true;
                }
            }

            foreach (var obj in ObjectManager.Get<Obj_AI_Base>().Where(IsWard))
            {
                if (IsValidJumpTarget(obj))
                {
                    context.Plugin.E.CastOnUnit(obj, true);
                    LastWardJump = Environment.TickCount + 2000;
                    return true;
                }
            }

            if (Environment.TickCount >= LastWardJump)
            {
                var wardSlot = Items.GetWardSlot();

                if (wardSlot != null)
                {
                    wardSlot.UseItem(new Vector3(x, y, 0));
                    LastWardJump = Environment.TickCount + 2000;
                }
            }

            return true;
        }

        public static double GetLiandrysDamage(Target target)
        {
            //return DamageLib.CalcMagicDmg(0.06 * target.Unit.MaxHealth, target.Unit);
            return Damage.CalcDamage(ObjectManager.Player, target.Unit, Damage.DamageType.Magical, 0.06*target.Unit.MaxHealth);
        }

        public static bool IsWard(Obj_AI_Base obj)
        {
            return obj.Name.Contains("Ward") || obj.Name.Contains("Wriggle") || obj.Name.Contains("Trinket");
        }

        public static bool IsValidJumpTarget(Obj_AI_Base obj)
        {
            return obj != null && ((obj.IsValidTarget(Katarina.Instance.E.Range, false) && obj.Distance(Game.CursorPos, true) <= _wardDistance*_wardDistance) || (obj.IsValid && IsWard(obj) && obj.Distance(GetMousePosition(), true) <= 300*300));
        }

        public static Vector3 GetMousePosition()
        {
            var range = 600;
            var myPos = ObjectManager.Player.ServerPosition;
            var mousePos = Game.CursorPos;

            var norm = (myPos - mousePos);
            norm.Normalize();

            return myPos - norm * range;
        }
    }
}
