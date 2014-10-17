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
    class Pantheon
    {
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public static Menu panthMenu;

        public static SpellSlot IgniteSlot;

        public static bool isChanneling = false;

        public Pantheon()
        {
            //CustomEvents.Game.OnGameLoad += OnGameLoad;
            Obj_AI_Base.OnPlayAnimation += OnAnimation;

            Game.PrintChat(string.Format("<font color='#F7A100'>{0} - {1} loaded.</font>", Assembly.GetExecutingAssembly().GetName().Name, Program.ChampionName));
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Game.OnGameUpdate += OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;

            LoadSpells();
            LoadMenu();
        }

        public void LoadMenu()
        {
            panthMenu = new Menu("LittleRedSharpie: " + Program.ChampionName, Program.ChampionName, true);
            panthMenu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Program._orbwalker = new Orbwalking.Orbwalker(panthMenu.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);

            /*var comboMenu = new Menu("Combo", "combo");
            comboMenu.AddItem(new MenuItem("comboAA", "AA during combo").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboQ", "Combo Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboW", "Combo W").SetValue(false));
            comboMenu.AddItem(new MenuItem("comboE", "Combo E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboUltimate", "UseUltimate").SetValue(false));*/

            var mixedMenu = new Menu("Harras", "mixed");
            mixedMenu.AddItem(new MenuItem("hMode", "Harass Mode: ").SetValue(new StringList(new[] { "Q", "W+E" })));
            //mixedMenu.AddItem(new MenuItem("autoQ", "Auto-Q when Target in Range").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Toggle)));
            //mixedMenu.AddItem(new MenuItem("aQT", "Don't Auto-Q if in enemy Turret Range").SetValue(true));
            mixedMenu.AddItem(new MenuItem("harassMana", "Min. Mana Percent: ").SetValue(new Slider(50)));

            var clearMenu = new Menu("Farm", "clear");
            clearMenu.AddItem(new MenuItem("clearQ", "Use Q").SetValue(new StringList(new[] { "Farm/Mixed", "LaneClear", "All", "No" }, 2)));
            clearMenu.AddItem(new MenuItem("clearE", "Use E").SetValue(new StringList(new[] { "Farm/Mixed", "LaneClear", "All", "No" }, 1)));
            clearMenu.AddItem(new MenuItem("eMin", "Min. Minions in Range of E").SetValue(new Slider(2, 10, 1)));

            var ultimateMenu = new Menu("Ultimate", "ultimate");
            ultimateMenu.AddItem(new MenuItem("ultimateKey", "Ultimate on Target").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
            ultimateMenu.AddItem(new MenuItem("ultimateAuto", "Auto Ultimate").SetValue(true));
            ultimateMenu.AddItem(new MenuItem("ultimateMin", "Min. Enemies in Range").SetValue(new Slider(1, 5, 1)));

            var killstealMenu = new Menu("Killsteal", "killsteal");
            killstealMenu.AddItem(new MenuItem("killSteal", "Use Killsteal").SetValue(true));  
            killstealMenu.AddItem(new MenuItem("killstealIgnite", "Use Ignite").SetValue(true));

            var miscMenu = new Menu("Misc Settings", "misc");
            miscMenu.AddItem(new MenuItem("interrupt", "Interrupt Dangerous Spells").SetValue(true));  

            var drawingMenu = new Menu("Drawing", "drawing");
            drawingMenu.AddItem(new MenuItem("disabledraw", "Disable all drawing").SetValue(false));
            drawingMenu.AddItem(new MenuItem("QRange", "Q Range").SetValue(new Circle(false, Color.FromArgb(100, 128, 42, 42))));
            drawingMenu.AddItem(new MenuItem("WRange", "W Range").SetValue(new Circle(false, Color.FromArgb(100, 128, 42, 42))));
            drawingMenu.AddItem(new MenuItem("ERange", "E Range").SetValue(new Circle(false, Color.FromArgb(100, 128, 42, 42))));
            drawingMenu.AddItem(new MenuItem("RRangeM", "R Range on minimap").SetValue(new Circle(true, Color.FromArgb(100, 128, 42, 42))));
            //drawingMenu.AddItem(new MenuItem("RRange", "R Range").SetValue(new Circle(false, Color.FromArgb(100, 107, 142, 35))));

            panthMenu.AddSubMenu(targetSelectorMenu);
            //panthMenu.AddSubMenu(comboMenu);
            panthMenu.AddSubMenu(mixedMenu);
            panthMenu.AddSubMenu(clearMenu);
            //panthMenu.AddSubMenu(ultimateMenu);
            panthMenu.AddSubMenu(killstealMenu);
            panthMenu.AddSubMenu(miscMenu);
            panthMenu.AddSubMenu(drawingMenu);
            panthMenu.AddToMainMenu();
        }

        public void LoadSpells()
        {

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 5500);

            const double eAngle = 80 * Math.PI / 180;
            const float feAngle = (float)eAngle;

            E.SetSkillshot(0.30f, feAngle, int.MaxValue, false, SkillshotType.SkillshotCone);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
        }

        public void OnGameLoad(EventArgs args)
        {
            Game.PrintChat(string.Format("<font color='#F7A100'>{0} - {1} loaded.</font>", Assembly.GetExecutingAssembly().GetName().Name, Program.ChampionName));
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Game.OnGameUpdate += OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Drawing.OnDraw += OnDraw;
        }

        public void OnGameUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                //Program._orbwalker.SetAttacks(true);
                switch (Program._orbwalker.ActiveMode)
                    { 
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        //Game.PrintChat(panthMenu.Item("hMode").GetValue<StringList>().SelectedIndex.ToString());
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        Harass();
                        Farm();
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        Farm();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        Farm();
                        break;
                    }
                Killsteal();
            }
        }

        public static void Combo()
        {
            //if (Program._orbwalker.GetTarget() == null)
                //return;
            Obj_AI_Base target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (target != null)
            {
                //Game.PrintChat(panthMenu.Item("hMode").GetValue<StringList>().SelectedIndex.ToString());
                if (Q.IsReady() && target.IsValidTarget(Q.Range) && !isChanneling)
                    Q.CastOnUnit(target);
                if (W.IsReady() && target.IsValidTarget(Q.Range) && !isChanneling && !Q.IsReady())
                    W.CastOnUnit(target);
                if (E.IsReady() && !target.IsMoving && target.IsValidTarget(Q.Range) && !Q.IsReady() && !W.IsReady())
                    E.Cast(target.Position);
            }
        }

        public static void Harass()
        {
            //if (Program._orbwalker.GetTarget() == null)
                //return;
            Obj_AI_Base target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            //Game.PrintChat(panthMenu.Item("hMode").GetValue<StringList>().SelectedIndex.ToString());
            var mana = ObjectManager.Player.MaxMana * (panthMenu.Item("harassMana").GetValue<Slider>().Value / 100.0);

            if (target != null && ObjectManager.Player.Mana > mana)
            {
                int menuItem = panthMenu.Item("hMode").GetValue<StringList>().SelectedIndex;
                switch (menuItem)
                {
                    case 0:
                        Q.CastOnUnit(target);
                        break;
                    case 1:
                        W.CastOnUnit(target);
                        if (!target.CanMove)
                            E.Cast(target.Position);
                        break;
                }
            }
        }

        public void Farm()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);

            var useQsetting = panthMenu.Item("clearQ").GetValue<StringList>().SelectedIndex;
            var useEsetting = panthMenu.Item("clearE").GetValue<StringList>().SelectedIndex;

            var useQ = (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && (useQsetting == 1 || useQsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && (useQsetting == 0 || useQsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && (useQsetting == 0 || useQsetting == 2));
            var useE = (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && (useEsetting == 1 || useEsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && (useEsetting == 0 || useEsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && (useEsetting == 0 || useEsetting == 2));

            if (useQ && Q.IsReady())
            {
                foreach (var minion in minions)
                {
                    var qDamage = (minion.Health < minion.MaxHealth * 0.15) ? ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) * 2 : ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q);
                    if (qDamage > 1.2 * minion.Health)
                    {
                        Q.CastOnUnit(minion);
                    }
                }
            }

            if (useE && E.IsReady())
            {
                foreach (var minion in minions)
                {
                    var eDamage = ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E);
                    if (E.CastIfWillHit(minion, panthMenu.Item("eMin").GetValue<Slider>().Value - 1))
                    {
                        if (useEsetting == 1)
                        {
                            if (eDamage > 1.2 * minion.Health)
                            {
                                E.Cast(minion.Position);
                            }
                        }
                        else if ( useEsetting == 2)
                        {
                            E.Cast(minion.Position);
                        }
                    }
                }
            }
        }

        public void Killsteal()
        {
            if (!panthMenu.Item("killSteal").GetValue<bool>())
            {
                return;
            }
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero != null && hero.IsValidTarget() && hero.IsEnemy)
                {
                    var qDamage = (hero.Health <= hero.MaxHealth * 0.15) ? (ObjectManager.Player.GetSpellDamage(hero, SpellSlot.Q) * 2) : ObjectManager.Player.GetSpellDamage(hero, SpellSlot.Q);
                    var wDamage = ObjectManager.Player.GetSpellDamage(hero, SpellSlot.W);
                    var eDamage = ObjectManager.Player.GetSpellDamage(hero, SpellSlot.E) * 3;

                    if (hero.Health < qDamage && Q.IsReady() && Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < Q.Range)
                        Q.CastOnUnit(hero);
                    else if (hero.Health < wDamage && W.IsReady() && Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < W.Range)
                        W.CastOnUnit(hero);
                    else if (hero.Health < eDamage && E.IsReady() && Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < E.Range)
                        E.Cast(hero.Position);
                    else if (hero.Health < qDamage + wDamage && Q.IsReady() && W.IsReady() && Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < Q.Range)
                        W.CastOnUnit(hero);
                    else if (hero.Health < qDamage + eDamage && Q.IsReady() && E.IsReady() && Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < Q.Range)
                        Q.CastOnUnit(hero);
                    else if (hero.Health < wDamage + eDamage && W.IsReady() && E.IsReady() && Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < W.Range)
                        W.CastOnUnit(hero);
                    else if (hero.Health < qDamage + wDamage + eDamage && Q.IsReady() && W.IsReady() && E.IsReady() && Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < Q.Range)
                        Q.CastOnUnit(hero);
                    if (panthMenu.Item("killstealIgnite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown)
                    {
                        double dmgIgnite = ObjectManager.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
                        if (Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) > E.Range && dmgIgnite > hero.Health && ObjectManager.Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                        {
                            ObjectManager.Player.SummonerSpellbook.CastSpell(IgniteSlot, hero);
                        }
                    }
                }
            }
        }

        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (panthMenu.Item("stopChannel").GetValue<bool>())
            {
                String[] interruptingSpells = {
                    "AbsoluteZero",
                    "AlZaharNetherGrasp", 
		            "CaitlynAceintheHole", 
		            "Crowstorm", 
		            "DrainChannel", 
		            "FallenOne", 
		            "GalioIdolOfDurand", 
		            "InfiniteDuress", 
		            "KatarinaR", 
		            "MissFortuneBulletTime", 
		            "Teleport", 
		            "Pantheon_GrandSkyfall_Jump", 
		            "ShenStandUnited", 
		            "UrgotSwap2"
                };

                foreach (string interruptingSpellName in interruptingSpells)
                {
                    if (unit.Team != ObjectManager.Player.Team && args.SData.Name == interruptingSpellName)
                    {
                        if (ObjectManager.Player.Distance(unit) <= W.Range && W.IsReady())
                            W.CastOnUnit(unit);
                    }
                }
            }
        }

        private void OnAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                //Game.PrintChat(args.Animation.ToString());
                if (args.Animation.ToLower().IndexOf("spell3") > -1 || args.Animation.ToLower().IndexOf("ult") > -1)
                {
                    Program._orbwalker.SetAttack(false);
                    Program._orbwalker.SetMovement(false);
                    isChanneling = true;
                }
                else
                {
                    Program._orbwalker.SetAttack(true);
                    Program._orbwalker.SetMovement(true);
                    isChanneling = false;
                }
            }
        }

        public static void OnDraw(EventArgs args)
        {
            Program.Draw(SpellList, panthMenu);
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (panthMenu.Item("disabledraw").GetValue<bool>()) { return; }
            var menuItem = panthMenu.Item(R.Slot + "RangeM").GetValue<Circle>();
            if (menuItem.Active)
                Utility.DrawCircle(ObjectManager.Player.Position, R.Range, R.IsReady() ? Color.FromArgb(100, 128, 42, 42) : Color.Red, 1, 30, true);
        }

    }
}
