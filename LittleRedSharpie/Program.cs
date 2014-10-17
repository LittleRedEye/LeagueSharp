#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace LittleRedSharpie
{
    class Program
    {
        public static string ChampionName = null;
        //public static Menu _menu;
        public static Orbwalking.Orbwalker _orbwalker;

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            //new Cassiopeia();
            //ChampionName = ObjectManager.Player.ChampionName;
        }

        public static void OnGameLoad(EventArgs args)
        {
            ChampionName = ObjectManager.Player.ChampionName;
            //Game.PrintChat(ChampionName.ToString());
            switch (ChampionName)
            {
                case "Cassiopeia":
                    new Cassiopeia();
                    break;
                case "Varus":
                    new Varus();
                    break;
                case "Pantheon":
                    new Pantheon();
                    break;
                case "JarvanIV":
                    new SKOJarvanIV();
                    break;
                case "Rengar":
                    new SKORengar();
                    break;
                case "Karma":
                    new SKOKarma();
                    break;
                //case "Mordekaiser":
                    //new Mordekaiser();
                    //break;
                default:
                    break;
            } 
        }

        internal static void castSpell(Obj_AI_Base target, Spell spell, bool onTarget)
        {
            if (ObjectManager.Player.Spellbook.CanUseSpell(spell.Slot) != SpellState.Ready || Vector3.Distance(ObjectManager.Player.Position, target.ServerPosition) > spell.Range)
            {
                return;
            }
            if (onTarget)
            {
                spell.CastOnUnit(target);
            }
            else
            {
                var prediction = spell.GetPrediction(target, true);
                if (prediction.Hitchance >= HitChance.High)
                {
                    spell.Cast(prediction.CastPosition);
                }
            }
        }

        internal static void Draw(List<Spell> SpellList, Menu menu)
        {
            if (menu.Item("disabledraw").GetValue<bool>()) { return; }
            foreach (var spell in SpellList)
            {
                //Cassiopeia.cassMenu.
                var menuItem = menu.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active && (spell.Level > 0) && spell.IsReady()) { Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, spell.IsReady() ? menuItem.Color : Color.Red); }
            }
        }
    }
}
