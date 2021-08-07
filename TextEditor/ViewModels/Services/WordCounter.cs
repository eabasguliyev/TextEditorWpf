using System;
using System.Linq;
using System.Net.Mime;

namespace TextEditor.ViewModels.Services
{
    public class WordCounter
    {
        public int GetWordCount(string text)
        {
            return Counter(SplitToWords(ClearText(text)));
        }
        private string ClearText(string text)
        {
            var replaced = text.Replace("\r\n", " ");

            foreach (var t in text.Where(char.IsPunctuation))
            {
                replaced = text.Replace(t, ' ');
            }

            return replaced;
        }

        private string[] SplitToWords(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private int Counter(string[] words)
        {
            return words.Count(w =>
            {
                if (w.Length == 1 && (char.IsDigit(w[0]) || char.IsSeparator(w[0]) ||
                                      char.IsNumber(w[0]) || char.IsDigit(w[0]) ||
                                      char.IsPunctuation(w[0]) || char.IsSymbol(w[0])))
                    return false;
                return true;
            });
        }
    }
}