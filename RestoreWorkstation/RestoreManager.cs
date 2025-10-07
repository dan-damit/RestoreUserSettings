using System.Diagnostics;
using System.IO;

namespace RestoreWorkstation
{
    class RestoreManager
    {
        public RestoreManager()
        {
            Logger.Log("🧩 RestoreManager initialized.");
        }

        // Restores files from the given source path to the current user's profile directory.
        public async Task<bool> RestoreToUserProfileAsync(string sourcePath, IProgress<int>? progress = null)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
                {
                    Logger.Log("⚠ Invalid source path.");
                    return false;
                }

                string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                Logger.Log($"📦 Restoring from: {sourcePath}");
                Logger.Log($"📍 Target user directory: {targetPath}");

                try
                {
                    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                    {
                        string targetDir = dirPath.Replace(sourcePath, targetPath);
                        Directory.CreateDirectory(targetDir);
                    }

                    string[] files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
                    int total = files.Length;
                    int count = 0;

                    foreach (string filePath in files)
                    {
                        string targetFile = filePath.Replace(sourcePath, targetPath);

                        try
                        {
                            File.Copy(filePath, targetFile, overwrite: true);
                            Logger.Log($"✅ Copied: {filePath} → {targetFile}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"⚠ Failed to copy: {filePath} → {targetFile} — {ex.Message}");
                        }

                        count++;
                        int percent = (int)((count / (double)total) * 100);
                        progress?.Report(percent);
                    }

                    Logger.Log("🎉 Restore completed successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Log($"❌ Restore failed: {ex.Message}");
                    return false;
                }
            });
        }

        // Merges all .reg files found in the "RegKeys" subfolder of the given source path into the Windows registry.
        public async Task<bool> MergeRegistryKeysAsync(string sourcePath)
        {
            return await Task.Run(() =>
            {
                string regKeyFolder = Path.Combine(sourcePath, "RegKeys");

                if (!Directory.Exists(regKeyFolder))
                {
                    Logger.Log("⚠ RegKeys folder not found — skipping registry merge.");
                    return false;
                }

                string[] regFiles = Directory.GetFiles(regKeyFolder, "*.reg", SearchOption.TopDirectoryOnly);
                if (regFiles.Length == 0)
                {
                    Logger.Log("⚠ No .reg files found in RegKeys folder.");
                    return false;
                }

                Logger.Log($"🔐 Found {regFiles.Length} registry files to merge.");

                int successCount = 0;
                int failCount = 0;

                foreach (string regFile in regFiles)
                {
                    try
                    {
                        Logger.Log($"📄 Merging: {Path.GetFileName(regFile)}");

                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "reg.exe",
                                Arguments = $"import \"{regFile}\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };

                        process.Start();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            Logger.Log($"✅ Merged: {Path.GetFileName(regFile)}");
                            successCount++;
                        }
                        else
                        {
                            Logger.Log($"⚠ Merge failed: {Path.GetFileName(regFile)} (Exit code: {process.ExitCode})");
                            failCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"⚠ Exception merging {Path.GetFileName(regFile)}: {ex.Message}");
                        failCount++;
                    }
                }

                Logger.Log($"📊 Registry merge summary: {successCount} succeeded, {failCount} failed.");
                return true;
            });
        }
    }
}
