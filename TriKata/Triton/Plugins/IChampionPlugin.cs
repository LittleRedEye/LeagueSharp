using LeagueSharp.Common;

namespace Triton.Plugins
{
    public interface IChampionPlugin : IPlugin
    {
        void RegisterSpell(string key, Spell spell);
        void SetupSpells();

        string ChampionName { get; set; }
        bool IsCorrectChampion { get; }
    }
}