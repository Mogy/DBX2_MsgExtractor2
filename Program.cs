using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DBX2_MsgExtractor2
{
    class Program
    {
        const string DIR_EN_MSG = "PcEnMsg";
        const string DIR_TEMP = "tmp";
        const string MSG_TOOL = "Dragon_Ball_Xenoverse_2_MSG_Tool.exe";
        const string EN_MSG = "enMsg.txt";
        static void Main(string[] args)
        {
            // フォルダ生成
            Directory.CreateDirectory(DIR_EN_MSG);

            // msgTool存在チェック
            if (!File.Exists(MSG_TOOL))
            {
                Console.WriteLine($"{MSG_TOOL} is not found.");
                Console.WriteLine();
                Console.WriteLine("-- please push any key --");
                Console.ReadKey();
                Environment.Exit(1);
            }

            // tempフォルダを再生成
            if (Directory.Exists(DIR_TEMP))
            {
                Directory.Delete(DIR_TEMP, true);
                // 実行完了待ち
                while (Directory.Exists(DIR_TEMP))
                {
                    Thread.Sleep(1);
                }
            }
            Directory.CreateDirectory(DIR_TEMP);

            // 出力ファイルを削除
            if (File.Exists(EN_MSG))
            {
                File.Delete(EN_MSG);
            }

            // msgTool設定
            var msgTool = new ProcessStartInfo();
            msgTool.FileName = MSG_TOOL;
            msgTool.CreateNoWindow = true;
            msgTool.UseShellExecute = false;

            var enFiles = Directory.GetFiles(DIR_EN_MSG, "*_en.msg");
            var currents = 1;

            foreach (string enPath in enFiles)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"{currents++}/{enFiles.Length}");

                // メッセージ出力
                var enMsg = Path.GetFileName(enPath);
                var enTxtPath = Path.Join(DIR_TEMP, Path.ChangeExtension(enMsg, "txt"));
                exportMessage(msgTool, enMsg, enPath, enTxtPath, EN_MSG);
            }

            // tempフォルダを削除
            Directory.Delete(DIR_TEMP, true);

            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"{enFiles.Length} messages extracted.");
            Console.WriteLine();
            Console.WriteLine("-- please push any key --");
            Console.ReadKey();
        }

        static void exportMessage(ProcessStartInfo msgTool, string msg, string msgPath, string txtPath, string exportPath)
        {
            // msgTool実行
            msgTool.Arguments = String.Join(" ", "-e", msgPath, txtPath);
            Process.Start(msgTool).WaitForExit();

            // メッセージからNullを除外
            var data = "";
            using (var sr = new StreamReader(txtPath))
            {
                data = sr.ReadToEnd();
            }
            data = data.Replace("\0", "");
            using (var sw = new StreamWriter(txtPath))
            {
                sw.Write(data);
            }

            // メッセージを抽出
            using (var sw = new StreamWriter(exportPath, true))
            {
                sw.WriteLine($"■■■{msg}");
                sw.Write(data);
            }
        }
    }
}
