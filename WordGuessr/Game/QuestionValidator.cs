using System.Linq;
using WordGuessr.Game.Models;

namespace WordGuessr.Game;

public static class QuestionValidator
{
    public static (bool, string) ValidateQuestion(Question question)
    {
        if (question.QuestionBody.Length < 5)
            return (false, "Слишком короткий вопрос");
        
        if (question.Answer.Length < 3)
            return (false, "Слишком короткий ответ");

        if (!IsValidSymbols(question.Answer))
            return (false, "В ответе присутствуют неправильные символы");

        return (true, "");
    }

    private static bool IsValidSymbols(string value)
    {
        var upper = value.ToUpper();
        return upper.All(character => character is <= 'Я' and >= 'А');
    }
}