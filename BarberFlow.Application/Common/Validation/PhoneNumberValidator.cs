using BarberFlow.Application.Common.Constants;
using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Utils;

namespace BarberFlow.Application.Common.Validation;
public static class PhoneNumberValidator
{
    public static string NormalizeAndValidate(string phoneNumber)
    {
        var normalized = PhoneNumberUtils.NormalizePhoneNumber(phoneNumber);

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ValidationException("Phone number is required.");

        if (normalized.Length < AuthConstants.PhoneNumberMinLength ||
            normalized.Length > AuthConstants.PhoneNumberMaxLength)
        {
            throw new ValidationException("Invalid phone number.");
        }

        return normalized;
    }
}
