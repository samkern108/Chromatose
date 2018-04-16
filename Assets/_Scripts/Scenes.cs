using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public struct SceneInfo
    {
		public SceneInfo(int sceneId, bool sceneEnabled) {
			this.sceneEnabled = sceneEnabled;
			this.sceneId = sceneId;
		}
		public bool sceneEnabled;
		public int sceneId;
    }

    public class Scenes
    {
        public static SceneInfo[] scenes = new SceneInfo[12];

//    Yellow, Tangerine, Orange, RedOrange, Red, Pink, Purple, DarkBlue, LightBlue, BlueGreen, Green, LightGreen
        public static void Init() {
			scenes[0] = new SceneInfo(0,false);
			scenes[1] = new SceneInfo(0,false);
			scenes[2] = new SceneInfo(2,true); // orange
			scenes[3] = new SceneInfo(0,false);
			scenes[4] = new SceneInfo(1,true); // red
			scenes[5] = new SceneInfo(0,false);
			scenes[6] = new SceneInfo(0,false);
			scenes[7] = new SceneInfo(3,true); // darkblue
			scenes[8] = new SceneInfo(0,false);
			scenes[9] = new SceneInfo(0,false);
			scenes[10] = new SceneInfo(0,false);
			scenes[11] = new SceneInfo(0,false);
		}
    }
}