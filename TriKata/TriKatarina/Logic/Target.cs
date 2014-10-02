using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Triton.Constants;

namespace TriKatarina.Logic
{
    public class Target
    {
        private Obj_AI_Hero _target;
        private DamageContext _damageContext = new DamageContext();
        private List<float> _distances = new List<float>();
        private int _killIndex;
        private static readonly Render.Text _renderText = new Render.Text(
                0, 0, "", 20, new ColorBGRA(255, 204, 0, 255));

        private static string[] _killStrings = new[]
        {
            "Harass", "Q - Kill", "W - Kill", "E - Kill", "Q+W - Kill", "Q+E - Kill", "E+W - Kill", "Q+E+W - Kill",
            "Q+E+W+Item - Kill", "Q+W+E+R: ", "Need Cds"
        };

        public Target(Obj_AI_Hero target)
        {
            _renderText.OutLined = true;
            _target = target;
        }

        public void CalculateDamage()
        {
            if (_target == null || !_target.IsValid)
                return;

            _damageContext.PDamage = Katarina.Instance.Q.IsReady()
                //? DamageLib.getDmg(_target, DamageLib.SpellType.Q, DamageLib.StageType.Default)
                ? ObjectManager.Player.GetSpellDamage(_target, SpellSlot.Q)
                : 0;
            _damageContext.QDamage = Katarina.Instance.Q.IsReady()
                ? ObjectManager.Player.GetSpellDamage(_target, SpellSlot.Q)
                : 0;
            _damageContext.WDamage = Katarina.Instance.W.IsReady()
                ? ObjectManager.Player.GetSpellDamage(_target, SpellSlot.W)
                : 0;
            _damageContext.EDamage = Katarina.Instance.E.IsReady()
                ? ObjectManager.Player.GetSpellDamage(_target, SpellSlot.E)
                : 0;
            _damageContext.RDamage = Katarina.Instance.R.IsReady()
                ? ObjectManager.Player.GetSpellDamage(_target, SpellSlot.R)*10
                : 0;

            _damageContext.DFGDamage = Items.CanUseItem((int) ItemIds.DeathfireGrasp)
                ? ObjectManager.Player.GetItemDamage(_target, Damage.DamageItems.Dfg)
                : 0;
            _damageContext.HXGDamage = Items.CanUseItem((int) ItemIds.HextechGunblade)
                ? ObjectManager.Player.GetItemDamage(_target, Damage.DamageItems.Hexgun)
                : 0;
            _damageContext.BWCDamage = Items.CanUseItem((int) ItemIds.BilgewaterCutlass)
                ? ObjectManager.Player.GetItemDamage(_target, Damage.DamageItems.Bilgewater)
                : 0;
            _damageContext.LiandrysDamage = Items.HasItem((int) ItemIds.LiandrysTorment)
                ? KatarinaUtilities.GetLiandrysDamage(this)
                : 0;

            var ignite = ObjectManager.Player.SummonerSpellbook.Spells.FirstOrDefault(x => x.Name == "summonerdot");
            _damageContext.IgniteDamage = ignite != null && ignite.State == SpellState.Ready
                ? ObjectManager.Player.GetSummonerSpellDamage(_target, Damage.SummonerSpell.Ignite)
                : 0;

            if (_target.Health >
                (_damageContext.PDamage + _damageContext.QDamage + _damageContext.EDamage + _damageContext.WDamage +
                 _damageContext.RDamage + _damageContext.ItemDamage))
            {
                _killIndex = 1;
            }

            else if (_target.Health <= _damageContext.QDamage)
            {
                _killIndex = Katarina.Instance.Q.IsReady() ? 2 : 11;
            }
            else if (_target.Health <= _damageContext.WDamage)
            {
                _killIndex = Katarina.Instance.W.IsReady() ? 3 : 11;
            }
            else if (_target.Health <= _damageContext.EDamage)
            {
                _killIndex = Katarina.Instance.E.IsReady() ? 4 : 11;
            }
            else if (_target.Health <= (_damageContext.QDamage + _damageContext.WDamage) && Katarina.Instance.Q.IsReady() &&
                     Katarina.Instance.W.IsReady())
            {
                if (Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady())
                    _killIndex = 5;
                else
                    _killIndex = 11;
            }
            else if (_target.Health <= (_damageContext.QDamage + _damageContext.EDamage) &&
                     Katarina.Instance.Q.IsReady() && Katarina.Instance.E.IsReady())
            {
                if (Katarina.Instance.Q.IsReady() && Katarina.Instance.E.IsReady())
                    _killIndex = 6;
                else
                    _killIndex = 11;
            }
            else if (_target.Health <= (_damageContext.WDamage + _damageContext.EDamage) &&
                     Katarina.Instance.W.IsReady() && Katarina.Instance.E.IsReady())
            {
                if (Katarina.Instance.W.IsReady() && Katarina.Instance.E.IsReady())
                    _killIndex = 7;
                else
                    _killIndex = 11;
            }
            else if (_target.Health <=
                     (_damageContext.QDamage + _damageContext.WDamage + _damageContext.EDamage) &&
                     Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady() &&
                     Katarina.Instance.E.IsReady())
            {
                if (Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady() &&
                    Katarina.Instance.E.IsReady())
                    _killIndex = 8;
                else
                    _killIndex = 11;
            }
            else if ((_target.Health <=
                      (_damageContext.QDamage + _damageContext.WDamage + _damageContext.EDamage +
                       _damageContext.ItemDamage) ||
                      _target.Health <=
                      (_damageContext.QDamage + _damageContext.PDamage + _damageContext.WDamage +
                       _damageContext.EDamage + _damageContext.ItemDamage)) &&
                     Katarina.Instance.Q.IsReady() &&
                     Katarina.Instance.W.IsReady() &&
                     Katarina.Instance.E.IsReady())
            {
                if (Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady() &&
                    Katarina.Instance.E.IsReady())
                    _killIndex = 9;
                else
                    _killIndex = 11;
            }
            else if (_target.Health <=
                     (_damageContext.QDamage + _damageContext.PDamage + _damageContext.WDamage +
                      _damageContext.EDamage + _damageContext.RDamage + _damageContext.ItemDamage))
            {
                if (Katarina.Instance.Q.IsReady() && Katarina.Instance.W.IsReady() &&
                    Katarina.Instance.E.IsReady())
                    _killIndex = 10;
                else
                    _killIndex = 11;
            }

        }

        public void DrawText()
        {
            if (_target == null || !_target.IsValid || !_target.IsHPBarRendered || _killIndex == 0)
                return;

            var pos = _target.HPBarPosition;
            var vec = new Vector2(pos.X+50, pos.Y + 175);
            
            _renderText.X = (int) vec.X;
            _renderText.Y = (int) vec.Y;
            _renderText.text = KillText;

            _renderText.OnEndScene();
        }

        public bool CanKill
        {
            get { return !_target.IsDead && _target.Health <= _damageContext.TotalDamage; }
        }

        public bool IgniteCanKill
        {
            get { return _target.Health < _damageContext.IgniteDamage && _target.IsValidTarget(600); }
        }

        public string KillText
        {
            get
            {
                
                if (_killIndex-1 != 10)
                    return _killStrings[_killIndex-1];

                return string.Format(_killStrings[_killIndex - 1] + "{0:4.1}s Kill",
                    ((_target.Health -
                      (_damageContext.QDamage + _damageContext.PDamage + _damageContext.WDamage + _damageContext.EDamage +
                       _damageContext.ItemDamage))*(1/_damageContext.RDamage))*2.5);
            }
        }

        public bool IsRunningAway
        {
            get { return false; }
        }

        public Obj_AI_Hero Unit
        {
            get { return _target; }
        }

        public DamageContext DamageContext
        {
            get { return _damageContext; }
        }
    }
}
