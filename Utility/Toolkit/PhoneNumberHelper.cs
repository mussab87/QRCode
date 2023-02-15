using System;
using libphonenumber;
using Utility.Core.Logging;

namespace Utility.Toolkit
{
    public class PhoneNumberHelper
    {
        public static PhoneNumberResult IsValidInternationalNumber(string internationalPhoneNumber)
        {
            return IsValidNumberForMobile(internationalPhoneNumber, null);
        }

        public static PhoneNumberResult IsValidNumberForMobile(string phoneNumber, string regionCode)
        {
            FileTrace.WriteMemberEntry();

            PhoneNumberResult phoneNumberResult = new PhoneNumberResult() { IsValid = true, InvalidReasonDescription = string.Empty };
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    phoneNumberResult.IsValid = false;
                    phoneNumberResult.InvalidReasonDescription = "Phone number is empty";

                    return phoneNumberResult;
                }

                // Check if this is an international number.
                if (phoneNumber.StartsWith("00"))
                {
                    // Number was supplied with international code as 00 prefix.
                    // Replace 00 at beginning with + for libphonenumber to validate.
                    phoneNumber = "+" + phoneNumber.Remove(0, 2);
                }

                if (phoneNumber.StartsWith("+"))
                {
                    try
                    {
                        // Number is in international format with a + prefix.
                        var parsedInternational = PhoneNumberUtil.Instance.Parse(phoneNumber, string.Empty).IsValidNumber;
                        if (!parsedInternational)
                        {
                            phoneNumberResult.IsValid = false;
                            phoneNumberResult.InvalidReasonDescription = string.Format("International phone number '{0}' is not a valid number.", phoneNumber);
                        }
                        else
                        {
                            phoneNumberResult.IsValid = true;
                        }
                    }
                    catch (NumberParseException internationalNPE)
                    {
                        phoneNumberResult.IsValid = false;
                        phoneNumberResult.InvalidReasonDescription = string.Format("Unable to parse international number '{0}'. Format exception was: '{2}'", phoneNumber, regionCode, internationalNPE.Message);
                    }
                }
                else
                {
                    // Try to parse the local number.
                    try
                    {
                        var parsed = PhoneNumberUtil.Instance.Parse(phoneNumber, regionCode);
                        var valid = parsed.IsValidNumber;
                        if (!valid)
                        {
                            if (phoneNumber.StartsWith("05") && phoneNumber.Length == 10)
                            {
                                phoneNumberResult.IsValid = true;
                            }
                            else
                            {
                                phoneNumberResult.IsValid = false;
                                phoneNumberResult.InvalidReasonDescription = string.Format("Local phone number '{0}' is not a valid number.", phoneNumber);
                            }
                        }
                    }
                    catch (NumberParseException regionSuppliedNPE)
                    {
                        phoneNumberResult.IsValid = false;
                        phoneNumberResult.InvalidReasonDescription = string.Format("Unable to parse number '{0}' into region '{1}'. Format exception was: '{2}'", phoneNumber, regionCode, regionSuppliedNPE.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
                throw;
            }
            finally
            {
                FileTrace.WriteMemberExit();
            }
            return phoneNumberResult;
        }

        public static PhoneNumberResult IsValidNumberForWhatsApp(string phoneNumber, string regionCode)
        {
            FileTrace.WriteMemberEntry();

            PhoneNumberResult phoneNumberResult = new PhoneNumberResult() { IsValid = true, InvalidReasonDescription = string.Empty };
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    phoneNumberResult.IsValid = false;
                    phoneNumberResult.InvalidReasonDescription = "Phone number is empty";

                    return phoneNumberResult;
                }

                // Check if this is an international number.
                if (phoneNumber.StartsWith("00"))
                {
                    // Number was supplied with international code as 00 prefix.
                    // Replace 00 at beginning with + for libphonenumber to validate.
                    phoneNumber = "+" + phoneNumber.Remove(0, 2);
                }

                if (phoneNumber.StartsWith("+"))
                {
                    try
                    {
                        // Number is in international format with a + prefix.
                        var parsedInternational = PhoneNumberUtil.Instance.Parse(phoneNumber, string.Empty).IsValidNumber;
                        if (!parsedInternational)
                        {
                            phoneNumberResult.IsValid = false;
                            phoneNumberResult.InvalidReasonDescription = string.Format("International phone number '{0}' is not a valid number.", phoneNumber);
                        }
                    }
                    catch (NumberParseException internationalNPE)
                    {
                        phoneNumberResult.IsValid = false;
                        phoneNumberResult.InvalidReasonDescription = string.Format("Unable to parse international number '{0}'. Format exception was: '{2}'", phoneNumber, regionCode, internationalNPE.Message);
                    }
                }
                else
                {
                    // Try to parse the local number.
                    try
                    {
                        var parsed = PhoneNumberUtil.Instance.Parse(phoneNumber, regionCode);
                        var valid = parsed.IsValidNumber;
                        if (!valid)
                        {
                            phoneNumberResult.IsValid = false;
                            phoneNumberResult.InvalidReasonDescription = string.Format("Local phone number '{0}' is not a valid number.", phoneNumber);
                        }
                    }
                    catch (NumberParseException regionSuppliedNPE)
                    {
                        phoneNumberResult.IsValid = false;
                        phoneNumberResult.InvalidReasonDescription = string.Format("Unable to parse number '{0}' into region '{1}'. Format exception was: '{2}'", phoneNumber, regionCode, regionSuppliedNPE.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
                throw;
            }
            finally
            {
                FileTrace.WriteMemberExit();
            }
            return phoneNumberResult;
        }
    }
}