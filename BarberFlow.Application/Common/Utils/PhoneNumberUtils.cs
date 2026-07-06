namespace BarberFlow.Application.Common.Utils
{
    public static class PhoneNumberUtils
    {
        public static string NormalizePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }
    }
}
