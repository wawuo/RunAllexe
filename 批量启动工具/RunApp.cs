using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public class RunApp
{
    private TextBox txtLog;
    private CancellationTokenSource cts = new CancellationTokenSource();
    private List<Process> processes = new List<Process>(); // 新增

    public RunApp(TextBox txtLog)
    {
        this.txtLog = txtLog;
    }

    //  public async Task RunAsync()
    //  {
    //      // 在开始时创建一个新的 CancellationTokenSource
    //      cts = new CancellationTokenSource();
    //      // 使用 Task.Run 方法在新线程中运行代码
    //      await Task.Run(async () =>
    //      {
    //          string filePath = "./conf.ini";
    //
    //          // 如果 conf.ini 文件不存在，直接退出方法
    //          if (!File.Exists(filePath))
    //          {
    //              Log($"文件不存在: {filePath}");
    //              return;
    //          }
    //
    //          string[] lines = File.ReadAllLines(filePath);
    //          foreach (string line in lines)
    //          {
    //              // 检查 CancellationToken 是否已经被取消
    //              if (cts.Token.IsCancellationRequested)
    //              {
    //                  Log("操作已被取消");
    //                  return;
    //              }
    //
    //              string[] parts = line.Split('#');
    //              string exePath = parts[0];
    //              int delay = 0;
    //
    //              // 如果有 #，尝试解析延时
    //              if (parts.Length == 2 && !int.TryParse(parts[1], out delay))
    //              {
    //                  Log($"无法解析延时: {parts[1]}");
    //                  continue;
    //              }
    //
    //              // 如果 .exe 文件的路径无效，跳过当前行并处理下一行
    //              if (!File.Exists(exePath))
    //              {
    //                  Log($"无效的路径: {exePath}");
    //                  continue;
    //              }
    //
    //              // 如果有延时，等待指定的时间
    //              if (delay > 0)
    //              {
    //                  try
    //                  {
    //                      await Task.Delay(delay * 1000, cts.Token); // 延时，单位为秒
    //                  }
    //                  catch (TaskCanceledException)
    //                  {
    //                      // 如果任务被取消，直接返回
    //                      Log("操作已被取消");
    //                      return;
    //                  }
    //              }
    //              // 运行 .exe 文件
    //              try
    //              {
    //                  Process.Start(exePath);
    //                  Log($"运行成功:{exePath}");
    //              }
    //              catch (Win32Exception ex)
    //              {
    //                  Log($"无法启动进程:{exePath} {ex.Message}");
    //                  continue;
    //              }
    //          }
    //      });
    //  }

    public async Task RunAsync()
    {
        // 在开始时创建一个新的 CancellationTokenSource
        cts = new CancellationTokenSource();
        // 使用 Task.Run 方法在新线程中运行代码
        await Task.Run(async () =>
        {
            string filePath = "./conf.ini";

            // 如果 conf.ini 文件不存在，直接退出方法
            if (!File.Exists(filePath))
            {
                Log($"文件不存在: {filePath}");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // 检查 CancellationToken 是否已经被取消
                if (cts.Token.IsCancellationRequested)
                {
                    Log("操作已被取消");
                    return;
                }

                string[] parts = line.Split('#');
                string exePath = parts[0];
                int delay = 0;

                // 如果有 #，尝试解析延时
                if (parts.Length == 2 && !int.TryParse(parts[1], out delay))
                {
                    Log($"无法解析延时: {parts[1]}");
                    continue;
                }

                // 如果 .exe 文件的路径无效，跳过当前行并处理下一行
                if (!File.Exists(exePath))
                {
                    Log($"无效的路径: {exePath}");
                    continue;
                }

                // 如果有延时，等待指定的时间
                if (delay > 0)
                {
                    try
                    {
                        await Task.Delay(delay * 1000, cts.Token); // 延时，单位为秒
                    }
                    catch (TaskCanceledException)
                    {
                        // 如果任务被取消，直接返回
                        Log("操作已被取消");
                        return;
                    }
                }
                // 运行 .exe 文件
                try
                {

                    string directory = Path.GetDirectoryName(exePath); // 提取目录路径

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                       FileName = exePath,
                        WorkingDirectory = directory
                    };

                    //Process.Start(startInfo);
                    Process process = Process.Start(startInfo); // 修改
                    //WorkingDirectory = @{exePath};
                    processes.Add(process); // 新增

                    Log($"运行成功:{exePath}");
                }
                catch (Win32Exception ex)
                {
                    Log($"无法启动进程:{exePath} {ex.Message}");
                    continue;
                }
            }
        });
    }


    public void Stop()
    {
        // 取消 CancellationToken
        cts.Cancel();
        return;

    }
    
    public void StopAllProcesses() // 新增
        {
            foreach (var process in processes)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            processes.Clear();
        }
    

    private void Log(string message)
    {
        // 获取当前的日期，并格式化为 yyyy-MM-dd 的形式
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 检查 ./log 文件夹是否存在，如果不存在则创建它
        string logDirectory = "./log";
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // 将错误信息添加到主界面的文本框
        if (this.txtLog.InvokeRequired)
        {
            this.txtLog.Invoke(new Action(() => this.txtLog.AppendText($"{dateTime} {message}" + Environment.NewLine)));
        }
        else
        {
            this.txtLog.AppendText($"{dateTime} {message}" + Environment.NewLine);
        }

        // 将错误信息追加到按日期命名的 log 文件
        File.AppendAllText($"{logDirectory}/log-{date}.txt", $"{dateTime} {message}" + Environment.NewLine);
    }
}
