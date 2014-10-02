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
    class Mordekaiser
    {
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public static Menu mordeMenu;

        public static SpellSlot IgniteSlot;

        public Mordekaiser()
        {
            //CustomEvents.Game.OnGameLoad += OnGameLoad;
            //Obj_AI_Base.OnPlayAnimation += OnAnimation;

            Game.PrintChat(string.Format("<font color='#F7A100'>{0} - {1} loaded.</font>", Assembly.GetExecutingAssembly().GetName().Name, Program.ChampionName));
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;

            LoadSpells();
            LoadMenu();
        }

        public void LoadMenu()
        {
            mordeMenu = new Menu("LittleRedSharpie: " + Program.ChampionName, Program.ChampionName, true);
            mordeMenu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Program._orbwalker = new Orbwalking.Orbwalker(mordeMenu.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);

            var comboMenu = new Menu("Combo", "combo");
            comboMenu.AddItem(new MenuItem("comboQ", "Combo Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboW", "Combo W").SetValue(false));
            comboMenu.AddItem(new MenuItem("comboE", "Combo E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboUltimate", "UseUltimate").SetValue(false));

            var mixedMenu = new Menu("Harras", "mixed");
            mixedMenu.AddItem(new MenuItem("mixedstationaryQ", "Q only on enemy action").SetValue(false));
            mixedMenu.AddItem(new MenuItem("mixedQ", "Harass Q").SetValue(true));
            mixedMenu.AddItem(new MenuItem("mixedW", "Harass W").SetValue(false));
            mixedMenu.AddItem(new MenuItem("mixedE", "Harass E").SetValue(true));

            var clearMenu = new Menu("Farm", "clear");
            clearMenu.AddItem(new MenuItem("clearQ", "Use Q").SetValue(new StringList(new[] { "Farm", "LaneClear", "Both", "No" }, 2)));
            clearMenu.AddItem(new MenuItem("clearW", "Use W").SetValue(new StringList(new[] { "Farm", "LaneClear", "Both", "No" }, 1)));
            clearMenu.AddItem(new MenuItem("clearE", "Use E").SetValue(new StringList(new[] { "Farm", "LaneClear", "Both", "No" }, 2)));

            var ultimateMenu = new Menu("Ultimate", "ultimate");
            ultimateMenu.AddItem(new MenuItem("ultimateKey", "Ultimate on Target").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
            ultimateMenu.AddItem(new MenuItem("ultimateAuto", "Auto Ultimate").SetValue(true));
            ultimateMenu.AddItem(new MenuItem("ultimateMin", "Min. Enemies in Range").SetValue(new Slider(1, 5, 1)));

            var killstealMenu = new Menu("Killsteal", "killsteal");
            killstealMenu.AddItem(new MenuItem("killstealEnabled", "Enabled").SetValue(true));
            //killstealMenu.AddItem(new MenuItem("killstealE", "Use E").SetValue(true)); 
            killstealMenu.AddItem(new MenuItem("killstealIgnite", "Use Ignite").SetValue(true));

            var drawingMenu = new Menu("Drawing", "drawing");
            drawingMenu.AddItem(new MenuItem("disabledraw", "Disable all drawing").SetValue(false));
            drawingMenu.AddItem(new MenuItem("WRange", "W Range").SetValue(new Circle(false, Color.FromArgb(100, 80, 80, 80))));
            drawingMenu.AddItem(new MenuItem("ERange", "E Range").SetValue(new Circle(false, Color.FromArgb(100, 80, 80, 80))));
            drawingMenu.AddItem(new MenuItem("RRange", "R Range").SetValue(new Circle(false, Color.FromArgb(100, 80, 80, 80))));

            mordeMenu.AddSubMenu(targetSelectorMenu);
            mordeMenu.AddSubMenu(comboMenu);
            mordeMenu.AddSubMenu(mixedMenu);
            mordeMenu.AddSubMenu(clearMenu);
            mordeMenu.AddSubMenu(ultimateMenu);
            mordeMenu.AddSubMenu(killstealMenu);
            mordeMenu.AddSubMenu(drawingMenu);
            mordeMenu.AddToMainMenu();
        }

        public void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 750);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 850);

            //const double ultAngle = 80 * Math.PI / 180;
            //const float fUltAngle = (float)ultAngle;

            //E.SetSkillshot(0.30f, fUltAngle, int.MaxValue, false, SkillshotType.SkillshotCone);

            //SpellList.Add(Q);
            SpellList.Add(W); 
            SpellList.Add(E);
            SpellList.Add(R);
        }

        public void OnGameUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                Program._orbwalker.SetMovement(true);
                //Obj_AI_Base qTarget = SimpleTs.GetTarget(Q.ChargedMaxRange, SimpleTs.DamageType.Magical);
                switch (Program._orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        //Combo();
                        //Game.PrintChat(ObjectManager.Player.AttackRange.ToString());
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        //Harrass();
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        //Farm();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        //Farm();
                        break;
                }
                //Killsteal();
            }
        }

        public static void OnDraw(EventArgs args)
        {
            //Utility.DrawCircle(ObjectManager.Player.Position, 125, Color.Red);
            Program.Draw(SpellList, mordeMenu);
        }
    }
}
