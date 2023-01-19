﻿using System;
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
    private Stack<char> _currentTryCharacters;
    private bool _isCurrentWordGuessed;

    public int Health { get; private set; } = DefaultHealth;
    public int GuessedWordsCount { get; private set; }
    public Question? CurrentQuestion { get; private set; }
    public Keyboard? CurrentWordKeyboard { get; private set; }
    public string CurrentTryWord => string.Concat(_currentTryCharacters.Reverse());

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
        if (!_isCurrentWordGuessed && CurrentQuestion is not null)
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
        _currentTryCharacters = new Stack<char>();
        _isCurrentWordGuessed = false;
    }

    public bool GuessCharacter(char character)
    {
        if (_currentTryCharacters.Count == CurrentQuestion!.Answer.Length)
            return false;
        _currentTryCharacters.Push(character);
        return true;
    }

    public (bool, IsCorrectCharacter[]) GuessWord()
    {
        List<IsCorrectCharacter> corrects = new();

        if (CurrentTryWord.Length == 0)
        {
            Health--;
            if (Health == 0)
            {
                ResetGame();
                throw new Exception("Game ended");
            }

            return (false,
                new IsCorrectCharacter[CurrentQuestion.Answer.Length]
                    .Select(x => IsCorrectCharacter.Incorrect)
                    .ToArray());
        }

        if (CurrentTryWord.Length < CurrentQuestion.Answer.Length)
        {
            for (int i = 0; i < CurrentTryWord.Length; i++)
                corrects.Add(IsCorrectCharacter.Neutral);

            while (corrects.Count != CurrentQuestion.Answer.Length)
                corrects.Add(IsCorrectCharacter.Incorrect);

            return (false, corrects.ToArray());
        }

        var correctAnswer = CurrentQuestion.Answer.ToUpper();
        for (int i = 0; i < CurrentTryWord.Length; i++)
        {
            corrects.Add(CurrentTryWord[i] == correctAnswer[i]
                ? IsCorrectCharacter.Correct
                : IsCorrectCharacter.Incorrect);
        }

        if (corrects.All(x => x == IsCorrectCharacter.Incorrect))
        {
            Health--;
            if (Health == 0)
            {
                ResetGame();
                throw new Exception("Game ended");
            }

            return (false, corrects.ToArray());
        }

        if (corrects.All(x => x == IsCorrectCharacter.Correct))
        {
            Health++;
            GuessedWordsCount++;
            _isCurrentWordGuessed = true;
            NextWord();
            return (true, corrects.ToArray());
        }

        return (false, corrects.ToArray());
    }

    public bool ResetCharacter()
    {
        return _currentTryCharacters.TryPop(out _);
    }

    private void ResetGame()
    {
        Health = DefaultHealth + 1;
        GuessedWordsCount = 0;
        NextWord();
    }
}