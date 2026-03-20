using System;
using System.Text.RegularExpressions;

namespace API.Utilities;

public enum PasswordScore
{
    Blank = 0,
    VeryWeak = 1,
    Weak = 2,
    Medium = 3,
    Strong = 4,
    VeryStrong = 5
}

public class PasswordUtils
{
    public (bool IsStrong, string Message) CheckStrength(string password)
    {
        string passwordRequirements = "Required: ";
        int score = 0;

        if (password.Length >= 12) score++;
        else passwordRequirements += " Atleast 12 Characters, ";

        //has both uppercase and lowercase letters
        if (Regex.Match(password, @"([a-z].*[A-Z])|([A-Z].*[a-z])", RegexOptions.ECMAScript).Success) score++;
        else passwordRequirements += "Lowercase & Uppercase, ";

        //Contains a digit
        if (Regex.Match(password, @"([0-9])", RegexOptions.ECMAScript).Success) score++;
        else passwordRequirements += "Number, ";

        //Contains a special character 
        if (Regex.Match(password, @"[^a-zA-Z0-9]", RegexOptions.ECMAScript).Success) score++;
        else passwordRequirements += "Special Character, ";

        PasswordScore passwordScore = (PasswordScore)Math.Clamp(score, 0, (int)PasswordScore.VeryStrong);

        bool isStrong = passwordScore >= PasswordScore.Strong;
        string message = isStrong
            ? $"Password is {passwordScore}."
            : $"Password is {passwordScore}, {passwordRequirements.TrimEnd(',', ' ')}";

        return (isStrong, message);
    }
}
