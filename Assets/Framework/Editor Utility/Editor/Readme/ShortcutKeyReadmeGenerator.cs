using UnityEngine;
using UnityEditor;

public class ShortcutKeyReadmeGenerator
{
    [MenuItem("Tools/生成快捷键文档")]
    public static void GenerateShortcutKeyReadme()
    {
        // 直接访问 ShortcutKeyWindow 中的快捷键数据
        var shortcutsArray = ShortcutKeyWindow.shortcuts;

        // 查找是否已有现成的 Readme 资产，否则创建一个新的
        string[] guids = AssetDatabase.FindAssets("t:Readme ShortcutKey");
        Readme readme = null;
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            readme = AssetDatabase.LoadAssetAtPath<Readme>(path);
        }
        else
        {
            readme = ScriptableObject.CreateInstance<Readme>();
            string assetPath = "Assets/Framework/Editor Utility/ShortcutKeyReadme.asset";
            AssetDatabase.CreateAsset(readme, assetPath);
        }

        // 优化文档标题，增加装饰效果
        readme.title = "<size=30><b><color=orange>快捷键全攻略</color></b></size>\n<color=gray>（快速掌握高效操作，助你事半功倍）</color>";

        // 计算总的 section 数量：原始数据 + 额外添加的快捷键
        int extraCount = 3;
        int baseCount = shortcutsArray.Length;
        int totalCount = baseCount + extraCount;
        readme.sections = new Readme.Section[totalCount];

        // 使用已有的快捷键数据，并加上美化富文本
        for (int i = 0; i < baseCount; i++)
        {
            var shortcut = shortcutsArray[i];
            readme.sections[i] = new Readme.Section
            {
                heading = $"<size=18><b>{shortcut.Name}</b></size>",
                text = $"<b>描述:</b> {shortcut.Description}\n<b>快捷键:</b> <color=green>{shortcut.Shortcut}</color>",
                linkText = "",
                url = ""
            };
        }

        // 额外添加一些快捷键（扩展功能示例）
        readme.sections[baseCount + 0] = new Readme.Section
        {
            heading = "<size=18><b>全屏截图</b></size>",
            text = "<b>描述:</b> 快速截取当前屏幕画面\n<b>快捷键:</b> <color=green>Ctrl + Shift + S (Windows) / Command + Shift + S (Mac)</color>",
            linkText = "",
            url = ""
        };
        readme.sections[baseCount + 1] = new Readme.Section
        {
            heading = "<size=18><b>重载场景</b></size>",
            text = "<b>描述:</b> 快速重新加载当前场景，适合调试\n<b>快捷键:</b> <color=green>Ctrl + R (Windows) / Command + R (Mac)</color>",
            linkText = "",
            url = ""
        };
        readme.sections[baseCount + 2] = new Readme.Section
        {
            heading = "<size=18><b>调试模式切换</b></size>",
            text = "<b>描述:</b> 开启或关闭调试模式，显示调试信息\n<b>快捷键:</b> <color=green>Ctrl + D (Windows) / Command + D (Mac)</color>",
            linkText = "",
            url = ""
        };

        EditorUtility.SetDirty(readme);
        AssetDatabase.SaveAssets();
        Debug.Log("快捷键文档已生成或更新！");
    }
}
