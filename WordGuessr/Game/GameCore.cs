using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using WordGuessr.Game.Models;

namespace WordGuessr.Game;

public class GameCore
{
    private const int DefaultHealth = 3;

    private readonly Question[] _questions;
    private int _currentQuestionIndex = -1;

    public int Health { get; private set; } = DefaultHealth;
    public int GuessedWordsCount { get; private set; }
    public Question? CurrentQuestion { get; private set; }
    public Keyboard? CurrentWordKeyboard { get; private set; }
    public GuessedWord? TypedWord { get; private set; }

    public GameCore()
    {
        var questionCollection = App.Connection.Database!.GetCollection<Question>("Questions");
        _questions = questionCollection.Find(x => true).ToList().ToArray();
        var rnd = new Random();
        rnd.Shuffle(_questions);

        if (_questions.Length == 0)
            throw new Exception("No questions found in database");
    }

    public void NextWord()
    {
        if (TypedWord is not null && !TypedWord.IsGuessed && CurrentQuestion is not null)
            Health--;

        if (Health == 0)
        {
            ResetGame();
            throw new Exception("Game ended");
        }

        _currentQuestionIndex++;
        if (_currentQuestionIndex >= _questions.Length)
        {
            _currentQuestionIndex = 0;
            var rnd = new Random();
            rnd.Shuffle(_questions);
            while (_questions[_currentQuestionIndex] == CurrentQuestion)
            {
                rnd.Shuffle(_questions);
            }
        }

        CurrentQuestion = _questions[_currentQuestionIndex];
        CurrentWordKeyboard = new Keyboard(CurrentQuestion.Answer);
        TypedWord = new GuessedWord(CurrentQuestion.Answer.Length);
    }


    public (bool, IsCorrectCharacter[]) GuessWord()
    {
        if (TypedWord is null || CurrentQuestion is null)
            return (false, Array.Empty<IsCorrectCharacter>());

        List<IsCorrectCharacter> corrects = new();

        if (TypedWord.Word.Length == 0)
        {
            Health--;
            if (Health != 0)
                return (false,
                    new IsCorrectCharacter[CurrentQuestion.Answer.Length]
                        .Select(x => IsCorrectCharacter.Incorrect)
                        .ToArray());
            ResetGame();
            throw new Exception("Game ended");
        }

        if (TypedWord.Word.Length < CurrentQuestion.Answer.Length)
        {
            for (int i = 0; i < TypedWord.Word.Length; i++)
                corrects.Add(IsCorrectCharacter.Neutral);

            while (corrects.Count != CurrentQuestion.Answer.Length)
                corrects.Add(IsCorrectCharacter.Incorrect);

            return (false, corrects.ToArray());
        }

        var correctAnswer = CurrentQuestion.Answer.ToUpper();
        corrects.AddRange(TypedWord.Word.Select((t, i) => t == correctAnswer[i]
            ? IsCorrectCharacter.Correct
            : IsCorrectCharacter.Incorrect));

        if (corrects.All(x => x == IsCorrectCharacter.Incorrect))
        {
            Health--;
            if (Health != 0) return (false, corrects.ToArray());
            ResetGame();
            throw new Exception("Game ended");
        }

        if (corrects.Any(x => x != IsCorrectCharacter.Correct)) return (false, corrects.ToArray());
        Health++;
        GuessedWordsCount++;
        TypedWord.IsGuessed = true;
        NextWord();
        return (true, corrects.ToArray());
    }

    private void ResetGame()
    {
        Health = DefaultHealth + 1;
        GuessedWordsCount = 0;
        NextWord();
    }
}