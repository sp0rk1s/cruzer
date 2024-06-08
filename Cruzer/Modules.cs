using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Modules {
class Sequence {
    public string process;
    public string subprocess;
    private string? endMessage;
    private int longest = 0;
    private string status = "loading";
    private int step = 0;
    private interface StepAnimation {
        ConsoleColor Color {get; set;}
        string text {get; set;}
    }
    private readonly string[][] steps = [
        ["Yellow", "*", "DarkYellow", "*", "Yellow", "*     "],
        ["Yellow", " *", "DarkYellow", "*", "Yellow", "*    "],
        ["Yellow", "  *", "DarkYellow", "*", "Yellow", "*   "],
        ["Yellow", "   *", "DarkYellow", "*", "Yellow", "*  "],
        ["Yellow", "    *", "DarkYellow", "*", "Yellow", "* "],
        ["Yellow", "     *", "DarkYellow", "*", "Yellow", "*"],
        ["Yellow", "      *", "DarkYellow", "*"],
        ["Yellow", "     *", "DarkYellow", "*", "Yellow", "*"],
        ["Yellow", "    *", "DarkYellow", "*", "Yellow", "* "],
        ["Yellow", "   *", "DarkYellow", "*", "Yellow", "*  "],
        ["Yellow", "  *", "DarkYellow", "*", "Yellow", "*   "],
        ["Yellow", " *", "DarkYellow", "*", "Yellow", "*    "],
        ["Yellow", "*", "DarkYellow", "*", "Yellow", "*     "],
        ["DarkYellow", "*", "Yellow", "*      "]
    ];
    public Sequence(string processName, string subprocessName) {
        process = processName;
        subprocess = subprocessName;
        ThreadStart threadStart = new(Loop);
        Thread childThread = new(threadStart);
        childThread.Start();
    }
    public void End(string endText, string endType = "ok") {
        endMessage = endText;
        if (endType != "loading") {
            status = endType;
        } else {
            status = "warn";
        }
    }
    private void Loop() {
        Console.CursorVisible = false;
        while (status == "loading") {
            Console.Write("\r[");
            bool bitIsColor = true;
            foreach (string bit in steps[step]) {
                if (bitIsColor) {
                    if (Enum.TryParse(bit, out ConsoleColor consoleColor)) {
                        Console.ForegroundColor = consoleColor;
                    }
                } else {
                    Console.Write(bit);
                }
                bitIsColor = !bitIsColor;
            }
            Console.ResetColor();
            Console.Write($"] {process}");
            if (subprocess != null) {
                int length = process.Length + subprocess.Length + 3;
                if (length < longest) {
                    Console.Write("".PadLeft(longest - length, ' '));
                } else {
                    longest = length;
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" ({subprocess})");
                Console.ResetColor();
            } else {
                int length = process.Length;
                if (length < longest) {
                    Console.Write("".PadLeft(longest - length, ' '));
                } else {
                    longest = length;
                }
            }
            Console.CursorVisible = false;
            step = (step + 1) % 14;
            Thread.Sleep(50);
        }
        if (status == "ok") {
            Console.Write("\r[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("   OK   ");
            Console.ResetColor();
            Console.Write($"] {endMessage}");
        } else if (status == "info") {
            Console.Write("\r[");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("  INFO  ");
            Console.ResetColor();
            Console.Write($"] {endMessage}");
        } else if (status == "warn") {
            Console.Write("\r[");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  WARN  ");
            Console.ResetColor();
            Console.Write($"] {endMessage}");
        } else if (status == "depend") {
            Console.Write("\r[");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" DEPEND ");
            Console.ResetColor();
            Console.Write($"] {endMessage}");
        } else if (status == "failed") {
            Console.Write("\r[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" FAILED ");
            Console.ResetColor();
            Console.Write($"] {endMessage}");
        } else {
            Console.Write("\r[");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(status.ToUpper().PadRight(8, ' '));
            Console.ResetColor();
            Console.Write($"] {endMessage}");
        }
        {
            int length = process.Length;
            if (length < longest) {
                Console.WriteLine("".PadLeft(longest - length, ' '));
            } else {
                longest = length;
            }
        }
        Console.CursorVisible = true;
    }
}
}