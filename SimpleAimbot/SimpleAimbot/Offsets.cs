using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAimbot
{
    public class Offsets
    {
        // offsets.cs -> a2x cs2 dumper github
        public static int dwViewAngles = 0x1937EB0;
        public static int dwLocalPlayerPawn = 0x173D5A8;
        public static int dwEntityList = 0x18C9E88;
        public static int dwViewMatrix = 0x192B320;

        // client.dll.cs -> a2x cs2 dumper github
        public static int m_hPlayerPawn = 0x7E4;
        public static int m_iHealth = 0x334;
        public static int m_vOldOrigin = 0x127C;
        public static int m_iTeamNum = 0x3CB;
        public static int m_vecViewOffset = 0xC58;
        public static int m_lifeState = 0x338;
        public static int m_modelState = 0x160;
        public static int m_pGameSceneNode = 0x318;
    }
}
