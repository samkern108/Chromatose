using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class LevelPalette
    {
        public Color darkColor, baseColor, lightColor, ultraColor;
        public Color complementColor;
        public Color playerColor;
    }

    public static class Palette
    {
        private static LevelPalette yellow, tangerine, orange, redorange, red, pink, purple, darkblue, lightblue, bluegreen, green, lightgreen;
		public enum LevelColor { yellow, tangerine, orange, redorange, red, pink, purple, darkblue, lightblue, bluegreen, green, lightgreen };
		public static Dictionary<LevelColor, LevelPalette> levels = new Dictionary<LevelColor, LevelPalette>();

        public static void InitColors()
        {
            yellow = new LevelPalette();
            yellow.baseColor = ColorFromHex(0xFFE800);
            yellow.lightColor = ColorFromHex(0xFF7F00);
            yellow.ultraColor = ColorFromHex(0xFF7F00);
            yellow.complementColor = ColorFromHex(0xFF7F00);
            yellow.darkColor = ColorFromHex(0xFF7F00);
            yellow.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.yellow, yellow);

            tangerine = new LevelPalette();
            tangerine.baseColor = ColorFromHex(0xFFBC00);
            tangerine.lightColor = ColorFromHex(0xFF7F00);
            tangerine.ultraColor = ColorFromHex(0xFF7F00);
            tangerine.complementColor = ColorFromHex(0xFF7F00);
            tangerine.darkColor = ColorFromHex(0xFF7F00);
            tangerine.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.tangerine, tangerine);

            orange = new LevelPalette();
            orange.baseColor = ColorFromHex(0xFF7F00);
            orange.lightColor = ColorFromHex(0xFF7F00);
            orange.ultraColor = ColorFromHex(0xFF7F00);
            orange.complementColor = ColorFromHex(0xFF7F00);
            orange.darkColor = ColorFromHex(0xFF7F00);
            orange.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.orange, orange);

            redorange = new LevelPalette();
            redorange.baseColor = ColorFromHex(0xFF4D00);
            redorange.lightColor = ColorFromHex(0xFF4D00);
            redorange.ultraColor = ColorFromHex(0xFF4D00);
            redorange.complementColor = ColorFromHex(0xFF4D00);
            redorange.darkColor = ColorFromHex(0xFF4D00);
            redorange.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.redorange, redorange);

            red = new LevelPalette();
            red.baseColor = ColorFromHex(0xFB0400);
            red.lightColor = ColorFromHex(0xFF7F00);
            red.ultraColor = ColorFromHex(0xFF7F00);
            red.complementColor = ColorFromHex(0xFF7F00);
            red.darkColor = ColorFromHex(0xFF7F00);
            red.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.red, red);

            pink = new LevelPalette();
            pink.baseColor = ColorFromHex(0xEC0084);
            pink.lightColor = ColorFromHex(0xFF7F00);
            pink.ultraColor = ColorFromHex(0xFF7F00);
            pink.complementColor = ColorFromHex(0xFF7F00);
            pink.darkColor = ColorFromHex(0xFF7F00);
            pink.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.pink, pink);

            purple = new LevelPalette();
            purple.baseColor = ColorFromHex(0x811B87);
            purple.lightColor = ColorFromHex(0xFF7F00);
            purple.ultraColor = ColorFromHex(0xFF7F00);
            purple.complementColor = ColorFromHex(0xFF7F00);
            purple.darkColor = ColorFromHex(0xFF7F00);
            purple.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.purple, purple);

            darkblue = new LevelPalette();
            darkblue.baseColor = ColorFromHex(0x133F72);
            darkblue.lightColor = ColorFromHex(0x1462D0);
            darkblue.ultraColor = ColorFromHex(0x00FFFF);
            darkblue.complementColor = ColorFromHex(0xFF7F00);
            darkblue.darkColor = ColorFromHex(0x004EA8);
            darkblue.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.darkblue, darkblue);

            lightblue = new LevelPalette();
            lightblue.baseColor = ColorFromHex(0x0079C9);
            lightblue.lightColor = ColorFromHex(0xFF7F00);
            lightblue.ultraColor = ColorFromHex(0xFF7F00);
            lightblue.complementColor = ColorFromHex(0xFF7F00);
            lightblue.darkColor = ColorFromHex(0xFF7F00);
            lightblue.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.lightblue, lightblue);

            bluegreen = new LevelPalette();
            bluegreen.baseColor = ColorFromHex(0xFF7F00);
            bluegreen.lightColor = ColorFromHex(0xFF7F00);
            bluegreen.ultraColor = ColorFromHex(0xFF7F00);
            bluegreen.complementColor = ColorFromHex(0xFF7F00);
            bluegreen.darkColor = ColorFromHex(0xFF7F00);
            bluegreen.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.bluegreen, bluegreen);

            green = new LevelPalette();
            green.baseColor = ColorFromHex(0x009731);
            green.lightColor = ColorFromHex(0xFF7F00);
            green.ultraColor = ColorFromHex(0xFF7F00);
            green.complementColor = ColorFromHex(0xFF7F00);
            green.darkColor = ColorFromHex(0xFF7F00);
            green.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.green, green);

            lightgreen = new LevelPalette();
            lightgreen.baseColor = ColorFromHex(0x84C500);
            lightgreen.lightColor = ColorFromHex(0xFF7F00);
            lightgreen.ultraColor = ColorFromHex(0xFF7F00);
            lightgreen.complementColor = ColorFromHex(0xFF7F00);
            lightgreen.darkColor = ColorFromHex(0x84C500);
            lightgreen.playerColor = ColorFromHex(0xFFFFFF);
			levels.Add(LevelColor.lightgreen, lightgreen);
        }

        public static Color ColorFromHex(int c, float alpha = 1.0f)
        {
            int r = (c >> 16) & 0x000000FF;
            int g = (c >> 8) & 0x000000FF;
            int b = c & 0x000000FF;

            Color ret = ColorFromRGB(r, g, b);
            ret.a = alpha;

            return ret;
        }

        public static Color ColorFromRGB(int r, int g, int b)
        {
            return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, 1.0f);
        }

        public static Color Invisible = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
}