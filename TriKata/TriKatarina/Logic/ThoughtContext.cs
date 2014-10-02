using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using TriKatarina.Logic.Thoughts;
using Triton.Plugins;

namespace TriKatarina.Logic
{
    public class ThoughtContext
    {
        private bool _castingUlt;
        List<Target> _targets = new List<Target>();

        public Target Target { get; set; }
        public ChampionPluginBase Plugin { get; set; }

        public float QTimeToHit { get; set; }

        public bool CastingUlt
        {
            get
            {
                return _castingUlt || ObjectManager.Player.IsChannelingImportantSpell();
            }
            set { _castingUlt = value; }
        }

        public List<Target> Targets
        {
            get { return _targets; }
            set { _targets = value; }
        }
    }
}