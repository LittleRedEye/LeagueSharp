using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Triton.Plugins
{
    public interface IPlugin
    {
        string Name { get; set; }
        PluginType Type { get; }

        void OnGameLoad(EventArgs args);
        void OnDraw(EventArgs args);
        void OnGameUpdate(EventArgs args);
        void OnPacketReceived(GamePacketEventArgs args);
        void OnPacketSend(GamePacketEventArgs args);

        bool Initialize();
        void SetupConfig();

        Menu Config { get; }
    }
}
