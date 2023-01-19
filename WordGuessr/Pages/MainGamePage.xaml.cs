using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WordGuessr.Game;

namespace WordGuessr.Pages;

public partial class MainGamePage : Page
{
    private readonly Stack<Button> _inputHistory = new();
    private List<Label>? _currentWordLabels;
    private readonly GameCore _game;

    public MainGamePage()
    {
        InitializeComponent();
        _game = new GameCore();
        UpdateHealthAndCount();
        NextWord();
    }

    private void NextWord()
    {
        try
        {
            _game.NextWord();
            PrepareWord();
            UpdateHealthAndCount();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            PrepareWord();
            UpdateHealthAndCount();
        }
    }

    private void PrepareWord()
    {
        QuestionTb.Text = _game.CurrentQuestion!.QuestionBody;
        GenerateKeyboard();
        GenerateLabelsForWord();
    }

    private void GenerateLabelsForWord()
    {
        _currentWordLabels = new List<Label>();
        LabelSp.Children.Clear();
        for (int i = 0; i < _game.CurrentQuestion!.Answer.Length; i++)
        {
            var label = new Label
            {
                BorderThickness = new Thickness(3),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderBrush = Brushes.Black,
                Margin = new Thickness(1),
                Height = 50,
                Width = 50
            };

            _currentWordLabels.Add(label);
            LabelSp.Children.Add(label);
        }
    }

    private void GenerateKeyboard()
    {
        KeyboardSp.Children.Clear();
        foreach (var character in _game.CurrentWordKeyboard!.KeyboardChars)
        {
            var btn = new Button
            {
                Content = character
            };

            btn.Click += (sender, _) =>
            {
                var button = (Button)sender;
                if (!_game.GuessCharacter((char)button.Content)) return;
                button.IsEnabled = false;
                _inputHistory.Push(button);
                FillWord();
                ResetColors();
            };

            KeyboardSp.Children.Add(btn);
        }
    }

    private void FillWord()
    {
        for (int i = 0; i < _game.CurrentQuestion.Answer.Length; i++)
        {
            if (i < _game.CurrentTryWord.Length)
                _currentWordLabels![i].Content = _game.CurrentTryWord[i];
            else
                _currentWordLabels![i].Content = "";
        }
    }

    private void CreateNewQuestionButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService!.Navigate(new AddQuestionPage());
    }

    private void NextWordButton_Click(object sender, RoutedEventArgs e)
    {
        NextWord();
    }

    private void ResetLastCharacterButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_inputHistory.TryPop(out var button)) return;

        button.IsEnabled = true;
        _game.ResetCharacter();
        ResetColors();
        FillWord();
    }

    private void GuessAnswerButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var res = _game.GuessWord();
            UpdateHealthAndCount();
            ColorLabels(res.Item2);
            if (!res.Item1) return;
            MessageBox.Show("Вы угадали слово!");
            PrepareWord();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            PrepareWord();
            UpdateHealthAndCount();
        }
    }

    private void ColorLabels(IsCorrectCharacter[] status)
    {
        for (int i = 0; i < _currentWordLabels.Count; i++)
        {
            if (status[i] == IsCorrectCharacter.Correct)
                _currentWordLabels[i].Background = Brushes.Green;
            else if (status[i] == IsCorrectCharacter.Incorrect)
                _currentWordLabels[i].Background = Brushes.Red;
            else
                _currentWordLabels[i].Background = Brushes.White;
        }
    }

    private void ResetColors()
    {
        foreach (var label in _currentWordLabels)
        {
            label.Background = Brushes.White;
        }
    }

    private void UpdateHealthAndCount()
    {
        CountRun.Text = _game.GuessedWordsCount.ToString();
        HpRun.Text = _game.Health.ToString();
    }
}