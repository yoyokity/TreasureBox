using System.Numerics;
using ECommons.GameHelpers;

namespace TreasureBox.Helper;

public class PosHelper
{
    public unsafe static void TPpos(Vector3 pos)
    {
        Player.IGameObject->SetPosition(pos.X, pos.Y, pos.Z);
    }

    public static Vector3 GetPos => Player.Position;
}