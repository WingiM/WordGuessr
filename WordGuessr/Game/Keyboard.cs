using System;
using System.Collections.Generic;

namespace WordGuessr.Game;

public class Keyboard
{
    private const int CharacterCount = 40;

    public IReadOnlyList<char> KeyboardChars { get; }

    public Keyboard(string word)
    {
        if (word.Length > CharacterCount)
            throw new NotSupportedException("Слово больше чем клавиаутра");

        var chars = new char[CharacterCount];
        var upperWord = word.ToUpper();
        for (int i = 0; i < word.Length; i++)
        {
            chars[i] = upperWord[i];
        }

        var random = new Random();
        for (int i = word.Length; i < CharacterCount; i++)
        {
            chars[i] = (char)random.Next('А', 'Я' + 1);
        }

        random.Shuffle(chars);
        KeyboardChars = chars;
    }
}