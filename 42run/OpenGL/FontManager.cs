﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _42run.OpenGL
{
    public static class FontManager
    {
        private static Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        public static Font Get(string name)
        {
            if (_fonts.ContainsKey(name))
                return _fonts[name];
            var font = new Font($"{name}.png", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789=*-+[]{}()|\\,./<>?;:'\"");
            _fonts.Add(name, font);
            return font;
        }

        public static void Remove(string name)
        {
            if (_fonts.ContainsKey(name))
            {
                _fonts[name].Dispose();
                _fonts.Remove(name);
            }
        }
    }
}