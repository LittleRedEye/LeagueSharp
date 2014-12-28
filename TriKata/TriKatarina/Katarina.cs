using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TriKatarina.Logic;
using TriKatarina.Logic.Thoughts;
using Triton;
using Triton.Constants;
using Triton.Logic;
using Triton.Plugins;

namespace TriKatarina
{
    class Katarina : ChampionPluginBase
    {

        private int _wardJumpRange = 600;
        private Brain _brain;
        private static Katarina _instance;
        private ThoughtContext _thoughtContext;

        public static Katarina Instance
        {
            get { return _instance ?? (_instance = new Katarina()); }
        }

        public void Run()
        {
        }

        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;
            
            _thoughtContext = new ThoughtContext() {Plugin = this};

            _brain = new Brain();

            _brain.Thoughts.Add(new AnalyzeThought());
            _brain.Thoughts.Add(new AcquireTargetThought());
            _brain.Thoughts.Add(new MoveToMouseThought());
            _brain.Thoughts.Add(new KillStealThought());
            _brain.Thoughts.Add(new WardJumpThought());
            _brain.Thoughts.Add(new FullComboThought());
            _brain.Thoughts.Add(new HarassThought());
            _brain.Thoughts.Add(new FarmThought());

            return true;
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
            
            _config.AddSubMenu(new Menu("Orbwalking", "Orbwalking", false));

            Menu comboMenu = new Menu("Combo Settings", "Combo");
            comboMenu.AddItem(new MenuItem("ComboKey", "Full Combo Key (SBTW)").SetValue(new KeyBind(32, KeyBindType.Press)));
            comboMenu.AddItem(new MenuItem("StopUlt", "Stop ulti if target can die").SetValue(true));
            comboMenu.AddItem(new MenuItem("AutoE", "Auto E if not in Ulti Range").SetValue(false));
            comboMenu.AddItem(new MenuItem("ComboDetonateQ", "Try to proc Q mark").SetValue(false));
            comboMenu.AddItem(new MenuItem("ComboItems", "Use items with burst").SetValue(false));
            comboMenu.AddItem(new MenuItem("ComboMoveToMouse", "Move to mouse").SetValue(true));

            Menu harassMenu = new Menu("Harass Settings", "Harass");
            harassMenu.AddItem(new MenuItem("HarassKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
            harassMenu.AddItem(new MenuItem("HarrassQWE", "Harass Mode").SetValue(new StringList(new string[] { "Q+W+E", "Q+W" })));
            harassMenu.AddItem(new MenuItem("HarassDetonateQ", "Try to proc Q mark").SetValue(true));
            harassMenu.AddItem(new MenuItem("WHarass", "Always W ").SetValue(true));
            harassMenu.AddItem(new MenuItem("HarassMoveToMouse", "Move to mouse").SetValue(true));

            Menu drawMenu = new Menu("Draw Settings", "Drawing");
            drawMenu.AddItem(new MenuItem("DisableAllDrawing", "Disable drawing").SetValue(false));
            drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q range").SetValue(true));
            drawMenu.AddItem(new MenuItem("DrawW", "Draw W range").SetValue(false));
            drawMenu.AddItem(new MenuItem("DrawE", "Draw E range").SetValue(false));
            drawMenu.AddItem(new MenuItem("DrawKill", "Draw kill text").SetValue(true));

            Menu farmMenu = new Menu("Farm Settings", "Farming");
            farmMenu.AddItem(new MenuItem("FarmKey", "Farm Key").SetValue(new KeyBind('X', KeyBindType.Press)));
            farmMenu.AddItem(new MenuItem("QFarm", "Farm with Q").SetValue(true));
            farmMenu.AddItem(new MenuItem("WFarm", "Farm with W").SetValue(true));
            farmMenu.AddItem(new MenuItem("EFarm", "Farm with E").SetValue(false));
            farmMenu.AddItem(new MenuItem("FarmMoveToMouse", "Move to mouse while farming").SetValue(false));

            Menu ksMenu = new Menu("Kill Steal Settings", "KillStealing");
            ksMenu.AddItem(new MenuItem("KillSteal", "Enabled").SetValue(true));
            ksMenu.AddItem(new MenuItem("KsUseUlt", "Use Ult").SetValue(true));
            ksMenu.AddItem(new MenuItem("KsUseItems", "Use Items").SetValue(true));

            Menu miscMenu = new Menu("Misc Settings", "Misc");
            miscMenu.AddItem(new MenuItem("WardJumpKey", "Ward Jump Key").SetValue(new KeyBind('G', KeyBindType.Press)));
            miscMenu.AddItem(new MenuItem("packets", "Use Packets").SetValue(true));

            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            Menu targetSelectorMenu = new Menu("Target Selector", "TargetSelector", false);
            TargetSelector.AddToMenu(targetSelectorMenu);

            _config.AddSubMenu(targetSelectorMenu);

            _config.AddSubMenu(comboMenu);
            _config.AddSubMenu(harassMenu);
            _config.AddSubMenu(farmMenu);
            _config.AddSubMenu(ksMenu);
            _config.AddSubMenu(miscMenu);
            _config.AddSubMenu(drawMenu);

            _config.AddToMainMenu();
        }

        public override void SetupSpells()
        {
            RegisterSpell("Q", new Spell(SpellSlot.Q, 675f));
            RegisterSpell("W", new Spell(SpellSlot.W, 375f));
            RegisterSpell("E", new Spell(SpellSlot.E, 700f));
            RegisterSpell("R", new Spell(SpellSlot.R, 550f));

            Q.SetTargetted(400, 1400);
        }

        public override void OnGameUpdate(EventArgs args)
        {
            _brain.Think(_thoughtContext);
        }

        public override void OnPacketSend(GamePacketEventArgs args)
        {
            if (_thoughtContext.CastingUlt && !Config.Item("WardJumpKey").GetValue<KeyBind>().Active && args.Channel == PacketChannel.C2S)
            {
                var gamePacket = new GamePacket(args.PacketData);
                switch ((C2SPacketOpcodes) gamePacket.Header)
                {
                    case C2SPacketOpcodes.Move:
                        var movePacket = Packet.C2S.Move.Decoded(args.PacketData);
                        if (movePacket.SourceNetworkId == ObjectManager.Player.NetworkId)
                        {
                            if (Config.Item("StopUlt").GetValue<bool>())
                            {
                                if ((!Q.IsReady() && !W.IsReady() && !E.IsReady()) && _thoughtContext.Target != null &&
                                    _thoughtContext.Target.Unit.IsValid &&
                                    _thoughtContext.Target.Unit.Health >
                                    (_thoughtContext.Target.DamageContext.QDamage + _thoughtContext.Target.DamageContext.WDamage +
                                     _thoughtContext.Target.DamageContext.EDamage))
                                    args.Process = false;
                            }
                        }
                        break;
                    case C2SPacketOpcodes.Cast:
                        var castPacket = Packet.C2S.Cast.Decoded(args.PacketData);
                        
                        if (castPacket.SourceNetworkId == ObjectManager.Player.NetworkId && castPacket.Slot == SpellSlot.R)
                            _thoughtContext.CastingUlt = true;

                        break;
                }
            }            
        }

        public override void OnDraw(EventArgs args)
        {
            if (Config.Item("DisableAllDrawing").GetValue<bool>())
                return;

            if (Config.Item("DrawQ").GetValue<bool>())
                Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.FromArgb(255, 178, 0, 0), 5, 30, false);
            
            if (Config.Item("DrawW").GetValue<bool>())
                Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.FromArgb(255, 178, 0, 0), 5, 30, false);

            if (Config.Item("DrawE").GetValue<bool>())
                Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.FromArgb(255, 178, 0, 0), 5, 30, false);

            if (Config.Item("DrawKill").GetValue<bool>())
                _thoughtContext.Targets.ForEach(x=>x.DrawText());
        }

        public override void OnWndProc(WndEventArgs args)
        {
            if (args.Msg == 0x204 && _thoughtContext.CastingUlt)
                _thoughtContext.CastingUlt = false;
        }

        public override string ChampionName
        {
            get { return "Katarina"; }
        }
    }
}
