using System.Linq;
using Game339.Shared.Diagnostics;

namespace Game339.Shared.Services.Implementation
{
    public class StringService : IStringService
    {
        private readonly IGameLog _log;

        public StringService(IGameLog log)
        {
            _log = log;
        }

        public string Reverse(string input)
        {
            var output = new string(input.Reverse().ToArray());
            _log.Info($"{nameof(StringService)}.{nameof(Reverse)} - {nameof(input)}: {input} - {nameof(output)}: {output}");
            return output;
        }

        public string ReverseWords(string input)
        {
            var words = input.Split(' ');
            var trailingPunctuation = "";

            // Strip trailing punctuation from the last word
            if (words.Length > 0)
            {
                var lastWord = words[words.Length - 1];
                while (lastWord.Length > 0 && char.IsPunctuation(lastWord[lastWord.Length - 1]))
                {
                    trailingPunctuation = lastWord[lastWord.Length - 1] + trailingPunctuation;
                    lastWord = lastWord.Substring(0, lastWord.Length - 1);
                }
                words[words.Length - 1] = lastWord;
            }

            var reversed = words.Reverse().ToArray();
            var output = string.Join(" ", reversed) + trailingPunctuation;
            _log.Info($"{nameof(StringService)}.{nameof(ReverseWords)} - {nameof(input)}: {input} - {nameof(output)}: {output}");
            return output;
        }
    }
}