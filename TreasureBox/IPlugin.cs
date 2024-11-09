namespace TreasureBox;

public interface IPlugin
{
    /// <summary>
    /// 插件名称
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 插件版本
    /// </summary>
    public string Version { get; }
    /// <summary>
    /// 图标，填""为默认图标，否则自定义，请将自定义图标放到“Resources\img”目录下
    /// </summary>
    public string ImgPath { get; }
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(); 
    /// <summary>
    /// 绘制界面
    /// </summary>
    public void Draw();
}