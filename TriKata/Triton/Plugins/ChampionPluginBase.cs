using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Triton.Plugins
{
    public class ChampionPluginBase : PluginBase, IChampionPlugin
    {
        Dictionary<string,Spell> _spells = new Dictionary<string, Spell>();
        private bool _isCorrectChampion = true;
        
        private Spell _q;
        private Spell _w;
        private Spell _e;
        private Spell _r;

        protected Orbwalking.Orbwalker _orbwalker;

        public ChampionPluginBase()
        {
        }

        public override bool Initialize()
        {
            if (!IsCorrectChampion)
                return false;

            SetupConfig();
            SetupSpells();

            return base.Initialize();
        }

        public override void OnWndProc(WndEventArgs args)
        {
            
        }

        public override void SetupConfig()
        {
            _config = new Menu(ChampionName, ChampionName, true);
        }

        public virtual void SetupSpells()
        {
        }
                
        public override void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                _isCorrectChampion = false;
                return;
            }

            base.OnGameLoad(args);
        }


        public void RegisterSpell(string key, Spell spell)
        {
            if (!_spells.ContainsKey(key))
                _spells.Add(key, spell);

            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    _q = spell;
                    break;
                case SpellSlot.W:
                    _w = spell;
                    break;
                case SpellSlot.E:
                    _e = spell;
                    break;
                case SpellSlot.R:
                    _r = spell;
                    break;
            }
        }

        public Spell GetSpell(string key)
        {
            Spell tmp;

            _spells.TryGetValue(key, out tmp);

            return tmp;
        }

        
        public virtual string ChampionName { get; set; }

        public bool IsCorrectChampion
        {
            get { return _isCorrectChampion; }
        }

        public Dictionary<string, Spell> Spells
        {
            get { return _spells; }
        }

        public PluginType Type
        {
            get
            {
                return PluginType.Champion;
            }
        }

        public Spell Q
        {
            get { return _q; }
        }

        public Spell W
        {
            get { return _w; }
        }

        public Spell E
        {
            get { return _e; }
        }

        public Spell R
        {
            get { return _r; }
        }

        public Orbwalking.Orbwalker Orbwalker
        {
            get { return _orbwalker; }
        }
    }
}
