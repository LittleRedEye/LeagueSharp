using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX;

namespace Triton
{
    public static class TextManager
    {
        static Dictionary<string, Render.Text> _texts = new Dictionary<string, Render.Text>();

        public static void Add(string name, string text, Vector2 position, int size, ColorBGRA color, string fontName)
        {
            _texts.Add(name, new Render.Text(position, text, size, color, fontName));
        }

        public static void OnDraw(EventArgs args)
        {
            _texts.Values.ToList().ForEach(x=>x.OnEndScene());
        }
    }
}
