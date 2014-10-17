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
    class Cassiopeia
    {
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public static Menu cassMenu;

        public static SpellSlot IgniteSlot;

        public Cassiopeia()
        {
            //CustomEvents.Game.OnGameLoad += OnGameLoad;
            Obj_AI_Base.OnPlayAnimation += OnAnimation;

            Game.PrintChat(string.Format("<font color='#F7A100'>{0} - {1} loaded.</font>", Assembly.GetExecutingAssembly().GetName().Name, Program.ChampionName));
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;

            

            LoadSpells();
            LoadMenu();
        }

        public void LoadMenu()
        {
            cassMenu = new Menu("LittleRedSharpie: " + Program.ChampionName, Program.ChampionName, true);
            cassMenu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Program._orbwalker = new Orbwalking.Orbwalker(cassMenu.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);

            var comboMenu = new Menu("Combo", "combo");
            comboMenu.AddItem(new MenuItem("comboAA", "AA during combo").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboQ", "Combo Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboW", "Combo W").SetValue(false));
            comboMenu.AddItem(new MenuItem("comboE", "Combo E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboUltimate", "UseUltimate").SetValue(false));

            var mixedMenu = new Menu("Harras", "mixed");
            mixedMenu.AddItem(new MenuItem("mixedstationaryQ", "Q only on enemy action").SetValue(false));
            mixedMenu.AddItem(new MenuItem("mixedQ", "Harass Q").SetValue(true));
            mixedMenu.AddItem(new MenuItem("mixedW", "Harass W").SetValue(false));
            mixedMenu.AddItem(new MenuItem("mixedE", "Harass E").SetValue(true));
            mixedMenu.AddItem(new MenuItem("mixedWfarm", "W Farm").SetValue(false));
            mixedMenu.AddItem(new MenuItem("mixedEfarm", "E Farm").SetValue(true));

            var clearMenu = new Menu("Farm", "clear");
            clearMenu.AddItem(new MenuItem("clearQ", "Use Q").SetValue(new StringList(new[] { "Farm", "LaneClear", "Both", "No" }, 2)));
            clearMenu.AddItem(new MenuItem("clearW", "Use W").SetValue(new StringList(new[] { "Farm", "LaneClear", "Both", "No" }, 1)));
            clearMenu.AddItem(new MenuItem("clearE", "Use E").SetValue(new StringList(new[] { "Farm", "LaneClear", "Both", "No" }, 2)));
            clearMenu.AddItem(new MenuItem("clearpoisonE", "E only on poisoned").SetValue(true));
            clearMenu.AddItem(new MenuItem("clearlasthitE", "E only for last hit").SetValue(true));

            var ultimateMenu = new Menu("Ultimate", "ultimate");
            ultimateMenu.AddItem(new MenuItem("ultimateKey", "Ultimate on Target").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
            ultimateMenu.AddItem(new MenuItem("ultimateAuto", "Auto Ultimate").SetValue(true));
            ultimateMenu.AddItem(new MenuItem("ultimateMin", "Min. Enemies in Range").SetValue(new Slider(1, 5, 1)));

            var killstealMenu = new Menu("Killsteal", "killsteal");
            killstealMenu.AddItem(new MenuItem("killstealEnabled", "Enabled").SetValue(true));
            killstealMenu.AddItem(new MenuItem("killstealE", "Use E").SetValue(true)); 
            killstealMenu.AddItem(new MenuItem("killstealIgnite", "Use Ignite").SetValue(true));

            var drawingMenu = new Menu("Drawing", "drawing");
            drawingMenu.AddItem(new MenuItem("disabledraw", "Disable all drawing").SetValue(false));
            drawingMenu.AddItem(new MenuItem("QRange", "Q Range").SetValue(new Circle(false, Color.FromArgb(100, 107, 142, 35))));
            drawingMenu.AddItem(new MenuItem("WRange", "W Range").SetValue(new Circle(false, Color.FromArgb(100, 107, 142, 35))));
            drawingMenu.AddItem(new MenuItem("ERange", "E Range").SetValue(new Circle(false, Color.FromArgb(100, 107, 142, 35))));
            drawingMenu.AddItem(new MenuItem("RRange", "R Range").SetValue(new Circle(false, Color.FromArgb(100, 107, 142, 35))));

            cassMenu.AddSubMenu(targetSelectorMenu);
            cassMenu.AddSubMenu(comboMenu);
            cassMenu.AddSubMenu(mixedMenu);
            cassMenu.AddSubMenu(clearMenu);
            cassMenu.AddSubMenu(ultimateMenu);
            cassMenu.AddSubMenu(killstealMenu);
            cassMenu.AddSubMenu(drawingMenu);
            cassMenu.AddToMainMenu();
        }

        public void LoadSpells()
        {

            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 850);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 825);

            const double ultAngle = 80 * Math.PI / 180;
            const float fUltAngle = (float)ultAngle;

            Q.SetSkillshot(0.60f, 75f, int.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.50f, 106f, 2500f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.30f, fUltAngle, int.MaxValue, false, SkillshotType.SkillshotCone);

            SpellList.Add(Q);
            SpellList.Add(W); 
            SpellList.Add(E);
            SpellList.Add(R);
        }

        public void OnGameLoad(EventArgs args)
        {
            Game.PrintChat(string.Format("<font color='#F7A100'>{0} - {1} loaded.</font>", Assembly.GetExecutingAssembly().GetName().Name, Program.ChampionName));
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
        }

        public void OnGameUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                Program._orbwalker.SetAttack(true);
                switch (Program._orbwalker.ActiveMode)
                    {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        Harrass();
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
                Ultimate();
            }
        }

        public void Combo()
        {
            //if (Program._orbwalker.GetTarget() == null)
                //return;
            Obj_AI_Base target = SimpleTs.GetTarget(Q.Range + (Q.Width / 2), SimpleTs.DamageType.Magical);
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !target.IsValidTarget() || target.GetType() != typeof(Obj_AI_Hero))
            {
                return;
            }
            if (cassMenu.Item("comboAA").GetValue<bool>())
            {
                Program._orbwalker.SetAttack(true);
            }
            else
            {
                Program._orbwalker.SetAttack(false);
            }
            if (cassMenu.Item("comboUltimate").GetValue<bool>() && target.IsValidTarget(R.Range))
            {
                var prediction = R.GetPrediction(target);
                //if (prediction.AoeTargetsHitCount >= cassMenu.Item("ultimateMin").GetValue<Slider>().Value)
                //{
                    R.Cast(prediction.CastPosition);
                //}
            }
            if (!HasPoisonBuff(target) && cassMenu.Item("comboQ").GetValue<bool>())
            {
                Program.castSpell(target, Q, false);
            }
            else if (cassMenu.Item("comboQ").GetValue<bool>())
            {
                //BuffInstance buff = GetPoisonBuff(target);
                //if (buff.EndTime - Game.Time <= (3 / 2))
                {
                    Program.castSpell(target, Q, false);
                }
            }
            if (cassMenu.Item("comboW").GetValue<bool>())
            {
                Program.castSpell(target, W, false);
            }
            if (HasPoisonBuff(target) && cassMenu.Item("comboE").GetValue<bool>())
            {
                //BuffInstance buff = GetPoisonBuff(target);
                //if (buff.EndTime - Game.Time > (3 / 6))
                {
                    Program.castSpell(target,E,true);
                }
            }
        }

        public void Harrass()
        {
            //if (Program._orbwalker.GetTarget() == null)
                //return;
            Obj_AI_Base target = SimpleTs.GetTarget(Q.Range + (Q.Width / 2), SimpleTs.DamageType.Magical);
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed || !target.IsValidTarget() || target.GetType() != typeof(Obj_AI_Hero))
            {
                return;
            }
            Program._orbwalker.SetAttack(false);
            if (!HasPoisonBuff(target) && cassMenu.Item("mixedQ").GetValue<bool>() && !cassMenu.Item("mixedstationaryQ").GetValue<bool>())
            {
                Program.castSpell(target, Q, false);
            }
            else if (cassMenu.Item("mixedQ").GetValue<bool>() && !cassMenu.Item("mixedstationaryQ").GetValue<bool>())
            {
                //BuffInstance buff = GetPoisonBuff(target);
                //if (buff.EndTime - Game.Time <= (3 / 2))
                //{
                Program.castSpell(target, Q, false);
                //}
            }
            if (cassMenu.Item("mixedW").GetValue<bool>())
            {
                Program.castSpell(target, W, false);
            }
            if (HasPoisonBuff(target) && cassMenu.Item("mixedE").GetValue<bool>())
            {
                //BuffInstance buff = GetPoisonBuff(target);
                //if (buff.EndTime - Game.Time > (3 / 6))
                //{
                    //CastE(target);
                    Program.castSpell(target, E, true);
                //}
            }
        }

        public void Farm()
        {
            var rangedMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width,MinionTypes.Ranged);
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width,MinionTypes.All);
            var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width,MinionTypes.Ranged);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width, MinionTypes.All);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All);

            var useQsetting = cassMenu.Item("clearQ").GetValue<StringList>().SelectedIndex;
            var useWsetting = cassMenu.Item("clearW").GetValue<StringList>().SelectedIndex;
            var useEsetting = cassMenu.Item("clearE").GetValue<StringList>().SelectedIndex;

            var useQ = (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && (useQsetting == 1 || useQsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && (useQsetting == 0 || useQsetting == 2));
            var useW = (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && (useWsetting == 1 || useWsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && (useWsetting == 0 || useWsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && cassMenu.Item("mixedWfarm").GetValue<bool>());
            var useE = (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && (useEsetting == 1 || useEsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && (useEsetting == 0 || useEsetting == 2)) || (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && cassMenu.Item("mixedEfarm").GetValue<bool>());

            if (useQ && Q.IsReady())
            {
                var farmLocation1 = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);
                var farmLocation2 = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);

                if (farmLocation1.MinionsHit >= 2)
                {
                    Q.Cast(farmLocation1.Position);
                }
                else if (farmLocation2.MinionsHit >= 1 || allMinionsQ.Count == 1)
                {
                    Q.Cast(farmLocation2.Position);
                }
            }
            if (useW && W.IsReady())
            {
                var farmLocation1 = W.GetCircularFarmLocation(allMinionsW, W.Width);
                var farmLocation2 = W.GetCircularFarmLocation(allMinionsW, W.Width);

                if (farmLocation1.MinionsHit >= 2)
                {
                    W.Cast(farmLocation1.Position);
                }
                else if (farmLocation2.MinionsHit >= 1 || allMinionsW.Count == 1)
                {
                    W.Cast(farmLocation2.Position);
                }
            }
            if (useE && E.IsReady())
            {
                foreach (var minion in allMinionsE)
                {
                    //double damage = DamageLib.CalcMagicMinionDmg((30 + 25 * (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level)) + (((35 + 5 * (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level))/100)*ObjectManager.Player.FlatMagicDamageMod), minion as Obj_AI_Minion, true);
                    //double damage = DamageLib.getDmg(minion, DamageLib.SpellType.E);
                    double damage = ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E);
                    //Game.PrintChat(minion.Health.ToString());
                    //Game.PrintChat(damage.ToString());
                    BuffInstance buff = GetPoisonBuff(minion);
                    if (cassMenu.Item("clearlasthitE").GetValue<bool>())
                    {
                        if (damage > 1.2 * minion.Health)// && buff.EndTime - Game.Time > (3 / 6)) clearlasthitE
                        {
                            if (cassMenu.Item("clearpoisonE").GetValue<bool>())
                            {
                                if (HasPoisonBuff(minion))
                                {
                                    Program.castSpell(minion, E, true);
                                }
                            }
                            else
                            {
                                Program.castSpell(minion, E, true);
                            }
                        }
                    }
                    else
                    {
                        if (cassMenu.Item("clearpoisonE").GetValue<bool>())
                        {
                            if (HasPoisonBuff(minion))
                            {
                                Program.castSpell(minion, E, true);
                            }
                        }
                        else
                        {
                            Program.castSpell(minion, E, true);
                        }
                    }
                    
                }
            }
        }



        private void OnAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe || !cassMenu.Item("mixedstationaryQ").GetValue<bool>() || sender.GetType() != typeof(Obj_AI_Hero) || Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed || !(sender is Obj_AI_Base))
                return;
            Obj_AI_Base test = (Obj_AI_Base)sender;
            if ((args.Animation.ToLower().IndexOf("spell") > -1 || args.Animation.ToLower().IndexOf("attack") > -1 || args.Animation.ToLower().IndexOf("idle") > -1) && test.IsValidTarget(Q.Range))
            {
                Program.castSpell(test, Q, false);
            }
        }

        private void Killsteal()
        {
            if (!cassMenu.Item("killstealEnabled").GetValue<bool>())
            {
                return;
            }
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero != null && hero.IsValidTarget(Q.Range + Q.Width) && hero.IsEnemy)
                {
                    if (cassMenu.Item("killstealE").GetValue<bool>())
                    {
                        double dmgE = ObjectManager.Player.GetSpellDamage(hero, SpellSlot.E);
                        if (Vector3.Distance(ObjectManager.Player.Position, hero.ServerPosition) > E.Range && dmgE * 1.2 > hero.Health)
                        {
                            Program.castSpell(hero, E, true);
                        }
                    }
                    if (cassMenu.Item("killstealIgnite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown)
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

        public bool HasPoisonBuff(Obj_AI_Base target)
        {
            return target.Buffs.Any(buff => buff.Type == BuffType.Poison);
        }

        private BuffInstance GetPoisonBuff(Obj_AI_Base target)
        {
            return target.Buffs.FirstOrDefault(buff => buff.Type == BuffType.Poison);
        }

        public void Ultimate()
        {
            if (Program._orbwalker.GetTarget() == null)
                return;
            Obj_AI_Base target = Program._orbwalker.GetTarget();
            if (ObjectManager.Player.Spellbook.CanUseSpell(R.Slot) != SpellState.Ready || Vector3.Distance(ObjectManager.Player.Position, target.ServerPosition) > R.Range || !target.IsValidTarget() || target.GetType() != typeof(Obj_AI_Hero))
            {
                return;
            }
            if (cassMenu.Item("ultimateKey").GetValue<KeyBind>().Active && target.IsValidTarget(R.Range))
            {
                var prediction = R.GetPrediction(target);
                R.Cast(prediction.CastPosition);
            }
            if (cassMenu.Item("ultimateAuto").GetValue<bool>())
            {
                
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
                {
                    if (enemy.IsValidTarget(R.Range))
                    {
                        var prediction = R.GetPrediction(enemy, true);
                        //Game.PrintChat(prediction.AoeTargetsHitCount.ToString());
                        //if (prediction.AoeTargetsHitCount >= cassMenu.Item("ultimateMin").GetValue<Slider>().Value)
                        //{
                            //R.(prediction.CastPosition);
                        //ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Distance(prediction.CastPosition) <= R.Width);
                        //objectmanager.Get<Obj_Ai_hero>().Where (enemy => enemy.distande(spellcastposition) <= width
                        R.CastIfWillHit(target, cassMenu.Item("ultimateMin").GetValue<Slider>().Value - 1);
                        //}
                    }
                }
            }
        }

        public static void OnDraw(EventArgs args)
        {
            Program.Draw(SpellList, cassMenu);
        }
    }
}
