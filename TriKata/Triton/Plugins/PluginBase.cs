using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Triton.Plugins
{
    public class PluginBase : IPlugin
    {
        protected Menu _config;

        public PluginBase()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        public virtual bool Initialize()
        {
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Game.OnGameProcessPacket += OnPacketReceived;
            Game.OnWndProc += OnWndProc;
            Game.OnGameSendPacket += OnPacketSend;

            return true;
        }

        public virtual void SetupConfig()
        {
            
        }

        public virtual void OnGameLoad(EventArgs args)
        {
            Initialize();
        }

        public virtual void OnDraw(EventArgs args)
        {
        }

        public virtual void OnGameUpdate(EventArgs args)
        {
        }

        public virtual void OnPacketReceived(GamePacketEventArgs args)
        {
        }

        public virtual void OnPacketSend(GamePacketEventArgs args)
        {
            
        }

        public virtual void OnWndProc(WndEventArgs args)
        {
        }

        public virtual string Name { get; set; }
        public virtual PluginType Type { get; set; }

        public Menu Config
        {
            get { return _config; }
        }

    }
}
