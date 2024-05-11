using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace SimpleAimbot
{
    public class Renderer : Overlay
    {
        public bool aimbot = true;
        public bool aimOnTeam = false;
        public Vector2 screenSize = new Vector2(1920, 1080);
        public float FOV = 50;
        public Vector4 circleColor = new Vector4(1, 1, 1, 1);
        protected override void Render()
        {
            ImGui.Begin("MENU");
            ImGui.Checkbox("AIMBOT", ref aimbot);
            ImGui.Checkbox("AIM ON TEAMMATES", ref aimOnTeam);
            ImGui.SliderFloat("RADIUS FOV", ref FOV, 10, 300); // buat ganti radius FOV
            if (ImGui.CollapsingHeader("FOV CIRCLE COLOR"))
                ImGui.ColorPicker4("##circlecolor", ref circleColor); // buat ganti warna FOV

            // draw circle
            DrawOVerlay();
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColor));
        }

        // overlay window
        void DrawOVerlay()
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.Begin("OVERLAY", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );
        }
    }
}
