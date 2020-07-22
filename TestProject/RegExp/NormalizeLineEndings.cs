using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace TestProject.RegExp
{
    public class NormalizeLineEndings
    {
        public static string NormalizeLineEndingsLoop(IEnumerable<char> input)
        {
            var output = new StringBuilder();
            var lastWasCR = false;
            foreach (var c in input)
            {
                if (lastWasCR)
                {
                    output.Append("\r\n");
                    switch (c)
                    {
                        case '\r':
                            break;
                        case '\n':
                            lastWasCR = false;
                            break;
                        default:
                            output.Append(c);
                            lastWasCR = false;
                            break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '\r':
                            lastWasCR = true;
                            break;
                        case '\n':
                            output.Append("\r\n");
                            break;
                        default:
                            output.Append(c);
                            break;
                    }
                }
            }
            
            if (lastWasCR)
            {
                output.Append("\r\n");
            }

            return output.ToString();
        }
        
        [Fact]
        public void LoopNormalizesToCRLF()
        {
            NormalizeLineEndingsLoop("a\r\nb\nc\rd")
                .ShouldBe("a\r\nb\r\nc\r\nd");
        }
        
        [Fact]
        public void ThreeReplacesNormalizeToCRLF()
        {
            "a\r\nb\nc\rd"
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", "\r\n")
                .ShouldBe("a\r\nb\r\nc\r\nd");
        }

        [Fact]
        public void FirstRegExpNormalizesToCRLF()
        {
            Regex.Replace("a\r\nb\nc\rd",
                    "\r?\n|\r", "\r\n")
                .ShouldBe("a\r\nb\r\nc\r\nd");
        }

        [Fact]
        public void FirstRegExpMatchesThreeTimes()
        {
            Regex.Matches("a\r\nb\nc\rd",
                    "\r?\n|\r")
                .Count
                .ShouldBe(3);
        }

        [Fact]
        public void SecondRegExpNormalizesToCRLF()
        {
            Regex.Replace("a\r\nb\nc\rd",
                    "(?<!\r)\n|\r(?!\n)", "\r\n")
                .ShouldBe("a\r\nb\r\nc\r\nd");
        }

        [Fact]
        public void SecondRegExpMatchesTwoTimes()
        {
            Regex.Matches("a\r\nb\nc\rd",
                    "(?<!\r)\n|\r(?!\n)")
                .Count
                .ShouldBe(2);
        }

        [Fact]
        public void TwoReplacesNormalizeToLF()
        {
            "a\r\nb\nc\rd"
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .ShouldBe("a\nb\nc\nd");
        }
    }
}