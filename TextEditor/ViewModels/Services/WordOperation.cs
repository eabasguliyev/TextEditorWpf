using System;
using System.Collections.Generic;
using System.Linq;

namespace TextEditor.ViewModels.Services
{
    public class WordOperation
    {
        public int GetWordCount(string text)
        {
            return SplitToWords(ClearText(text)).Count;
        }
        public string ClearText(string text)
        {
            var replaced = text.Replace('\r', ' ').Replace('\n', ' ');

            foreach (var t in replaced.Where(char.IsPunctuation))
            {
                replaced = replaced.Replace(t, ' ');
            }

            return replaced;
        }

        public List<string> SplitToWords(string text, bool potentialWord = false)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w =>
            {
                if (w.Length == 1 && (ValidateCharacter(w[0])))
                    return false;

                if (potentialWord)
                {
                    foreach (var character in w)
                    {
                        if (ValidateCharacter(character))
                            return false;
                    }   
                }
                return true;
            }).ToList();
        }

        private bool ValidateCharacter(char character)
        {
            return char.IsDigit(character) || char.IsSeparator(character) ||
                   char.IsNumber(character) ||
                   char.IsPunctuation(character) || char.IsSymbol(character);
        }
    }
}