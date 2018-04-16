using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public struct LevelPalette
    {
        public Color baseColor;
        public Color lightColor;
        public Color ultraColor;
        public Color complementColor;
        public Color darkColor;

        public LevelPalette(Color b, Color l, Color u, Color c, Color d)
        {
            baseColor = b;
            lightColor = l;
            ultraColor = u;
            complementColor = c;
            darkColor = d;
        }
    }

    public enum LevelColors { Yellow, Tangerine, Orange, RedOrange, Red, Pink, Purple, DarkBlue, LightBlue, BlueGreen, Green, LightGreen }

    public static class Palette
    {

        /*public struct LevelPaletteHex {
            public int baseColor;
            public int lightColor;
            public int ultraColor;
            public int complementColor;
            public int darkColor;

            public LevelPalette CreateLevelPalette() {
                return new LevelPalette (ColorFromHex(baseColor), ColorFromHex(lightColor), ColorFromHex(ultraColor), ColorFromHex(complementColor), ColorFromHex(darkColor));
            }
        }

        public static LevelPalette redLevel;
        public static LevelPalette blueLevel;

        public static void InitLevelColors() {
            redLevel = new LevelPalette ();
            redLevel.baseColor = ColorFromHex ();
        }*/

        private static int[] levelColorsHex = new int[]
        {
        0xFFE800, // yellow		0
		0xFFBC00, // tangerine	1
		0xFF7F00, // orange		2
		0xFF4D00, // red-orange	3
		0xFB0400, // red		4
		0xEC0084, // pink		5
		0x811B87, // purple		6
		0x133F72, // dark blue	7
		//0x004EA8, // dark blue	7
		0x0079C9, // light blue	8
		0x009DA8, // blue-green	9
		0x009731, // green		10
		0x84C500 // light green	11
        };

        private static int[] levelColorsLightHex = new int[]
        {
        0xFFE800, // yellow		0
		0xFFBC00, // tangerine	1
		0xFF7F00, // orange		2
		0xFF4D00, // red-orange	3
		0xAE5D5C, // red		4
		0xEC0084, // pink		5
		0x811B87, // purple		6
		0x1462D0, // dark blue	7
		0x0079C9, // light blue	8
		0x009DA8, // blue-green	9
		0x009731, // green		10
		0x84C500 // light green	11
        };

        private static int[] levelColorsUltraHex = new int[]
        {
        0xFFE800, // yellow		0
		0xFFBC00, // tangerine	1
		0xFFE800, // orange		2
		0xFF4D00, // red-orange	3
		0xFFE800, // red		4
		0xEC0084, // pink		5
		0x811B87, // purple		6
		0x00FFFF, // dark blue	7
		0x0079C9, // light blue	8
		0x009DA8, // blue-green	9
		0x009731, // green		10
		0x84C500 // light green	11
        };

        private static int[] levelComplementaryColorsHex = new int[]
        {
        0xFFE800, // yellow		0
		0xFFBC00, // tangerine	1
		0xFF7F00, // orange		2
		0xFF4D00, // red-orange	3
		0xFFE800, // red		4
		0xEC0084, // pink		5
		0x811B87, // purple		6
		0x004EA8, // dark blue	7
		0xC97900, // light blue	8
		0x009DA8, // blue-green	9
		0x009731, // green		10
		0x84C500 // light green	11
        };

        private static int[] levelDarkColorsHex = new int[]
        {
        0xFFE800, // yellow		0
		0xFFBC00, // tangerine	1
		0xFF7F00, // orange		2
		0xFF4D00, // red-orange	3
		0x810200, // red		4
		0xEC0084, // pink		5
		0x811B87, // purple		6
		0x004EA8, // dark blue	7
		0x003E67, // light blue	8
		0x009DA8, // blue-green	9
		0x009731, // green		10
		0x84C500 // light green	11
        };

        public static Color[] levelColors, levelColorsComplement, levelColorsDark, levelColorsLight, levelColorsUltra;

        public static void InitColors()
        {
            levelColors = new Color[levelColorsHex.Length];
            for (int i = 0; i < levelColors.Length; i++)
            {
                levelColors[i] = ColorFromHex(levelColorsHex[i]);
            }

            levelColorsComplement = new Color[levelComplementaryColorsHex.Length];
            for (int j = 0; j < levelColorsComplement.Length; j++)
            {
                levelColorsComplement[j] = ColorFromHex(levelColorsHex[j]);
            }

            levelColorsDark = new Color[levelDarkColorsHex.Length];
            for (int k = 0; k < levelColorsDark.Length; k++)
            {
                levelColorsDark[k] = ColorFromHex(levelDarkColorsHex[k]);
            }

            levelColorsLight = new Color[levelColorsLightHex.Length];
            for (int i = 0; i < levelColorsLight.Length; i++)
            {
                levelColorsLight[i] = ColorFromHex(levelColorsLightHex[i]);
            }

            levelColorsUltra = new Color[levelColorsUltraHex.Length];
            for (int i = 0; i < levelColorsUltra.Length; i++)
            {
                levelColorsUltra[i] = ColorFromHex(levelColorsUltraHex[i]);
            }
        }

        public static Color Invisible = new Color(1.0f, 1.0f, 1.0f, 0.0f);

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
    }
}