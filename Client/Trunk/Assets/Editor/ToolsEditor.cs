using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ToolsEditor
{
    /// <summary>
    /// 复制文件夹所有文件
    /// </summary>
    /// <param name="sourcePath">源目录</param>
    /// <param name="destPath">目的目录</param>
    public static void CopyFolder(string sourcePath, string destPath)
    {
        if (Directory.Exists(sourcePath))
        {
            if (!Directory.Exists(destPath))
            {
                //目标目录不存在则创建
                try
                {
                    Directory.CreateDirectory(destPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("创建目标目录失败：" + ex.Message);
                }
            }
            //获得源文件下所有文件
            List<string> files = new List<string>(Directory.GetFiles(sourcePath));
            files.ForEach(c =>
            {
                string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                string extension = Path.GetExtension(c);
                
                if (extension != ".meta")
                {
                    File.Copy(c, destFile, true);//覆盖模式
                }
            });
            //获得源文件下所有目录文件
            List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
            folders.ForEach(c =>
            {
                string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                //采用递归的方法实现
                CopyFolder(c, destDir);
            });

        }
    }
    /// <summary>
    ///   ExecuteProgram("批处理文件名字", "路径", "参数");
    /// </summary>
    /// <param name="exeFilename"></param>
    /// <param name="workDir"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    static bool ExecuteProgram(string exeFilename, string workDir, string args)
    {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
        info.FileName = exeFilename;
        info.WorkingDirectory = workDir;
        info.UseShellExecute = true;
        info.Arguments = args;
        info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

        System.Diagnostics.Process task = null;
        bool rt = true;
        try
        {
            Debug.Log("ExecuteProgram:" + args);

            task = System.Diagnostics.Process.Start(info);
            if (task != null)
            {
                task.WaitForExit(100000);
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ExecuteProgram:" + e.ToString());
            return false;
        }
        finally
        {
            if (task != null && task.HasExited)
            {
                rt = (task.ExitCode == 0);
            }
        }

        return rt;
    }

}
