using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using System.Reflection;
namespace TongHop
{
    class Program
    {
        public static int delaytick = 0;
        public static Menu Menu;
        public static Items.Item Guomvd;//BOTRK
        public static Items.Item Kiemht;//Bilgewater Cutlass
        public static Items.Item Ythien;//Sword of the Divine
        public static Items.Item Makiem;//Youmuu's Ghostblade
        public static Items.Item Kiemsung;//Hextech Gunblade
        public static Items.Item Riumx;//Ravenous Hydra
        public static Items.Item Tiamat;
        public static Items.Item Buadl;//Deathfire Grasp
        public static Items.Item Truongdts;//Seraph
        public static Items.Item Yeusach;//Frost Queen's Claim
        public static Items.Item Khienbang;//Randuin's Omen
        public static TargetSelector Chon;
        private static SpellSlot Heal;
        private static SpellSlot Barrier;
        private static SpellSlot Smite;
        public static SpellSlot Dot ;
        public static SpellSlot Exhaust;
        public static Obj_AI_Hero Tuong;
        public static Items.Item KhanGT; //Mercurial Scimitar
        public static Items.Item Mikael; //Mikael's Crucible
        public static Items.Item Dervish; //Dervish Blade
        public static Items.Item DaoTN; //Quicksilver Sash
        public static Items.Item Muramana;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        static void Game_OnGameLoad(EventArgs args)
        {
            Tuong = ObjectManager.Player;
            Heal = Tuong.GetSpellSlot("summonerheal");
            Barrier = Tuong.GetSpellSlot("summonerbarrier");
            Smite = Tuong.GetSpellSlot("summonersmite");
            Dot = Tuong.GetSpellSlot("summonerdot");
            Exhaust = Tuong.GetSpellSlot("summonerexhaust");
            Riumx = new Items.Item(3074, 400f);
            Tiamat = new Items.Item(3077, 400f);
            Guomvd = new Items.Item(3153, 450f);
            Kiemht = new Items.Item(3144, 450f);
            Makiem = new Items.Item(3142, (int)Orbwalking.GetRealAutoAttackRange(Tuong));
            Truongdts = new Items.Item(3040, 175f);
            Buadl = new Items.Item(3128, 750f);
            Khienbang = new Items.Item(3143, 450f);
            Kiemsung = new Items.Item(3146, 700f);
            Ythien = new Items.Item(3131, (int)Orbwalking.GetRealAutoAttackRange(Tuong));
            KhanGT = new Items.Item(3139, 185f);
            Dervish = new Items.Item(3137, 185f);
            DaoTN = new Items.Item(3140, 185f);
            Mikael = new Items.Item(3222, 750f);
            Yeusach = new Items.Item(3092, 900f);
            Muramana = new Items.Item(3042, (int)Orbwalking.GetRealAutoAttackRange(Tuong));
            try
            {
                Chon = new TargetSelector(1000, TargetSelector.TargetingMode.LowHP);
                Menu = new Menu("TongHop", "TongHop", true);
                Menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
                Menu.SubMenu("TargetSelector").AddItem(new MenuItem("Mode", "")).SetValue(new StringList(new[] { "AutoPriority", "Closest", "LessAttack", "LessCast", "LowHP", "MostAD", "MostAP", "NearMouse" }, 1));
                Menu.SubMenu("TargetSelector").AddItem(new MenuItem("Range", "")).SetValue(new Slider(750, 2000, 100));
                Menu.AddSubMenu(new Menu("UseItems", "SD"));
                Menu.AddSubMenu(new Menu("Spell", "SP"));
                Menu.SubMenu("SP").AddSubMenu(new Menu("Barrier", "Barrier"));
                Menu.SubMenu("SP").AddSubMenu(new Menu("Heal", "Heal"));
                Menu.SubMenu("SP").SubMenu("Barrier").AddItem(new MenuItem("UseBarrier", "Barrier!").SetValue(true));
                Menu.SubMenu("SP").SubMenu("Heal").AddItem(new MenuItem("UseHeal", "Heal!").SetValue(true));
                Menu.SubMenu("SP").SubMenu("Barrier").AddItem(new MenuItem("BarrierPercent", "").SetValue(new Slider(30, 0, 100)));
                Menu.SubMenu("SP").SubMenu("Heal").AddItem(new MenuItem("HealPercent", "").SetValue(new Slider(30, 0, 100)));
                Menu.SubMenu("SP").AddItem(new MenuItem("Ignite", "Ignite").SetValue(true));
                Menu.SubMenu("SP").AddSubMenu(new Menu("Exhaust", "Exhaust"));
                Menu.SubMenu("SP").SubMenu("Exhaust").AddItem(new MenuItem("UseExhaust", "Exhaust!").SetValue(true));
                Menu.SubMenu("SP").SubMenu("Exhaust").AddItem(new MenuItem("ExhaustPercent", "Percent").SetValue(new Slider(40, 0, 100)));
                Menu.SubMenu("SP").AddSubMenu(new Menu("Smite", "Smite"));
                Menu.SubMenu("SP").SubMenu("Smite").AddItem(new MenuItem("UseSmite", "Smite!").SetValue(true));
                Menu.SubMenu("SP").SubMenu("Smite").AddItem(new MenuItem("Smite", "ActiveUse").SetValue<KeyBind>(new KeyBind('N', KeyBindType.Toggle)));
                Menu.SubMenu("SD").AddSubMenu(new Menu("Active", "Tcong"));
                Menu.SubMenu("SD").AddSubMenu(new Menu("Defensive", "Pthu"));
                Menu.SubMenu("SD").AddItem(new MenuItem("ActiveUse", "ActiveUse").SetValue(new KeyBind(32, KeyBindType.Press)));
                Menu.AddSubMenu(new Menu("AutoPotions", "Potions"));
                Menu.SubMenu("Potions").AddSubMenu(new Menu("Health", "Health"));
                Menu.SubMenu("Potions").SubMenu("Health").AddItem(new MenuItem("HealthPotion", "Use Health").SetValue(true));
                Menu.SubMenu("Potions").SubMenu("Health").AddItem(new MenuItem("HealthPercent", "HP  Percent").SetValue(new Slider(50, 0, 100)));
                Menu.SubMenu("Potions").AddSubMenu(new Menu("Mana", "Mana"));
                Menu.SubMenu("Potions").SubMenu("Mana").AddItem(new MenuItem("ManaPotion", "Use Mana").SetValue(true));
                Menu.SubMenu("Potions").SubMenu("Mana").AddItem(new MenuItem("ManaPercent", "MP Percent").SetValue(new Slider(50, 0, 100)));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("BOTRK", "BOTRK").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("UrHealthPer", "UrHealthPer").SetValue(new Slider(50, 0, 100)));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("EnemyHealthPer", "EnemyHealthPer").SetValue(new Slider(50, 0, 100)));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("riumx", "Ravenous Hydra").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("tiamat", "Tiamat").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("kiemht", "Bilgewater Cutlass").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("kiemsung", "Hextech Gunblade").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("buadl", "Deathfire Grasp").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("yeusach", "Frost Queen's Claim").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("ythien", "Sword of the Divine").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("makiem", "Youmuu's Ghostblade").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("muramana", "Muramana").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Tcong").AddItem(new MenuItem("manaper", "Min Mana Percent").SetValue(new Slider(50, 0, 100)));

                Menu.SubMenu("SD").SubMenu("Pthu").AddItem(new MenuItem("truongdts", "Seraph's Embrace").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Pthu").AddItem(new MenuItem("HealthPer3", "Seraph Percent").SetValue(new Slider(50, 0, 100)));
                Menu.SubMenu("SD").SubMenu("Pthu").AddItem(new MenuItem("hbhmika", "Mikael's Crucible").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Pthu").AddItem(new MenuItem("khienbang", "Randuin's Omen").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Pthu").AddItem(new MenuItem("Khangt", "Mercurial Scimitar").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Pthu").AddItem(new MenuItem("daotn", "Quicksilver Sash").SetValue(true));
                Menu.SubMenu("SD").SubMenu("Pthu").AddItem(new MenuItem("Dervish", "Dervish Blade").SetValue(true));
                Menu.AddToMainMenu();
                Game.PrintChat("<font color='#1d87f2'>TongHop Loaded!</font>");
                Game.OnGameUpdate += Game_OnGameUpdate;
            }
            catch
            {
                
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
                Mode();
                heal();
                barrier();
                exhaust();
                ignite();
                smite();
                UsePotions();
                AItem();
                DItem();
        }
        private static void Mode()
        {

            float TSRange = Menu.Item("Range").GetValue<Slider>().Value;
            Chon.SetRange(TSRange);
            var mode = Menu.Item("Mode").GetValue<StringList>().SelectedIndex;

            switch (mode)
            {
                case 0:
                    Chon.SetTargetingMode(TargetSelector.TargetingMode.LowHP);
                    break;
                case 1:
                    Chon.SetTargetingMode(TargetSelector.TargetingMode.NearMouse);
                    break;
                case 2:
                    Chon.SetTargetingMode(TargetSelector.TargetingMode.MostAD);
                    break;
                case 3:
                    Chon.SetTargetingMode(TargetSelector.TargetingMode.MostAP);
                    break;
                case 4:
                    Chon.SetTargetingMode(TargetSelector.TargetingMode.LessAttack);
                    break;
                case 5:
                    Chon.SetTargetingMode(TargetSelector.TargetingMode.LessCast);
                    break;
                case 6:
                    Chon.SetTargetingMode(TargetSelector.TargetingMode.Closest);
                    break;
                case 7:
                    Chon.SetTargetingMode(TargetSelector.TargetingMode.AutoPriority);
                    break;
            }
        }
        private static void UsePotions()
        {
            if (Program.Menu.Item("HealthPotion").GetValue<bool>())
            {
                if (Tuong.Health * 100 / Tuong.MaxHealth <= Menu.Item("HealthPercent").GetValue<Slider>().Value)
                {
                    if (!Tuong.Buffs.Any(buff => buff.Name == "RegenerationPotion" || buff.Name == "ItemCrystalFlask" || buff.Name == "ItemMiniRegenPotion"))
                        HealthSlot().UseItem();
                }
            }
            if (Program.Menu.Item("ManaPotion").GetValue<bool>())
            {
                if (Tuong.Mana * 100 / Tuong.MaxMana <= Menu.Item("ManaPercent").GetValue<Slider>().Value)
                {
                    if (!Tuong.Buffs.Any(buff => buff.Name == "ItemCrystalFlask" || buff.Name == "FlaskOfCrystalWater"))
                        ManaSlot().UseItem();
                }
            }
        }
        private static InventorySlot HealthSlot()
        {
            return Tuong.InventoryItems.First(item => (item.Id == (ItemId)2003 && item.Stacks >= 1) || (item.Id == (ItemId)2009 && item.Stacks >= 1) || (item.Id == (ItemId)2010 && item.Stacks >= 1) || (item.Id == (ItemId)2041) && item.Charges >= 1);
        }

        private static InventorySlot ManaSlot()
        {
            return Tuong.InventoryItems.First(item => (item.Id == (ItemId)2004 && item.Stacks >= 1) || (item.Id == (ItemId)2041 && item.Charges >= 1));
        }

        private static void heal()
        {
            if (Heal == SpellSlot.Unknown ||
                (!Menu.Item("UseHeal").GetValue<bool>() ||
                 ObjectManager.Player.SummonerSpellbook.CanUseSpell(Heal) !=
                 SpellState.Ready))
                return;
            if (ObjectManager.Player.Health / ObjectManager.Player.MaxHealth * 100 <=
                Menu.Item("HealPercent").GetValue<Slider>().Value)
            {
                ObjectManager.Player.SummonerSpellbook.CastSpell(Heal);
                return;
            }
        }
        private static void barrier()
        {
            if (Barrier == SpellSlot.Unknown ||
                  (!Menu.Item("useBarrier").GetValue<bool>() ||
                   ObjectManager.Player.SummonerSpellbook.CanUseSpell(Barrier) !=
                   SpellState.Ready))
                return;
            if (!(ObjectManager.Player.Health / ObjectManager.Player.MaxHealth * 100 <=
                  Menu.Item("BarrierPercent").GetValue<Slider>().Value))
                return;
            ObjectManager.Player.SummonerSpellbook.CastSpell(Barrier);
        }
        private static void smite()
        {
            if(Smite == SpellSlot.Unknown ||
                (!Menu.Item("UseSmite").GetValue<bool>() ||
				 ObjectManager.Player.SummonerSpellbook.CanUseSpell(Smite) !=
				 SpellState.Ready))
				return;
			var minion = SmiteTarget.GetNearest(ObjectManager.Player.Position);
			if(minion != null && minion.Health <= SmiteTarget.Damage())
				ObjectManager.Player.SummonerSpellbook.CastSpell(Smite, minion);
        }
        private static void ignite()
        {
            if (Dot == SpellSlot.Unknown ||
                 ObjectManager.Player.SummonerSpellbook.CanUseSpell(Dot) !=
                 SpellState.Ready)
                return;
            if (!(Menu.Item("Ignite").GetValue<bool>()))
                return;
            const int range = 600;
            if (Menu.Item("Ignite").GetValue<bool>())
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(range) && ObjectManager.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) >= hero.Health))
                {
                    ObjectManager.Player.SummonerSpellbook.CastSpell(Dot, enemy);
                    return;
                }
        }
        private static void exhaust()
        {
            if (Exhaust == SpellSlot.Unknown ||
                (!Menu.Item("useExhaust").GetValue<bool>() ||
                 ObjectManager.Player.SummonerSpellbook.CanUseSpell(Exhaust) !=
                 SpellState.Ready))
                return;
            Obj_AI_Hero Hero = null;
            float td = 0;
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(750)))
            {
                var dps = enemy.BaseAttackDamage * enemy.AttackSpeedMod;
                if (Hero != null && !(td < dps))
                    continue;
                td = dps;
                Hero = enemy;
            }
            if (Hero == null)
                return;

            if (ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly && hero.Distance(ObjectManager.Player) <= 550).Any(friend => friend.Health <= td * 3))
                ObjectManager.Player.SummonerSpellbook.CastSpell(Exhaust, Hero);
        }
        internal class SmiteTarget
        {
            private static readonly string[] MinionNames = { "Worm", "Dragon", "LizardElder", "AncientGolem", "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith" };

            public static Obj_AI_Minion GetNearest(Vector3 pos)
            {
                var minions = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValid && MinionNames.Any(name => minion.Name.StartsWith(name)));
                var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
                var sMinion = objAiMinions.FirstOrDefault();
                double? nearest = null;
                var index = 0;
                for (; index < objAiMinions.Length; index++)
                {
                    var minion = objAiMinions[index];
                    var distance = Vector3.Distance(pos, minion.Position);
                    if (nearest != null && !(nearest > distance))
                        continue;
                    nearest = distance;
                    sMinion = minion;
                }
                return sMinion;
            }

            public static double Damage()
            {
                var level = ObjectManager.Player.Level;
                int[] stages = { 20 * level + 370, 30 * level + 330, 40 * level + 240, 50 * level + 100 };
                return stages.Max();
            }
        }
        private static void AItem()
        {
            if (Menu.Item("ActiveUse").GetValue<KeyBind>().Active)
            {
                BOTRK();
                kiemht();
                kiemsung();
                buadl();
                khienbang();
                yeusach();
                antistun();
                AntiStunFriend();
                ythien();
                makiem();
                riumx();
                tiamat();
            }
        }
        private static void makiem()
		{
            try
            {
				var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(Makiem.Range));
                if (Hero != null && Makiem.IsReady() && Menu.Item("makiem").GetValue<bool>())
					Makiem.Cast();
            }
            catch
            {

            }
		}
        private static void ythien()
        {
            try
            {
                var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(Ythien.Range));
                if (Hero != null && Ythien.IsReady() && Menu.Item("ythien").GetValue<bool>())
                    Ythien.Cast();
            }
            catch
            {

            }
        }
        private static void muramana()
        {
            try
            {
                var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(Muramana.Range));
                if (Hero != null && Muramana.IsReady() && Menu.Item("muramana").GetValue<bool>() && !Hero.HasBuff(Muramana.ToString(), true))
                {
                        Muramana.Cast();
                }
                if (Hero == null && Muramana.IsReady()  && Hero.HasBuff(Muramana.ToString(),true)) 
                    {    Muramana.Cast();
                    }
            }
            catch
            {

            }
        }

        private static void yeusach()
        {
            try
            {
                var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(Yeusach.Range));
                if (Hero != null && Yeusach.IsReady() && Menu.Item("yeusach").GetValue<bool>())
                  {  Yeusach.Cast(Hero.ServerPosition);
                  }
            }
            catch
            {

            }
        }
        private static void khienbang()
		{
            try
            {
				var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(Khienbang.Range));
                if (Hero != null && Khienbang.IsReady() && Menu.Item("khienbang").GetValue<bool>())
				{	Khienbang.Cast();
				}
					
            }
            catch
            {

            }
		}

		private static void tiamat()
		{
            try
            {
				var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(Tiamat.Range));
                if (Hero != null && Tiamat.IsReady() && Menu.Item("tiamat").GetValue<bool>())
				{	Tiamat.Cast();
				}
            }
            catch
            {

            }
		}

		private static void riumx()
		{
            try
            {
				var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(Riumx.Range));
                if (Hero != null && Riumx.IsReady() && Menu.Item("riumx").GetValue<bool>())
				{	Riumx.Cast();
				}
            }
            catch
            {

            }
		}

		private static void kiemsung()
		{
			try
            		{
				var hero = SimpleTs.GetTarget(Kiemsung.Range, SimpleTs.DamageType.Magical);
                if (hero != null && Kiemsung.IsReady() && Menu.Item("kiemsung").GetValue<bool>())
				{	Kiemsung.Cast(hero);
				}
			}
			catch
			{
			}
		}

		private static void kiemht()
		{
            try
            {
				var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(Kiemht.Range));
                if (Hero != null && Kiemht.IsReady() && Menu.Item("kiemht").GetValue<bool>())
				{	
					Kiemht.Cast(Hero);
				}
            }
            catch
            {

            }
		}

		private static void buadl()
		{
			try
			{
				var hero = SimpleTs.GetTarget(Buadl.Range, SimpleTs.DamageType.Magical);
                if (hero != null && Buadl.IsReady() && Menu.Item("buadl").GetValue<bool>())
				{	Buadl.Cast(hero);
				}
			}
			catch
			{
			}
		}

		private static void BOTRK()
		{
			try
			{
				var hero = SimpleTs.GetTarget(Guomvd.Range, SimpleTs.DamageType.Magical);
                if (hero != null && Menu.Item("BOTRK").GetValue<bool>())
                {
                    if ((Tuong.Health * 100 / Tuong.MaxHealth <= Menu.Item("UrHealthPer").GetValue<Slider>().Value) || (hero.Health * 100 / hero.MaxHealth <= Menu.Item("EnemyHealthPer").GetValue<Slider>().Value))
                    {
                        Guomvd.Cast(hero);
                    }
                }
			}
			catch
			{
			}
		}

		private static void antistun()
		{
			try
			{
			    if(Tuong.HasBuffOfType(BuffType.Snare) || Tuong.HasBuffOfType(BuffType.Stun))
                {
                    if(Mikael.IsReady() && Menu.Item("hbhmika").GetValue<bool>())
                    {
                        Mikael.Cast(Tuong);
                        return;
                    }
                    if (KhanGT.IsReady() && Menu.Item("khangt").GetValue<bool>())
                    {
                        KhanGT.Cast();
                        return;
                    }
                    if (DaoTN.IsReady() && Menu.Item("daotn").GetValue<bool>())
                    {
                        DaoTN.Cast();
                        return;
                    }
                }
			}
			catch
			{
			}
		}

        private static void AntiStunFriend()
        {
            try
            {
                var Hero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsAlly && !hero.IsDead && (hero.HasBuffOfType(BuffType.Snare) || hero.HasBuffOfType(BuffType.Stun)) && hero.Distance(Tuong) <= Mikael.Range && Mikael.IsReady());
                if (Hero != null)
                 {   Mikael.Cast(Hero);
                     return;
                 }
            }
            catch
            {
            }
        }
        private static void DItem()
        {
            try
            {
                if (Menu.Item("truongdts").GetValue<bool>())
                {
                    if (Truongdts.IsReady())
                        if (Tuong.Health * 100 / Tuong.MaxHealth <= Menu.Item("HealthPer3").GetValue<Slider>().Value)
                         {   Truongdts.Cast();
                         }
                }
            }
            catch
            {
            }

        }
    }
}
