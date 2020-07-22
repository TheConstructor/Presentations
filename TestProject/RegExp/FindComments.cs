using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace TestProject.RegExp
{
    public class FindComments
    {
        public static readonly Regex Tokens = new Regex(@"--|/\*|\*/|'|\[|""");

        public static IEnumerable<(int start, int end)> FindCommentsLoop(string input)
        {
            int start = -1, end, level = 0;
            for (var match = Tokens.Match(input); match.Success; match = Tokens.Match(input, end))
            {
                end = match.Index + match.Length;
                if (level == 0)
                {
                    switch (match.Value)
                    {
                        case "--":
                        {
                            end = input.IndexOfAny(new[] {'\r', '\n'}, end);
                            if (end < 0)
                            {
                                end = input.Length;
                            }

                            yield return (match.Index, end);
                            break;
                        }
                        case "/*":
                        {
                            start = match.Index;
                            level = 1;
                            break;
                        }
                        case "'":
                        case "\"":
                        {
                            end = input.IndexOf(match.Value, end, StringComparison.Ordinal) + 1;
                            if (end <= 0)
                            {
                                end = input.Length;
                            }

                            break;
                        }
                        case "[":
                        {
                            end = input.IndexOf(']', end) + 1;
                            if (end <= 0)
                            {
                                end = input.Length;
                            }
                            else
                                while (end < input.Length && input[end] == ']')
                                {
                                    end += 2;
                                }

                            break;
                        }
                    }
                }
                else
                {
                    switch (match.Value)
                    {
                        case "/*":
                        {
                            level++;
                            break;
                        }
                        case "*/":
                        {
                            level--;
                            if (level == 0)
                            {
                                yield return (start, end);
                            }

                            break;
                        }
                    }
                }
            }
        }

        [Theory]
        [InlineData("--Test", new[] {"--Test"})]
        [InlineData("--Test\r\nPRINT '--Test';", new[] {"--Test"})]
        [InlineData("/*Test*/", new[] {"/*Test*/"})]
        [InlineData("/*Test*/PRINT '--Test';", new[] {"/*Test*/"})]
        [InlineData("PRINT '--Test';/*Test*/", new[] {"/*Test*/"})]
        [InlineData("DROP TABLE [--Test];/*Test*/", new[] {"/*Test*/"})]
        [InlineData("DROP TABLE []]--Test];/*Test*/", new[] {"/*Test*/"})]
        [InlineData("/*Test*/DROP TABLE [--Test]]];", new[] {"/*Test*/"})]
        [InlineData("/*/*Test*/*/", new[] {"/*/*Test*/*/"})]
        [InlineData("/*/*Test*/PRINT '--Test';*/", new[] {"/*/*Test*/PRINT '--Test';*/"})]
        public void CommentsAreFound(string input, string[] comments)
        {
            FindCommentsLoop(input)
                .Select(m => input.Substring(m.start, m.end - m.start))
                .ShouldBe(comments);
        }
    }
}