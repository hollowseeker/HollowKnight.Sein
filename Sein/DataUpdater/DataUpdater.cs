using System.IO;

namespace Sein.DataUpdater;

public class DataUpdater
{
    public static void Run()
    {
        string modPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Hollow Knight\\hollow_knight_Data\\Managed\\Mods\\Custom Knight\\Skins\\Ori";
        DirectoryInfo modDir = new(modPath);
        if (modDir.Exists) modDir.Delete(true);
        modDir.Create();

        var root = InferGitRoot(Directory.GetCurrentDirectory());
        var sourcePath = Path.Combine(root, "Ori");
        CopyAllFiles(sourcePath, modPath);
    }

    private static string InferGitRoot(string path)
    {
        var info = Directory.GetParent(path);
        while (info != null)
        {
            if (Directory.Exists(Path.Combine(info.FullName, ".git"))) return info.FullName;
            info = Directory.GetParent(info.FullName);
        }

        return path;
    }

    private static void CopyAllFiles(string sourcePath, string targetPath)
    {
        // Create all directories.
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

        // Copy all files.
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }
}
