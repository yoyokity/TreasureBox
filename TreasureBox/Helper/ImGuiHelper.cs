using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using ImGuiNET;

namespace TreasureBox.Helper;

public static class ImGuiHelper
{
    private static int _id = 0;

    public static int GetId()
    {
        return _id++;
    }

    public static void TextColor(Color color, string text)
    {
        var vec = new Vector4()
        {
            X = color.R / 255f,
            Y = color.G / 255f,
            Z = color.B / 255f,
            W = color.A / 255f
        };
        ImGui.TextColored(vec, text);
    }

    public static bool ImageButton(string label, Vector2 size, ref bool buttonValue)
    {
        var ret = false;
        if (buttonValue)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 136 / 255f, 221 / 255f, 0.5f));
            if (ImGui.Button(label, size))
            {
                ret = true;
                buttonValue = !buttonValue;
            }

            ImGui.PopStyleColor(1);
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(173 / 255f, 173 / 255f, 173 / 255f, 0.5f));
            if (ImGui.Button(label, size))
            {
                ret = true;
                buttonValue = !buttonValue;
            }

            ImGui.PopStyleColor(1);
        }

        return ret;
    }


    public static void ToggleButton(string id, ref bool v)
    {
        var p = ImGui.GetCursorScreenPos();
        var drawList = ImGui.GetWindowDrawList();
        float height = ImGui.GetFrameHeight();
        float width = height * 1.55f;
        float radius = height * 0.50f;
        ImGui.InvisibleButton(id, new Vector2(width, height));
        if (ImGui.IsItemClicked())
        {
            v = !v;
        }

        Vector4 col_bg;
        if (ImGui.IsItemHovered())
        {
            col_bg = v
                ? new Vector4((145 + 20) / 255f, 211 / 255f, (68 + 20) / 255f, 1f)
                : new Vector4((218 - 20) / 255f, (218 - 20) / 255f, (218 - 20) / 255f, 1f);
        }
        else
        {
            col_bg = v
                ? new Vector4(145 / 255f, 211 / 255f, 68 / 255f, 1f)
                : new Vector4(218 / 255f, 218 / 255f, 218 / 255f, 1f);
        }

        drawList.AddRectFilled(p, new Vector2(p.X + width, p.Y + height), ImGui.GetColorU32(col_bg), height * 0.5f);
        drawList.AddCircleFilled(new Vector2(p.X + radius + (v ? 1 : 0) * (width - radius * 2.0f), p.Y + radius),
            radius - 1.5f, ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f)));
        drawList.AddText(new Vector2(p.X + width + 2f, p.Y), ImGui.GetColorU32(ImGuiCol.Text), id);
    }

    public static void SetHoverTooltip(string msg)
    {
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.SetTooltip(msg);
        }
    }

    /// <summary>
    /// 一个带上下间距的分割线
    /// </summary>
    /// <param name="topHeight">上间距</param>
    /// <param name="bottomHeight">下间距</param>
    public static void Separator(uint topHeight = 7, uint bottomHeight = 7)
    {
        ImGui.Dummy(new Vector2(0, topHeight));
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0, bottomHeight));
    }

    /// 右键点击
    public static bool IsRightMouseClicked()
    {
        return ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) &&
               ImGui.IsMouseClicked(ImGuiMouseButton.Right);
    }

    public static void Space(uint line)
    {
        for (int i = 0; i < line; i++)
        {
            ImGui.Spacing();
        }
    }


    public static void LeftInputText(string label, ref string text, uint maxLength = 200,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        text ??= string.Empty;
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + label);
        if (maxLength != 200)
            ImGui.SetNextItemWidth(maxLength);
        ImGui.InputText("", ref text, maxLength, flags);
        ImGui.PopID();
    }

    public static void LeftInputInt(string label, ref int value, int step = 1)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + label);
        ImGui.SetNextItemWidth(150);
        ImGui.InputInt("", ref value, step);
        ImGui.PopID();
    }

    public static void LeftInputInt(string label, ref uint value, int step = 1)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + label);
        ImGui.SetNextItemWidth(150);
        int intValue = (int)value;
        ImGui.InputInt("", ref intValue, step);
        value = (uint)intValue;
        ImGui.PopID();
    }

    public static bool LeftInputInt(string label, ref int value, int min, int max, int step = 1)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + label);
        ImGui.SetNextItemWidth(150);
        var ret = ImGui.InputInt("", ref value, step);
        value = Math.Clamp(value, min, max);
        ImGui.PopID();
        return ret;
    }

    public static void LeftInputFloat(string label, ref float value, float step = 0.1f)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + label);
        ImGui.SetNextItemWidth(150);
        ImGui.InputFloat("", ref value, step);
        ImGui.PopID();
    }

    public static void LeftInputFloatwithLabel(string label, ref float value, float step = 0.1f, int width = 200)
    {
        float targetPosX = ImGui.GetCursorPosX() + width;
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + label);
        ImGui.SameLine(targetPosX);
        ImGui.SetNextItemWidth(150);
        ImGui.InputFloat("", ref value, step);
        ImGui.PopID();
    }

    public static void LeftInputFloat(string label, ref float value, float min, float max, float step = 0.1f)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + label);
        ImGui.SetNextItemWidth(150);
        ImGui.InputFloat("", ref value, step);
        value = Math.Clamp(value, min, max);
        ImGui.PopID();
    }

    public static void LeftCombo(string label, ref int select, string[] items, int width = 200)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + label);
        ImGui.SetNextItemWidth(width);
        ImGui.Combo("", ref select, items, items.Length);
        ImGui.PopID();
    }

    public static void LeftCombowithLabel(string label, ref int select, string[] items, int width = 200,
        int maxLength = 0)
    {
        float targetPosX = ImGui.GetCursorPosX() + width;
        ImGui.PushID("##" + label);
        ImGui.SetNextItemWidth(width);
        ImGui.Combo("", ref select, items, items.Length);
        ImGui.PopID();
        ImGui.SameLine(targetPosX);
        if (maxLength > 0)
        {
            string alignedLabel = label.PadRight(maxLength);
            ImGui.Text(alignedLabel);
        }
        else
        {
            ImGui.Text(label);
        }
    }

    public static void LeftCombo(int id, string label, ref int select, string[] items, int size = 200)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.PushID("##" + id);
        ImGui.SetNextItemWidth(size);
        ImGui.Combo("", ref select, items, items.Length);
        ImGui.PopID();
    }


    private static Dictionary<Type, string[]> _allEnumNames = new();
    private static Dictionary<Type, int[]> _allEnumValues = new();

    public static void DrawEnum<T>(int id, string label, ref T value, int size = 200,
        Dictionary<string, string> nameMap = null) where T : struct, Enum
    {
        var enumType = typeof(T);
        if (!_allEnumNames.ContainsKey(enumType))
            _allEnumNames[enumType] = Enum.GetNames<T>();

        if (nameMap != null)
        {
            for (int i = 0; i < _allEnumNames[enumType].Length; i++)
            {
                if (nameMap.TryGetValue(_allEnumNames[enumType][i], out var mapName))
                {
                    _allEnumNames[enumType][i] = mapName;
                }
            }
        }

        if (!_allEnumValues.ContainsKey(enumType))
        {
            var enums = Enum.GetValues<T>();
            int[] array = new int[enums.Length];
            for (int i = 0; i < enums.Length; i++)
            {
                array[i] = Convert.ToInt32(enums[i]);
            }

            _allEnumValues[enumType] = array;
        }

        int targetValue = Convert.ToInt32(value);
        int selected = Array.FindIndex(_allEnumValues[enumType], x => x == targetValue);

        LeftCombo(id, label, ref selected, _allEnumNames[enumType], size);
        if (selected >= 0 && selected < _allEnumValues[enumType].Length)
        {
            value = Enum.Parse<T>(_allEnumValues[enumType][selected].ToString());
        }
    }

    public static void DrawEnum<T>(string label, ref T value, int size = 200,
        Dictionary<string, string> nameMap = null) where T : struct, Enum
    {
        DrawEnum(label.GetHashCode(), label, ref value, size, nameMap);
    }

    public static void DrawEnum(string label, ref object value, int size = 200)
    {
        var enumType = value.GetType();
        if (!_allEnumNames.ContainsKey(enumType))
            _allEnumNames[enumType] = Enum.GetNames(enumType);

        if (!_allEnumValues.ContainsKey(enumType))
        {
            var enums = Enum.GetValues(enumType);
            int[] array = new int[enums.Length];
            for (int i = 0; i < enums.Length; i++)
            {
                array[i] = Convert.ToInt32(enums.GetValue(i));
            }

            _allEnumValues[enumType] = array;
        }

        int targetValue = Convert.ToInt32(value);
        int selected = Array.FindIndex(_allEnumValues[enumType], x => x == targetValue);

        LeftCombo(label.GetHashCode(), label, ref selected, _allEnumNames[enumType], size);
        if (selected >= 0 && selected < _allEnumValues[enumType].Length)
        {
            value = Enum.Parse(enumType, _allEnumValues[enumType][selected].ToString());
        }
    }

    public static void KeepWindowInSight()
    {
        Vector2 pt = ImGui.GetWindowPos();
        Vector2 szy = ImGui.GetWindowSize();
        bool moved = false;
        Vector2 szx = ImGui.GetIO().DisplaySize;
        if (szy.X > szx.X || szy.Y > szx.Y)
        {
            szy.X = Math.Min(szy.X, szx.X);
            szy.Y = Math.Min(szy.Y, szx.Y);
            ImGui.SetWindowSize(szy);
        }

        if (pt.X < 0)
        {
            pt.X += (0.0f - pt.X) / 5.0f;
            moved = true;
        }

        if (pt.Y < 0)
        {
            pt.Y += (0.0f - pt.Y) / 5.0f;
            moved = true;
        }

        if (pt.X + szy.X > szx.X)
        {
            pt.X -= ((pt.X + szy.X) - szx.X) / 5.0f;
            moved = true;
        }

        if (pt.Y + szy.Y > szx.Y)
        {
            pt.Y -= ((pt.Y + szy.Y) - szx.Y) / 5.0f;
            moved = true;
        }

        if (moved == true)
        {
            ImGui.SetWindowPos(pt);
        }
    }

    /// <summary>
    /// 超链接文字
    /// </summary>
    /// <param name="label">显示文本</param>
    /// <param name="link">链接</param>
    /// <param name="tooltip">提示文本</param>
    /// <param name="color">文本颜色,默认null为imgui文本颜色</param>
    public static void LinkText(string label, string link, string tooltip = "", Color? color = null)
    {
        if (color == null)
        {
            ImGui.Text(label);
        }
        else
        {
            var _color = color.Value;
            var vec = new Vector4()
            {
                X = _color.R / 255f,
                Y = _color.G / 255f,
                Z = _color.B / 255f,
                W = _color.A / 255f
            };
            ImGui.TextColored(vec, label);
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            if (tooltip != "")
            {
                ImGui.SetTooltip(tooltip);
            }

            if (ImGui.IsItemClicked())
            {
                if (!(link.StartsWith("https://") || link.StartsWith("http://")))
                    link = "https://" + link;
                // 打开网站链接
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", link);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }
    }
}