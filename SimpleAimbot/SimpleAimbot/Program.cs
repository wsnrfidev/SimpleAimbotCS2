using SimpleAimbot;
using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;


// init swed
Swed swed = new Swed("cs2");

// module base
IntPtr client = swed.GetModuleBase("client.dll");

// init ImGui
Renderer renderer = new Renderer();
renderer.Start().Wait();
Vector2 screenSize = renderer.screenSize;

// entity handling
List<Entity> entities = new List<Entity>(); // all entities
Entity localPlayer = new Entity(); // local player


// const
const int HOTKEY = 0x06; // HOTKEY

// AIMBOT LOOP!!!

while (true) // runnnnnn......
{
    entities.Clear();
    Console.Clear();

    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

    // entry
    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    // update player information
    localPlayer.pawnAddress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iTeamNum);
    localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vOldOrigin);
    localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vecViewOffset);

   for (int i = 0; i < 64; i++) // 64 controlls
    {
        if (listEntry == IntPtr.Zero) // skip jika tidak valid
            continue;

        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);

        if (currentController == IntPtr.Zero)
            continue;

        // pawn

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);

        if (pawnHandle == 0)
            continue;

        // 2nd Entry

        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

        if (currentPawn == localPlayer.pawnAddress)
            continue;

        IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);
        IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80);

        // pawn attributes
        int health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
        int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);

        if (lifeState != 256)
            continue;
        if (team == localPlayer.team && !renderer.aimOnTeam)
            continue;

        Entity entity = new Entity();

        entity.pawnAddress = currentPawn;
        entity.controllerAddress = currentController;
        entity.health = health;
        entity.lifeState = lifeState;
        entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.view = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
        entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
        entity.head = swed.ReadVec(boneMatrix, 6 * 32);

        // get 2d infos
        viewMatrix ViewMatrix = ReadMatrix(client + Offsets.dwViewMatrix);
        entity.head2d = Calculate.WorldToScreen(ViewMatrix, entity.head, (int)screenSize.X, (int)screenSize.Y);
        entity.pixelDistance = Vector2.Distance(entity.head2d, new Vector2(screenSize.X / 2, screenSize.Y / 2));
        
        entities.Add(entity);
        
        // draw to console
        Console.ForegroundColor = ConsoleColor.Green;

        if (team != localPlayer.team)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        Console.WriteLine($"{entity.health}hp, head coords {entity.head}");

        Console.ResetColor();
    }

   // sort entities and aim

   entities = entities.OrderBy(o => o.pixelDistance).ToList();
    if (entities.Count > 0 && GetAsyncKeyState(HOTKEY) <0 && renderer.aimbot) // intinya ini eksekusi terakhir dari renderer, count, etc...
    {
        // ini view pos
        Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
        Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

        // ini cek kalo di dalem FOV dia auto ketrigger
        if (entities[0].pixelDistance < renderer.FOV)
        {
            // ini anglesnya, CAPEK PAKE BAHASA INGGRIS!!!!!
            Vector2 newAngles = Calculate.CalculateAngles(playerView, entities[0].head);
            Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);

            // angle baru
            swed.WriteVec(client, Offsets.dwViewAngles, newAnglesVec3);
        } 
    }
    Thread.Sleep(20); // INI MAU PAKE ATAU KAGAK SERAH LU DAH NYET!!!!!! OPSIONAL DOANG INI MAH!!!!!
}

// hotkey import
[DllImport("user32.dll")]

static extern short GetAsyncKeyState(int vKey);

viewMatrix ReadMatrix(IntPtr matrixAddress)
{
    var viewMatrix = new viewMatrix();
    var matrix = swed.ReadMatrix(matrixAddress);

    // row
    viewMatrix.m11 = matrix[0];
    viewMatrix.m12 = matrix[1];
    viewMatrix.m13 = matrix[2];
    viewMatrix.m14 = matrix[3];

    // 2nd row
    viewMatrix.m21 = matrix[4];
    viewMatrix.m22 = matrix[5];
    viewMatrix.m23 = matrix[6];
    viewMatrix.m24 = matrix[7];

    // 3rd row
    viewMatrix.m31 = matrix[8];
    viewMatrix.m32 = matrix[9];
    viewMatrix.m33 = matrix[10];
    viewMatrix.m34 = matrix[11];

    // 4th row
    viewMatrix.m41 = matrix[12];
    viewMatrix.m42 = matrix[13];
    viewMatrix.m43 = matrix[14];
    viewMatrix.m44 = matrix[15];

    return viewMatrix;
}