using System.Globalization;
using System.Text;

namespace Apicalypse.NamingPolicies
{
    internal sealed class PascalCaseNamingPolicy : NamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            StringBuilder resultBuilder = new StringBuilder();

            foreach (char c in name)
            {
                // Replace anything, but letters and digits, with space
                if (!char.IsLetterOrDigit(c))
                {
                    resultBuilder.Append(" ");
                }
                else if (char.IsUpper(c))
                {
                    resultBuilder.Append(" ");
                    resultBuilder.Append(c);
                }
                else
                {
                    resultBuilder.Append(c);
                }
            }

            string result = resultBuilder.ToString();

            // Make result string all lowercase, because ToTitleCase does not change all uppercase correctly
            result = result.ToLower();

            // Creates a TextInfo based on the "en-US" culture.
            TextInfo myTI = CultureInfo.InvariantCulture.TextInfo;

            result = myTI.ToTitleCase(result).Replace(" ", string.Empty);

            return result;
        }
    }
}
