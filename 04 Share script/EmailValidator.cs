using System.Text.RegularExpressions;

public class EmailValidator
{
    private const string EmailRegexPattern =
        @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

    private const string PasswordRegexPattern = @"^[a-zA-Z0-9]+$";

    public static bool IsValidEmail(string emailAddress)
    {
        if (string.IsNullOrEmpty(emailAddress))
        {
            return false;
        }

        var regex = new Regex(EmailRegexPattern);
        return regex.IsMatch(emailAddress.Trim());
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        var regex = new Regex(PasswordRegexPattern);
        return password.Length >= 6 && regex.IsMatch(password);
    }
}
