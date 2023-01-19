using System.Collections.Generic;
using System.Linq;

namespace WordGuessr.Game.Models;

public class GuessedWord
{
    private readonly int _answerLength;
    private readonly Stack<char> _currentTryCharacters;
    
    public bool IsGuessed { get; set; }
    public string Word => string.Concat(_currentTryCharacters.Reverse());

    public GuessedWord(int answerLength)
    {
        _currentTryCharacters = new Stack<char>();
        _answerLength = answerLength;
    }

    public bool InsertCharacter(char character)
    {
        if (_currentTryCharacters.Count == _answerLength)
            return false;
        _currentTryCharacters.Push(character);
        return true;
    }
    
    public bool ResetCharacter()
    {
        return _currentTryCharacters.TryPop(out _);
    }
}