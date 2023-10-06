using System;

namespace Notero.Utilities
{
    public static class DataFormatValidator
    {
        //public const string LessonNameFormat = "#{0} {1}";
        public const string BookNameFormat = "<color=#9F1A1A><b>Book Name:</b></color> {0}";
        public const string ChapterTextFormat = "<color=#9F1A1A><b>Chapter Name:</b></color> {0}";
        public const string HeaderTextFormat = "<color=#F68800>CHAPTER:</color> {0}";
        public const string MissionAmountTextFormat = "<color=#9F1A1A><b>Mission Amount:</b></color> {0}";

        public const string NoDataStringFormat = "None";
        public const int NoDataIntFormat = -1;
        private const int m_MaxStationIdDigits = 2;

        public const int NoneStationId = 0;

        public static string ValidateStringInfo(string info)
        {
            if(string.IsNullOrEmpty(info))
            {
                info = NoDataStringFormat;
            }

            return info;
        }

        public static int ValidatePositiveIntInfo(int num)
        {
            if(num < 0)
            {
                num = 0;
            }

            return num;
        }

        public static string RoundPositiveFloatToString(float num)
        {
            num = ValidatePositiveFloatToInt(num);
            return num.ToString();
        }

        public static int ValidatePositiveStringToInt(string info)
        {
            if(!int.TryParse(info, out int num))
            {
                num = -1;
            }

            return num;
        }

        public static int ValidatePositiveDecimalToInt(decimal num)
        {
            return ValidatePositiveIntInfo((int)num);
        }

        public static int ValidatePositiveFloatToInt(float num)
        {
            return ValidatePositiveIntInfo((int)num);
        }

        public static string StationIdToString(int id)
        {
            return NumberToZeroFrontString(m_MaxStationIdDigits, id);
        }

        /// <summary>
        /// Add zero in front of the number that has digits lower than expected, 
        /// e.g. expected digits is 3, result will be (001) (010) (100)
        /// </summary>
        /// <returns></returns>
        public static string NumberToZeroFrontString(int expectedDigits, int num)
        {
            if(num <= 0)
            {
                return NoDataStringFormat;
            }

            string numStr = num.ToString();
            string zero = new string('0', expectedDigits - numStr.Length);

            return $"{zero}{numStr}";
        }

        public static string SecondsToTimeFormat(float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString("mm':'ss");
        }

        public static string SecondsToTimeFormat(double seconds)
        {
            return SecondsToTimeFormat((float)seconds);
        }

        public static string FloatToDecimalFormat(float number)
        {
            string s = $"{number:0.00}";

            if(s.EndsWith("00"))
            {
                s = ((int)number).ToString();
            }

            return s;
        }

        public static string LessonNodePageFormat(int currentPage, int totalPage)
        {
            if(currentPage > totalPage)
            {
                return $"{currentPage} / {totalPage}";
            }
            else if(totalPage <= 0)
            {
                return NoDataStringFormat;
            }
            else
            {
                return $"{currentPage} / {totalPage}";
            }
        }
    }
}