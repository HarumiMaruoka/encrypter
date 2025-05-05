using System;
using System.IO;
using System.Text;

namespace VigenèreCipher
{
    class Program
    {
        static void Main(string[] args)
        {
            // ユーザーからの入力を格納する変数
            string mode = "";
            string keyString = "";
            string inputFilePath = "";
            string outputFilePath = "";

            // コマンドライン引数の処理
            if (args.Length == 4)
            {
                mode = args[0].ToLower();
                keyString = args[1];
                inputFilePath = args[2];
                outputFilePath = args[3];
            }
            else
            {
                // ユーザーに入力を求める
                Console.WriteLine("モードを入力してください（encrypt/decrypt）:");
                mode = Console.ReadLine().ToLower();

                Console.WriteLine("キーを入力してください:");
                keyString = Console.ReadLine();

                Console.WriteLine("入力ファイルのパスを入力してください:");
                inputFilePath = Console.ReadLine();

                Console.WriteLine("出力ファイルのパスを入力してください:");
                outputFilePath = Console.ReadLine();
            }

            // モードの検証
            if (mode != "encrypt" && mode != "decrypt")
            {
                Console.WriteLine("モードが無効です。'encrypt'または'decrypt'を入力してください。");
                return;
            }

            // 入力ファイルの存在確認
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("入力ファイルが存在しません。");
                return;
            }

            // キーの検証
            if (string.IsNullOrEmpty(keyString))
            {
                Console.WriteLine("キーは空にできません。");
                return;
            }

            // キー文字列をバイト配列に変換
            byte[] keyBytes = Encoding.UTF8.GetBytes(keyString);

            if (keyBytes.Length == 0)
            {
                Console.WriteLine("キーは空にできません。");
                return;
            }

            try
            {
                // ファイルの処理を実行
                ProcessFile(mode, keyBytes, inputFilePath, outputFilePath);
                Console.WriteLine("操作が正常に完了しました。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("エラーが発生しました: " + ex.Message);
            }
        }

        static void ProcessFile(string mode, byte[] keyBytes, string inputFilePath, string outputFilePath)
        {
            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                int keyLength = keyBytes.Length;
                byte[] buffer = new byte[4096]; // 4KBのバッファ
                int bytesRead;
                int keyIndex = 0;

                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // バッファ内の各バイトを処理
                    for (int i = 0; i < bytesRead; i++)
                    {
                        byte inputByte = buffer[i];
                        byte keyByte = keyBytes[keyIndex % keyLength];

                        byte outputByte;

                        if (mode == "encrypt")
                        {
                            outputByte = (byte)((inputByte + keyByte) % 256);
                        }
                        else // decrypt
                        {
                            outputByte = (byte)((inputByte - keyByte + 256) % 256);
                        }

                        buffer[i] = outputByte;
                        keyIndex++;
                    }

                    // 処理したバッファを書き込み
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}