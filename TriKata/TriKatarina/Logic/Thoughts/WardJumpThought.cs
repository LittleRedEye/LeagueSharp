using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Triton.Logic;

namespace TriKatarina.Logic.Thoughts
{
    class WardJumpThought : Thought
    {
        public override bool ShouldActualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            return context.Plugin.Config.Item("WardJumpKey").GetValue<KeyBind>().Active;
        }

        public override void Actualize(object contextObj)
        {
            var context = (ThoughtContext)contextObj;
            var position = ObjectManager.Player.Distance(Game.CursorPos, true) <= 300 * 300 ? Game.CursorPos : KatarinaUtilities.GetMousePosition();
            KatarinaUtilities.WardJump(context, position.X, position.Y);
        }
    }
}
