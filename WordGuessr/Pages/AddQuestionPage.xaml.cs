using System.Windows;
using System.Windows.Controls;
using WordGuessr.Game;
using WordGuessr.Game.Models;

namespace WordGuessr.Pages;

public partial class AddQuestionPage : Page
{
    public AddQuestionPage()
    {
        InitializeComponent();
    }

    private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
    {
        var question = new Question { QuestionBody = QuestionTb.Text, Answer = AnswerTb.Text };
        var validationResult = QuestionValidator.ValidateQuestion(question);
        if (!validationResult.Item1)
        {
            MessageBox.Show(validationResult.Item2);
            return;
        }

        var questionCollection = App.Connection.Database!.GetCollection<Question>("Questions");
        questionCollection.InsertOne(question);
        MessageBox.Show("Ok!");
        ResetFields();
    }

    private void ResetFields()
    {
        QuestionTb.Text = "";
        AnswerTb.Text = "";
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService!.GoBack();
    }
}