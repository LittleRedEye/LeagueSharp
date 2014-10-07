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
    class shitclasscuzimlazyasfuck
    {
        public int id { get; set; }
        public int stacks { get; set; }
    }
    class Varus
    {
        public static List<Spell> SpellList = new List<Spell>();
        public static List<shitclasscuzimlazyasfuck> stackList = new List<shitclasscuzimlazyasfuck>();
        public static Spell Q;
        public static Spell E;
        public static Spell R;

        public static Menu varusMenu;
        public static SpellSlot IgniteSlot;

        public Varus()
        {
            Game.PrintChat(string.Format("<font color='#F7A100'>{0} - {1} loaded.</font>", Assembly.GetExecutingAssembly().GetName().Name, Program.ChampionName));
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Game.OnGameUpdate += OnGameUpdate;
            GameObject.OnCreate += OnCreate;
            Drawing.OnDraw += OnDraw;
            //Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

            LoadSpells();
            LoadMenu();
        }

        public void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                Game.PrintChat("Spell name: " + args.SData.Name.ToString());
            }
        }

        public void LoadMenu()
        {
            varusMenu = new Menu("LittleRedSharpie: " + Program.ChampionName, Program.ChampionName, true);
            varusMenu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Program._orbwalker = new Orbwalking.Orbwalker(varusMenu.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);

            var comboMenu = new Menu("Combo", "combo");
            comboMenu.AddItem(new MenuItem("comboQdetonate", "Q only on 'x' stacks").SetValue(new Slider(3, 0, 3)));
            comboMenu.AddItem(new MenuItem("comboEafterQ", "Only use E if Q on cooldown").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboEdetonate", "W only on 'x' stacks").SetValue(new Slider(3, 0, 3)));
            comboMenu.AddItem(new MenuItem("comboEslow", "Slow with E if out of range").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboUltimate", "UseUltimate").SetValue(false));

            var mixedMenu = new Menu("Harras", "mixed");
            mixedMenu.AddItem(new MenuItem("mixedQdetonate", "Q only on 'x' stacks").SetValue(new Slider(0, 0, 3)));
            mixedMenu.AddItem(new MenuItem("mixedQ", "Harass Q").SetValue(true));
            mixedMenu.AddItem(new MenuItem("mixedEdetonate", "W only on 'x' stacks").SetValue(new Slider(0, 0, 3)));
            mixedMenu.AddItem(new MenuItem("mixedE", "Harass E").SetValue(false));

            var clearMenu = new Menu("Farm", "clear");
            clearMenu.AddItem(new MenuItem("clearQ", "Use Q").SetValue(new StringList(new[] { "Farm", "LaneClear", "Both", "No" }, 2)));
            clearMenu.AddItem(new MenuItem("clearE", "Use E").SetValue(new StringList(new[] { "Farm", "LaneClear", "Both", "No" }, 1)));

            var killstealMenu = new Menu("Killsteal", "killsteal");
            killstealMenu.AddItem(new MenuItem("killstealEnabled", "Enabled").SetValue(true));
            killstealMenu.AddItem(new MenuItem("killstealQ", "Use Q").SetValue(true));
            killstealMenu.AddItem(new MenuItem("killstealE", "Use E").SetValue(true));
            killstealMenu.AddItem(new MenuItem("killstealR", "Use R").SetValue(false));
            killstealMenu.AddItem(new MenuItem("killstealIgnite", "Use Ignite").SetValue(true));

            var drawingMenu = new Menu("Drawing", "drawing");
            drawingMenu.AddItem(new MenuItem("disabledraw", "Disable all drawing").SetValue(false));
            drawingMenu.AddItem(new MenuItem("QRange", "Q Range").SetValue(new Circle(false, Color.FromArgb(100, 73, 66, 153))));
            drawingMenu.AddItem(new MenuItem("ERange", "E Range").SetValue(new Circle(false, Color.FromArgb(100, 73, 66, 153))));
            drawingMenu.AddItem(new MenuItem("RRange", "R Range").SetValue(new Circle(false, Color.FromArgb(100, 73, 66, 153))));

            varusMenu.AddSubMenu(targetSelectorMenu);
            varusMenu.AddSubMenu(comboMenu);
            varusMenu.AddSubMenu(mixedMenu);
            varusMenu.AddSubMenu(clearMenu);
            //varusMenu.AddSubMenu(ultimateMenu);
            varusMenu.AddSubMenu(killstealMenu);
            varusMenu.AddSubMenu(drawingMenu);
            varusMenu.AddToMainMenu();
        }

        public void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 1550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 1075);
            //Rjump - 550
            Q.SetSkillshot(0.0f, 60f, 1850f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 275f, 1500f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 80f, 1950f, true, SkillshotType.SkillshotLine);

            Q.SetCharged("VarusQ", "VarusQ", 1075, 1550, 1.15f);
             
            SpellList.Add(Q);
            SpellList.Add(E);
            SpellList.Add(R);
        }

        public void OnGameUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                Program._orbwalker.SetMovement(true);
                Obj_AI_Base qTarget = SimpleTs.GetTarget(Q.ChargedMaxRange, SimpleTs.DamageType.Magical);
                switch (Program._orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        Harrass();
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

        public static void stackccheck()
        {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                int indx = stackList.FindIndex(p => p.id == hero.NetworkId);
                if (indx < 0)
                {
                    stackList.Add(new shitclasscuzimlazyasfuck() { id = hero.NetworkId, stacks = 0 });
                }
                else if (hero != null && hero.IsValidTarget() && hero.IsEnemy && !hero.HasBuff("varuswdebuff", true))
                {
                    stackList[indx].stacks = 0;
                }
            }
        }

        public static void Combo()
        {
            Obj_AI_Base qTarget = SimpleTs.GetTarget(Q.ChargedMaxRange, SimpleTs.DamageType.Magical);
            if (qTarget == null) { return; }
            stackccheck();
            if (Q.IsReady() && stackList[stackList.FindIndex(x => x.id == qTarget.NetworkId)].stacks >= varusMenu.Item("comboQdetonate").GetValue<Slider>().Value)
            {
                if (Q.IsCharging)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    var prediction = Q.GetPrediction(qTarget);
                    Q.Cast(qTarget, false, false);
                    return;
                }
                else if (qTarget != null)
                {
                    Q.StartCharging();
                }
            }
            if (E.IsReady() && (stackList[stackList.FindIndex(x => x.id == qTarget.NetworkId)].stacks >= varusMenu.Item("comboEdetonate").GetValue<Slider>().Value || (Program._orbwalker.GetTarget() == null && varusMenu.Item("comboEslow").GetValue<bool>())))
            {
                if (varusMenu.Item("comboEafterQ").GetValue<bool>() && Q.IsReady())
                {
                    return;
                }
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                var prediction = E.GetPrediction(qTarget);
                E.Cast(prediction.CastPosition);
            }
        }

        public static void Harrass()
        {
            Obj_AI_Base qTarget = SimpleTs.GetTarget(Q.ChargedMaxRange, SimpleTs.DamageType.Magical);
            if (qTarget == null) { return; }
            stackccheck();
            if (Q.IsReady() && stackList[stackList.FindIndex(x => x.id == qTarget.NetworkId)].stacks >= varusMenu.Item("mixedQdetonate").GetValue<Slider>().Value && varusMenu.Item("mixedQ").GetValue<bool>())
            {
                if (Q.IsCharging)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    var prediction = Q.GetPrediction(qTarget);
                    Q.Cast(qTarget, false, false);
                    return;
                }
                else if (qTarget != null)
                {
                    Q.StartCharging();
                }
            }
            if (E.IsReady() && stackList[stackList.FindIndex(x => x.id == qTarget.NetworkId)].stacks >= varusMenu.Item("mixedEdetonate").GetValue<Slider>().Value && varusMenu.Item("mixedE").GetValue<bool>())
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                var prediction = E.GetPrediction(qTarget);
                E.Cast(prediction.CastPosition);
            }
        }

        public static void Farm()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All);
            var rangedMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width, MinionTypes.Ranged);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width, MinionTypes.All);

            var useQsetting = varusMenu.Item("clearQ").GetValue<StringList>().SelectedIndex;
            var useEsetting = varusMenu.Item("clearE").GetValue<StringList>().SelectedIndex;

            var useQ = (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && (useQsetting == 1 || useQsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && (useQsetting == 0 || useQsetting == 2));
            var useE = (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && (useEsetting == 1 || useEsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && (useEsetting == 0 || useEsetting == 2));

            if (useQ && Q.IsReady())
            {
                if (Q.IsCharging)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    var locQ = Q.GetLineFarmLocation(allMinionsQ);
                    if (allMinionsQ.Count == allMinionsQ.Count(m => ObjectManager.Player.Distance(m) < Q.Range) && locQ.MinionsHit > 0 && locQ.Position.IsValid())
                        Q.Cast(locQ.Position);
                }
                else if (allMinionsQ.Count > 0)
                    Q.StartCharging();
            }

            if (useE && E.IsReady())
            {
                if (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var fl1 = Q.GetCircularFarmLocation(rangedMinionsE, E.Width);
                    var fl2 = Q.GetCircularFarmLocation(allMinionsE, E.Width);

                    if (fl1.MinionsHit >= 3)
                    {
                        E.Cast(fl1.Position);
                    }

                    else if (fl2.MinionsHit >= 2 || allMinionsE.Count == 1)
                    {
                        E.Cast(fl2.Position);
                    }
                }
                else
                {
                    foreach (var minion in allMinionsE)
                    {
                        if (!Orbwalking.InAutoAttackRange(minion) && minion.Health < 0.75 * ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E))
                        {
                            E.Cast(minion);
                        }
                    }
                }  
            }
        }

        public static void Killsteal()
        {
            if (!varusMenu.Item("killstealEnabled").GetValue<bool>())
            {
                return;
            }
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero != null && hero.IsEnemy)
                {
                    double qDamage = 0;
                    double eDamage = 0;
                    double rDamage = 0;
                    double dmgIgnite = 0;

                    double wPercent = 0.0;
                    switch (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level)
                    {
                        case 1:
                            wPercent = 0.02;
                            break;
                        case 2:
                            wPercent = 0.0275;
                            break;
                        case 3:
                            wPercent = 0.035;
                            break;
                        case 4:
                            wPercent = 0.0425;
                            break;
                        case 5:
                            wPercent = 0.05;
                            break;
                        default:
                            break;
                    }
                    var stacks = stackList[stackList.FindIndex(x => x.id == hero.NetworkId)].stacks;
                    var wDamage = stacks * wPercent * hero.MaxHealth;
                    qDamage = ObjectManager.Player.GetSpellDamage(hero, SpellSlot.Q) + wDamage;
                    eDamage = ObjectManager.Player.GetSpellDamage(hero, SpellSlot.E) + wDamage;
                    rDamage = ObjectManager.Player.GetSpellDamage(hero, SpellSlot.R) + wDamage;
                    dmgIgnite = ObjectManager.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);

                    if (Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < Q.ChargedMaxRange && !hero.IsDead && hero.Health < qDamage)
                    {
                        if (Q.IsCharging)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                            Q.Cast(hero, false, false);
                            return;
                        }
                        else if (hero != null)
                        {
                            Q.StartCharging();
                        }
                    }
                    if (Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < E.Range && !hero.IsDead && hero.Health < eDamage)
                    {
                        var prediction = E.GetPrediction(hero);
                        E.Cast(prediction.CastPosition);
                    }
                    if (Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) < R.Range && !hero.IsDead && hero.Health < rDamage)
                    {
                        var prediction = R.GetPrediction(hero);
                        R.Cast(prediction.CastPosition);
                    }
                    if (hero.IsValidTarget(600) && !hero.IsDead && hero.Health < dmgIgnite && IgniteSlot != SpellSlot.Unknown)
                    {
                        ObjectManager.Player.SummonerSpellbook.CastSpell(IgniteSlot, hero);
                    }
                }
            }
        }

        private void OnCreate(LeagueSharp.GameObject value0, System.EventArgs value1)
        {
            if (value0.Name.ToLower().IndexOf("varusw") == -1) { return; }
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero != null && hero.IsValidTarget() && hero.IsEnemy && !hero.IsMinion)
                {
                    if (value0.Name.ToLower().IndexOf("01") > -1)
                    {
                        stackList[stackList.FindIndex(x => x.id == hero.NetworkId)].stacks = 1;
                    }
                    if (value0.Name.ToLower().IndexOf("02") > -1)
                    {
                        stackList[stackList.FindIndex(x => x.id == hero.NetworkId)].stacks = 2;
                    }
                    if (value0.Name.ToLower().IndexOf("03") > -1)
                    {
                        stackList[stackList.FindIndex(x => x.id == hero.NetworkId)].stacks = 3;
                    }
                }
            }
        }

        public static void OnDraw(EventArgs args)
        {
            Program.Draw(SpellList, varusMenu);
        }
    }
}
