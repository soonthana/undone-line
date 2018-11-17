using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Undone.Line.Utils
{
  public static class DateTimes
  {
    #region Pubilc Enums and Static Dictionaries - DATE TIME
    public enum TimeZoneDisplayName
    {
      InternationalDateLineWest = 0,
      CoordinatedUniversalTime11 = 1,
      AleutianIslands = 2,
      Hawaii = 3,
      MarquesasIslands = 4,
      Alaska = 5,
      CoordinatedUniversalTime09 = 6,
      BajaCalifornia = 7,
      CoordinatedUniversalTime08 = 8,
      PacificTimeUS_Canada = 9,
      Arizona = 10,
      Chihuahua_LaPaz_Mazatlan = 11,
      MountainTimeUS_Canada = 12,
      CentralAmerica = 13,
      CentralTimeUS_Canada = 14,
      EasterIsland = 15,
      Guadalajara_MexicoCity_Monterrey = 16,
      Saskatchewan = 17,
      Bogota_Lima_Quito_RioBranco = 18,
      Chetumal = 19,
      EasternTimeUS_Canada = 20,
      Haiti = 21,
      Havana = 22,
      IndianaEast = 23,
      Asuncion = 24,
      AtlanticTimeCanada = 25,
      Caracas = 26,
      Cuiaba = 27,
      Georgetown_LaPaz_Manaus_SanJuan = 28,
      Santiago = 29,
      TurksandCaicos = 30,
      Newfoundland = 31,
      Araguaina = 32,
      Brasilia = 33,
      Cayenne_Fortaleza = 34,
      CityofBuenosAires = 35,
      Greenland = 36,
      Montevideo = 37,
      PuntaArenas = 38,
      SaintPierreandMiquelon = 39,
      Salvador = 40,
      CoordinatedUniversalTime02 = 41,
      MidAtlanticOld = 42,
      Azores = 43,
      CaboVerdeIslands = 44,
      CoordinatedUniversalTime = 45,
      Casablanca = 46,
      Dublin_Edinburgh_Lisbon_London = 47,
      Monrovia_Reykjavik = 48,
      Amsterdam_Berlin_Bern_Rome_Stockholm_Vienna = 49,
      Belgrade_Bratislava_Budapest_Ljubljana_Prague = 50,
      Brussels_Copenhagen_Madrid_Paris = 51,
      Sarajevo_Skopje_Warsaw_Zagreb = 52,
      WestCentralAfrica = 53,
      Windhoek = 54,
      Amman = 55,
      Athens_Bucharest = 56,
      Beirut = 57,
      Cairo = 58,
      Chisinau = 59,
      Damascus = 60,
      Gaza_Hebron = 61,
      Harare_Pretoria = 62,
      Helsinki_Kyiv_Riga_Sofia_Tallinn_Vilnius = 63,
      Jerusalem = 64,
      Kaliningrad = 65,
      Tripoli = 66,
      Baghdad = 67,
      Istanbul = 68,
      Kuwait_Riyadh = 69,
      Minsk = 70,
      Moscow_StPetersburg_Volgograd = 71,
      Nairobi = 72,
      Tehran = 73,
      AbuDhabi_Muscat = 74,
      Astrakhan_Ulyanovsk = 75,
      Baku = 76,
      Izhevsk_Samara = 77,
      PortLouis = 78,
      Saratov = 79,
      Tbilisi = 80,
      Yerevan = 81,
      Kabul = 82,
      Ashgabat_Tashkent = 83,
      Ekaterinburg = 84,
      Islamabad_Karachi = 85,
      Chennai_Kolkata_Mumbai_NewDelhi = 86,
      SriJayawardenepura = 87,
      Kathmandu = 88,
      Astana = 89,
      Dhaka = 90,
      Omsk = 91,
      YangonRangoon = 92,
      Bangkok_Hanoi_Jakarta = 93,
      Barnaul_GornoAltaysk = 94,
      Hovd = 95,
      Krasnoyarsk = 96,
      Novosibirsk = 97,
      Tomsk = 98,
      Beijing_Chongqing_HongKong_Urumqi = 99,
      Irkutsk = 100,
      KualaLumpur_Singapore = 101,
      Perth = 102,
      Taipei = 103,
      Ulaanbaatar = 104,
      Pyongyang = 105,
      Eucla = 106,
      Chita = 107,
      Osaka_Sapporo_Tokyo = 108,
      Seoul = 109,
      Yakutsk = 110,
      Adelaide = 111,
      Darwin = 112,
      Brisbane = 113,
      Canberra_Melbourne_Sydney = 114,
      Guam_PortMoresby = 115,
      Hobart = 116,
      Vladivostok = 117,
      LordHoweIsland = 118,
      BougainvilleIsland = 119,
      Chokurdakh = 120,
      Magadan = 121,
      NorfolkIsland = 122,
      Sakhalin = 123,
      SolomonIslands_NewCaledonia = 124,
      Anadyr_PetropavlovskKamchatsky = 125,
      Auckland_Wellington = 126,
      CoordinatedUniversalTime12 = 127,
      Fiji = 128,
      PetropavlovskKamchatskyOld = 129,
      ChathamIslands = 130,
      CoordinatedUniversalTime13 = 131,
      Nukualofa = 132,
      Samoa = 133,
      KiritimatiIsland = 134
    }

    public static Dictionary<int, string> TimeZoneDisplayNames = new Dictionary<int, string>
        {
            {0, "Dateline Standard Time"},
            {1, "UTC-11"},
            {2, "Aleutian Standard Time"},
            {3, "Hawaiian Standard Time"},
            {4, "Marquesas Standard Time"},
            {5, "Alaskan Standard Time"},
            {6, "UTC-09"},
            {7, "Pacific Standard Time (Mexico)"},
            {8, "UTC-08"},
            {9, "Pacific Standard Time"},
            {10, "US Mountain Standard Time"},
            {11, "Mountain Standard Time (Mexico)"},
            {12, "Mountain Standard Time"},
            {13, "Central America Standard Time"},
            {14, "Central Standard Time"},
            {15, "Easter Island Standard Time"},
            {16, "Central Standard Time (Mexico)"},
            {17, "Canada Central Standard Time"},
            {18, "SA Pacific Standard Time"},
            {19, "Eastern Standard Time (Mexico)"},
            {20, "Eastern Standard Time"},
            {21, "Haiti Standard Time"},
            {22, "Cuba Standard Time"},
            {23, "US Eastern Standard Time"},
            {24, "Paraguay Standard Time"},
            {25, "Atlantic Standard Time"},
            {26, "Venezuela Standard Time"},
            {27, "Central Brazilian Standard Time"},
            {28, "SA Western Standard Time"},
            {29, "Pacific SA Standard Time"},
            {30, "Turks and Caicos Standard Time"},
            {31, "Newfoundland Standard Time"},
            {32, "Tocantins Standard Time"},
            {33, "E. South America Standard Time"},
            {34, "SA Eastern Standard Time"},
            {35, "Argentina Standard Time"},
            {36, "Greenland Standard Time"},
            {37, "Montevideo Standard Time"},
            {38, "Magallanes Standard Time"},
            {39, "Saint Pierre Standard Time"},
            {40, "Bahia Standard Time"},
            {41, "UTC-02"},
            {42, "Mid-Atlantic Standard Time"},
            {43, "Azores Standard Time"},
            {44, "Cabo Verde Standard Time"},
            {45, "Coordinated Universal Time"},
            {46, "Morocco Standard Time"},
            {47, "GMT Standard Time"},
            {48, "Greenwich Standard Time"},
            {49, "W. Europe Standard Time"},
            {50, "Central Europe Standard Time"},
            {51, "Romance Standard Time"},
            {52, "Central European Standard Time"},
            {53, "W. Central Africa Standard Time"},
            {54, "Namibia Standard Time"},
            {55, "Jordan Standard Time"},
            {56, "GTB Standard Time"},
            {57, "Middle East Standard Time"},
            {58, "Egypt Standard Time"},
            {59, "E. Europe Standard Time"},
            {60, "Syria Standard Time"},
            {61, "West Bank Gaza Standard Time"},
            {62, "South Africa Standard Time"},
            {63, "FLE Standard Time"},
            {64, "Jerusalem Standard Time"},
            {65, "Russia TZ 1 Standard Time"},
            {66, "Libya Standard Time"},
            {67, "Arabic Standard Time"},
            {68, "Turkey Standard Time"},
            {69, "Arab Standard Time"},
            {70, "Belarus Standard Time"},
            {71, "Russia TZ 2 Standard Time"},
            {72, "E. Africa Standard Time"},
            {73, "Iran Standard Time"},
            {74, "Arabian Standard Time"},
            {75, "Astrakhan Standard Time"},
            {76, "Azerbaijan Standard Time"},
            {77, "Russia TZ 3 Standard Time"},
            {78, "Mauritius Standard Time"},
            {79, "Saratov Standard Time"},
            {80, "Georgian Standard Time"},
            {81, "Caucasus Standard Time"},
            {82, "Afghanistan Standard Time"},
            {83, "West Asia Standard Time"},
            {84, "Russia TZ 4 Standard Time"},
            {85, "Pakistan Standard Time"},
            {86, "India Standard Time"},
            {87, "Sri Lanka Standard Time"},
            {88, "Nepal Standard Time"},
            {89, "Central Asia Standard Time"},
            {90, "Bangladesh Standard Time"},
            {91, "Omsk Standard Time"},
            {92, "Myanmar Standard Time"},
            {93, "SE Asia Standard Time"},
            {94, "Altai Standard Time"},
            {95, "W. Mongolia Standard Time"},
            {96, "Russia TZ 6 Standard Time"},
            {97, "Novosibirsk Standard Time"},
            {98, "Tomsk Standard Time"},
            {99, "China Standard Time"},
            {100, "Russia TZ 7 Standard Time"},
            {101, "Malay Peninsula Standard Time"},
            {102, "W. Australia Standard Time"},
            {103, "Taipei Standard Time"},
            {104, "Ulaanbaatar Standard Time"},
            {105, "North Korea Standard Time"},
            {106, "Aus Central W. Standard Time"},
            {107, "Transbaikal Standard Time"},
            {108, "Tokyo Standard Time"},
            {109, "Korea Standard Time"},
            {110, "Russia TZ 8 Standard Time"},
            {111, "Cen. Australia Standard Time"},
            {112, "AUS Central Standard Time"},
            {113, "E. Australia Standard Time"},
            {114, "AUS Eastern Standard Time"},
            {115, "West Pacific Standard Time"},
            {116, "Tasmania Standard Time"},
            {117, "Russia TZ 9 Standard Time"},
            {118, "Lord Howe Standard Time"},
            {119, "Bougainville Standard Time"},
            {120, "Russia TZ 10 Standard Time"},
            {121, "Magadan Standard Time"},
            {122, "Norfolk Standard Time"},
            {123, "Sakhalin Standard Time"},
            {124, "Central Pacific Standard Time"},
            {125, "Russia TZ 11 Standard Time"},
            {126, "New Zealand Standard Time"},
            {127, "UTC+12"},
            {128, "Fiji Standard Time"},
            {129, "Kamchatka Standard Time"},
            {130, "Chatham Islands Standard Time"},
            {131, "UTC+13"},
            {132, "Tonga Standard Time"},
            {133, "Samoa Standard Time"},
            {134, "Line Islands Standard Time"}
        };

    public enum DateTimeFormat
    {
      /// <summary>
      /// dd/MM/yyyy HH:mm:ss
      /// </summary>
      DayMonthYearByForwardSlash_HourMinuteSecondByColon,
      /// <summary>
      /// dd/MM/yyyy HH:mm
      /// </summary>
      DayMonthYearByForwardSlash_HourMinuteByColon,
      /// <summary>
      /// dd-MM-yyyy HH:mm:ss
      /// </summary>
      DayMonthYearByDash_HourMinuteSecondByColon,
      /// <summary>
      /// dd-MM-yyyy HH:mm
      /// </summary>
      DayMonthYearByDash_HourMinuteByColon,
      /// <summary>
      /// dd/MM/yyyy
      /// </summary>
      DayMonthYearByForwardSlash,
      /// <summary>
      /// dd-MM-yyyy
      /// </summary>
      DayMonthYearByDash,
      /// <summary>
      /// ddMMyyyy
      /// </summary>
      DayMonthYear,
      /// <summary>
      /// yyyy/MM/dd HH:mm:ss
      /// </summary>
      YearMonthDayByForwardSlash_HourMinuteSecondByColon,
      /// <summary>
      /// yyyy/MM/dd HH:mm
      /// </summary>
      YearMonthDayByForwardSlash_HourMinuteByColon,
      /// <summary>
      /// yyyy-MM-dd HH:mm:ss
      /// </summary>
      YearMonthDayByDash_HourMinuteSecondByColon,
      /// <summary>
      /// yyyy-MM-dd HH:mm
      /// </summary>
      YearMonthDayByDash_HourMinuteByColon,
      /// <summary>
      /// yyyy/MM/dd
      /// </summary>
      YearMonthDayByForwardSlash,
      /// <summary>
      /// yyyy-MM-dd
      /// </summary>
      YearMonthDayByDash,
      /// <summary>
      /// yyyyMMdd
      /// </summary>
      YearMonthDay,
      /// <summary>
      /// HH:mm:ss
      /// </summary>
      HHmmssByColon,
      /// <summary>
      /// HH:mm
      /// </summary>
      HHmmByColon,
      /// <summary>
      /// yyyyMMddHHmmss
      /// </summary>
      YearMonthDayHourMinuteSecond,
      /// <summary>
      /// yyyy-MM-ddTHH:mm:ssZ
      /// </summary>
      YearMonthDayByDashTHourMinuteSecondByColonZ,
      /// <summary>
      /// yyyy-MM-ddTHH:mm:ss
      /// </summary>
      YearMonthDayByDashTHourMinuteSecondByColon,
      /// <summary>
      /// yyyyMMddTHHmmssZ
      /// </summary>
      YearMonthDayTHourMinuteSecondZ,
      /// <summary>
      /// yyyyMMddTHHmmss
      /// </summary>
      YearMonthDayTHourMinuteSecond
    }

    public enum DateTimeUtcOffset
    {
      /// <summary>
      /// Not Show Offset
      /// </summary>
      Null,
      /// <summary>
      /// +/-HH:MM
      /// </summary>
      HHMMByColon,
      /// <summary>
      /// +/-HHMM
      /// </summary>
      HHMM,
      /// <summary>
      /// +/-HH
      /// </summary>
      HH
    }
    #endregion

    #region Public Enums and Static Dictionaries - CULTURE
    // Table of Language Culture Names, Codes, and ISO Values
    // http://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
    public enum LanguageCultureName
    {
      AFRIKAANS_SOUTH_AFRICA = 0,
      ALBANIAN_ALBANIA = 1,
      ARABIC_ALGERIA = 2,
      ARABIC_BAHRAIN = 3,
      ARABIC_EGYPT = 4,
      ARABIC_IRAQ = 5,
      ARABIC_JORDAN = 6,
      ARABIC_KUWAIT = 7,
      ARABIC_LEBANON = 8,
      ARABIC_LIBYA = 9,
      ARABIC_MOROCCO = 10,
      ARABIC_OMAN = 11,
      ARABIC_QATAR = 12,
      ARABIC_SAUDI_ARABIA = 13,
      ARABIC_SYRIA = 14,
      ARABIC_TUNISIA = 15,
      ARABIC_UNITED_ARAB_EMIRATES = 16,
      ARABIC_YEMEN = 17,
      ARMENIAN_ARMENIA = 18,
      AZERI_CYRILLIC_AZERBAIJAN = 19,
      AZERI_LATIN_AZERBAIJAN = 20,
      BASQUE_BASQUE = 21,
      BELARUSIAN_BELARUS = 22,
      BULGARIAN_BULGARIA = 23,
      CATALAN_CATALAN = 24,
      CHINESE_CHINA = 25,
      CHINESE_HONG_KONG_SAR = 26,
      CHINESE_MACAU_SAR = 27,
      CHINESE_SINGAPORE = 28,
      CHINESE_TAIWAN = 29,
      CHINESE_SIMPLIFIED = 30,
      CHINESE_TRADITIONAL = 31,
      CROATIAN_CROATIA = 32,
      CZECH_CZECH_REPUBLIC = 33,
      DANISH_DENMARK = 34,
      DHIVEHI_MALDIVES = 35,
      DUTCH_BELGIUM = 36,
      DUTCH_THE_NETHERLANDS = 37,
      ENGLISH_AUSTRALIA = 38,
      ENGLISH_BELIZE = 39,
      ENGLISH_CANADA = 40,
      ENGLISH_CARIBBEAN = 41,
      ENGLISH_IRELAND = 42,
      ENGLISH_JAMAICA = 43,
      ENGLISH_NEW_ZEALAND = 44,
      ENGLISH_PHILIPPINES = 45,
      ENGLISH_SOUTH_AFRICA = 46,
      ENGLISH_TRINIDAD_AND_TOBAGO = 47,
      ENGLISH_UNITED_KINGDOM = 48,
      ENGLISH_UNITED_STATES = 49,
      ENGLISH_ZIMBABWE = 50,
      ESTONIAN_ESTONIA = 51,
      FAROESE_FAROE_ISLANDS = 52,
      FARSI_IRAN = 53,
      FINNISH_FINLAND = 54,
      FRENCH_BELGIUM = 55,
      FRENCH_CANADA = 56,
      FRENCH_FRANCE = 57,
      FRENCH_LUXEMBOURG = 58,
      FRENCH_MONACO = 59,
      FRENCH_SWITZERLAND = 60,
      GALICIAN_GALICIAN = 61,
      GEORGIAN_GEORGIA = 62,
      GERMAN_AUSTRIA = 63,
      GERMAN_GERMANY = 64,
      GERMAN_LIECHTENSTEIN = 65,
      GERMAN_LUXEMBOURG = 66,
      GERMAN_SWITZERLAND = 67,
      GREEK_GREECE = 68,
      GUJARATI_INDIA = 69,
      HEBREW_ISRAEL = 70,
      HINDI_INDIA = 71,
      HUNGARIAN_HUNGARY = 72,
      ICELANDIC_ICELAND = 73,
      INDONESIAN_INDONESIA = 74,
      ITALIAN_ITALY = 75,
      ITALIAN_SWITZERLAND = 76,
      JAPANESE_JAPAN = 77,
      KANNADA_INDIA = 78,
      KAZAKH_KAZAKHSTAN = 79,
      KONKANI_INDIA = 80,
      KOREAN_KOREA = 81,
      KYRGYZ_KAZAKHSTAN = 82,
      LATVIAN_LATVIA = 83,
      LITHUANIAN_LITHUANIA = 84,
      MACEDONIAN_FYROM = 85,
      MALAY_BRUNEI = 86,
      MALAY_MALAYSIA = 87,
      MARATHI_INDIA = 88,
      MONGOLIAN_MONGOLIA = 89,
      NORWEGIAN_BOKMÅL_NORWAY = 90,
      NORWEGIAN_NYNORSK_NORWAY = 91,
      POLISH_POLAND = 92,
      PORTUGUESE_BRAZIL = 93,
      PORTUGUESE_PORTUGAL = 94,
      PUNJABI_INDIA = 95,
      ROMANIAN_ROMANIA = 96,
      RUSSIAN_RUSSIA = 97,
      SANSKRIT_INDIA = 98,
      SERBIAN_CYRILLIC_SERBIA = 99,
      SERBIAN_LATIN_SERBIA = 100,
      SLOVAK_SLOVAKIA = 101,
      SLOVENIAN_SLOVENIA = 102,
      SPANISH_ARGENTINA = 103,
      SPANISH_BOLIVIA = 104,
      SPANISH_CHILE = 105,
      SPANISH_COLOMBIA = 106,
      SPANISH_COSTA_RICA = 107,
      SPANISH_DOMINICAN_REPUBLIC = 108,
      SPANISH_ECUADOR = 109,
      SPANISH_EL_SALVADOR = 110,
      SPANISH_GUATEMALA = 111,
      SPANISH_HONDURAS = 112,
      SPANISH_MEXICO = 113,
      SPANISH_NICARAGUA = 114,
      SPANISH_PANAMA = 115,
      SPANISH_PARAGUAY = 116,
      SPANISH_PERU = 117,
      SPANISH_PUERTO_RICO = 118,
      SPANISH_SPAIN = 119,
      SPANISH_URUGUAY = 120,
      SPANISH_VENEZUELA = 121,
      SWAHILI_KENYA = 122,
      SWEDISH_FINLAND = 123,
      SWEDISH_SWEDEN = 124,
      SYRIAC_SYRIA = 125,
      TAMIL_INDIA = 126,
      TATAR_RUSSIA = 127,
      TELUGU_INDIA = 128,
      THAI_THAILAND = 129,
      TURKISH_TURKEY = 130,
      UKRAINIAN_UKRAINE = 131,
      URDU_PAKISTAN = 132,
      UZBEK_CYRILLIC_UZBEKISTAN = 133,
      UZBEK_LATIN_UZBEKISTAN = 134,
      VIETNAMESE_VIETNAM = 135
    }

    // Table of Language Culture Names, Codes, and ISO Values
    // http://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
    public static Dictionary<int, string> LanguageCultureNames = new Dictionary<int, string>
        {
            {0, "af-ZA"},
            {1, "sq-AL"},
            {2, "ar-DZ"},
            {3, "ar-BH"},
            {4, "ar-EG"},
            {5, "ar-IQ"},
            {6, "ar-JO"},
            {7, "ar-KW"},
            {8, "ar-LB"},
            {9, "ar-LY"},
            {10, "ar-MA"},
            {11, "ar-OM"},
            {12, "ar-QA"},
            {13, "ar-SA"},
            {14, "ar-SY"},
            {15, "ar-TN"},
            {16, "ar-AE"},
            {17, "ar-YE"},
            {18, "hy-AM"},
            {19, "Cy-az-AZ"},
            {20, "Lt-az-AZ"},
            {21, "eu-ES"},
            {22, "be-BY"},
            {23, "bg-BG"},
            {24, "ca-ES"},
            {25, "zh-CN"},
            {26, "zh-HK"},
            {27, "zh-MO"},
            {28, "zh-SG"},
            {29, "zh-TW"},
            {30, "zh-CHS"},
            {31, "zh-CHT"},
            {32, "hr-HR"},
            {33, "cs-CZ"},
            {34, "da-DK"},
            {35, "div-MV"},
            {36, "nl-BE"},
            {37, "nl-NL"},
            {38, "en-AU"},
            {39, "en-BZ"},
            {40, "en-CA"},
            {41, "en-CB"},
            {42, "en-IE"},
            {43, "en-JM"},
            {44, "en-NZ"},
            {45, "en-PH"},
            {46, "en-ZA"},
            {47, "en-TT"},
            {48, "en-GB"},
            {49, "en-US"},
            {50, "en-ZW"},
            {51, "et-EE"},
            {52, "fo-FO"},
            {53, "fa-IR"},
            {54, "fi-FI"},
            {55, "fr-BE"},
            {56, "fr-CA"},
            {57, "fr-FR"},
            {58, "fr-LU"},
            {59, "fr-MC"},
            {60, "fr-CH"},
            {61, "gl-ES"},
            {62, "ka-GE"},
            {63, "de-AT"},
            {64, "de-DE"},
            {65, "de-LI"},
            {66, "de-LU"},
            {67, "de-CH"},
            {68, "el-GR"},
            {69, "gu-IN"},
            {70, "he-IL"},
            {71, "hi-IN"},
            {72, "hu-HU"},
            {73, "is-IS"},
            {74, "id-ID"},
            {75, "it-IT"},
            {76, "it-CH"},
            {77, "ja-JP"},
            {78, "kn-IN"},
            {79, "kk-KZ"},
            {80, "kok-IN"},
            {81, "ko-KR"},
            {82, "ky-KZ"},
            {83, "lv-LV"},
            {84, "lt-LT"},
            {85, "mk-MK"},
            {86, "ms-BN"},
            {87, "ms-MY"},
            {88, "mr-IN"},
            {89, "mn-MN"},
            {90, "nb-NO"},
            {91, "nn-NO"},
            {92, "pl-PL"},
            {93, "pt-BR"},
            {94, "pt-PT"},
            {95, "pa-IN"},
            {96, "ro-RO"},
            {97, "ru-RU"},
            {98, "sa-IN"},
            {99, "Cy-sr-SP"},
            {100, "Lt-sr-SP"},
            {101, "sk-SK"},
            {102, "sl-SI"},
            {103, "es-AR"},
            {104, "es-BO"},
            {105, "es-CL"},
            {106, "es-CO"},
            {107, "es-CR"},
            {108, "es-DO"},
            {109, "es-EC"},
            {110, "es-SV"},
            {111, "es-GT"},
            {112, "es-HN"},
            {113, "es-MX"},
            {114, "es-NI"},
            {115, "es-PA"},
            {116, "es-PY"},
            {117, "es-PE"},
            {118, "es-PR"},
            {119, "es-ES"},
            {120, "es-UY"},
            {121, "es-VE"},
            {122, "sw-KE"},
            {123, "sv-FI"},
            {124, "sv-SE"},
            {125, "syr-SY"},
            {126, "ta-IN"},
            {127, "tt-RU"},
            {128, "te-IN"},
            {129, "th-TH"},
            {130, "tr-TR"},
            {131, "uk-UA"},
            {132, "ur-PK"},
            {133, "Cy-uz-UZ"},
            {134, "Lt-uz-UZ"},
            {135, "vi-VN"}
        };
    #endregion



    #region Public Static Methods
    /// <summary>
    /// ใช้สำหรับแสดงวันเวลาปัจจุบัน (UTC) ให้อยู่ในรูปแบบตามที่ระบุ โดยใช้ Time Zone ไทย (GMT +07:00 - SE Asia Standard Time)
    /// </summary>
    /// <param name="format"></param>
    /// <param name="culture"></param>
    /// <param name="isShowOffset"></param>
    /// <returns></returns>
    public static string GetCurrentUtcDateTimeInThaiTimeZone(DateTimeFormat format, LanguageCultureName culture, DateTimeUtcOffset utcOffset)
    {
      var formatValue = GetDateTimeFormatValue(format);
      var cultureValue = GetCultureValue(culture);
      var currentUtc = DateTime.UtcNow;
      var timezone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); //https://msdn.microsoft.com/en-us/library/ms912391(v=winembedded.11).aspx
      var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(currentUtc, timezone);
      var offset = timezone.GetUtcOffset(currentUtc);

      if (utcOffset != DateTimeUtcOffset.Null)
      {
        return convertedTime.ToString(formatValue, CultureInfo.CreateSpecificCulture(cultureValue)) + GetDateTimeUtcOffsetValue(offset, utcOffset);
      }
      else
      {
        return convertedTime.ToString(formatValue, CultureInfo.CreateSpecificCulture(cultureValue));
      }
    }

    /// <summary>
    /// ใช้สำหรับแสดงวันเวลาปัจจุบัน (UTC) ให้อยู่ในรูปแบบตามที่ระบุ โดยใช้ Time Zone ตามที่ระบุ
    /// </summary>
    /// <param name="timeZoneId"></param>
    /// <param name="format"></param>
    /// <param name="culture"></param>
    /// <param name="utcOffset"></param>
    /// <returns></returns>
    public static string GetCurrentUtcDateTimeByTimeZoneId(TimeZoneDisplayName timezoneId, DateTimeFormat format, LanguageCultureName culture, DateTimeUtcOffset utcOffset)
    {
      var timezoneValue = GetTimeZoneValue(timezoneId);
      var formatValue = GetDateTimeFormatValue(format);
      var cultureValue = GetCultureValue(culture);
      var currentUtc = DateTime.UtcNow;
      var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneValue); //https://msdn.microsoft.com/en-us/library/ms912391(v=winembedded.11).aspx
      var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(currentUtc, timezone);
      var offset = timezone.GetUtcOffset(currentUtc);

      if (utcOffset != DateTimeUtcOffset.Null)
      {
        return convertedTime.ToString(formatValue, CultureInfo.CreateSpecificCulture(cultureValue)) + GetDateTimeUtcOffsetValue(offset, utcOffset);
      }
      else
      {
        return convertedTime.ToString(formatValue, CultureInfo.CreateSpecificCulture(cultureValue));
      }
    }

    /// <summary>
    /// ใช้สำหรับแปลงวันเวลาที่ระบุ ให้เป็นวันเวลา (UTC) อยู่ในรูปแบบตามที่ระบุ โดยใช้ Time Zone ไทย (GMT +07:00 - SE Asia Standard Time)
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="format"></param>
    /// <param name="culture"></param>
    /// <param name="isShowOffset"></param>
    /// <returns></returns>
    public static string ConvertToUtcDateTimeInThaiTimeZone(DateTime datetime, DateTimeFormat format, LanguageCultureName culture, DateTimeUtcOffset utcOffset)
    {
      var formatValue = GetDateTimeFormatValue(format);
      var cultureValue = GetCultureValue(culture);
      var datetimeUtc = datetime.ToUniversalTime();
      var timezone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); //https://msdn.microsoft.com/en-us/library/ms912391(v=winembedded.11).aspx
      var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(datetimeUtc, timezone);
      var offset = timezone.GetUtcOffset(datetimeUtc);

      if (utcOffset != DateTimeUtcOffset.Null)
      {
        return convertedTime.ToString(formatValue, CultureInfo.CreateSpecificCulture(cultureValue)) + GetDateTimeUtcOffsetValue(offset, utcOffset);
      }
      else
      {
        return convertedTime.ToString(formatValue, CultureInfo.CreateSpecificCulture(cultureValue));
      }
    }

    /// <summary>
    /// ใช้สำหรับแปลงวันเวลาที่ระบุ ให้เป็นวันเวลา (UTC) อยู่ในรูปแบบตามที่ระบุ โดยใช้ Time Zone ตามที่ระบุ
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="timeZoneId"></param>
    /// <param name="format"></param>
    /// <param name="culture"></param>
    /// <param name="utcOffset"></param>
    /// <returns></returns>
    public static string ConvertToUtcDateTimeByTimeZoneId(DateTime datetime, TimeZoneDisplayName timezoneId, DateTimeFormat format, LanguageCultureName culture, DateTimeUtcOffset utcOffset)
    {
      var timezoneValue = GetTimeZoneValue(timezoneId);
      var formatValue = GetDateTimeFormatValue(format);
      var cultureValue = GetCultureValue(culture);
      var datetimeUtc = datetime.ToUniversalTime();
      var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneValue); //https://msdn.microsoft.com/en-us/library/ms912391(v=winembedded.11).aspx
      var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(datetimeUtc, timezone);
      var offset = timezone.GetUtcOffset(datetimeUtc);

      if (utcOffset != DateTimeUtcOffset.Null)
      {
        return convertedTime.ToString(formatValue, CultureInfo.CreateSpecificCulture(cultureValue)) + GetDateTimeUtcOffsetValue(offset, utcOffset);
      }
      else
      {
        return convertedTime.ToString(formatValue, CultureInfo.CreateSpecificCulture(cultureValue));
      }
    }

    /// <summary>
    /// ใช้สำหรับแปลงวันเวลาที่ระบุ ให้เป็นวันเวลาในรูปแบบ Unix Time
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public static long ConvertToUnixTimeByDateTime(DateTime datetime)
    {
      var dateTimeOffset = new DateTimeOffset(datetime);

      return dateTimeOffset.ToUnixTimeSeconds();
    }

    /// <summary>
    /// ใช้สำหรับแปลงวันเวลาในรูปแบบ Unix Time ที่ระบุ ให้เป็นวันเวลา
    /// </summary>
    /// <param name="unixtime"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTimeByUnixTime(long unixtime)
    {
      return DateTimeOffset.FromUnixTimeSeconds(unixtime).DateTime.ToLocalTime();
    }
    #endregion



    #region Private Classes Models
    private class TimeZoneModel
    {
      public string Id { get; set; }
      public string StandardName { get; set; }
      public string DisplayName { get; set; }
      public string Offset { get; set; }
    }
    #endregion

    #region Private Static Methods
    private static string GetDateTimeFormatValue(DateTimeFormat format)
    {
      switch (format)
      {
        case DateTimeFormat.DayMonthYearByForwardSlash_HourMinuteSecondByColon:
          return "dd/MM/yyyy HH:mm:ss";
        case DateTimeFormat.DayMonthYearByForwardSlash_HourMinuteByColon:
          return "dd/MM/yyyy HH:mm";
        case DateTimeFormat.DayMonthYearByDash_HourMinuteSecondByColon:
          return "dd-MM-yyyy HH:mm:ss";
        case DateTimeFormat.DayMonthYearByDash_HourMinuteByColon:
          return "dd-MM-yyyy HH:mm";
        case DateTimeFormat.DayMonthYearByForwardSlash:
          return "dd/MM/yyyy";
        case DateTimeFormat.DayMonthYearByDash:
          return "dd-MM-yyyy";
        case DateTimeFormat.DayMonthYear:
          return "ddMMyyyy";
        case DateTimeFormat.YearMonthDayByForwardSlash_HourMinuteSecondByColon:
          return "yyyy/MM/dd HH:mm:ss";
        case DateTimeFormat.YearMonthDayByForwardSlash_HourMinuteByColon:
          return "yyyy/MM/dd HH:mm";
        case DateTimeFormat.YearMonthDayByDash_HourMinuteSecondByColon:
          return "yyyy-MM-dd HH:mm:ss";
        case DateTimeFormat.YearMonthDayByDash_HourMinuteByColon:
          return "yyyy-MM-dd HH:mm";
        case DateTimeFormat.YearMonthDayByForwardSlash:
          return "yyyy/MM/dd";
        case DateTimeFormat.YearMonthDayByDash:
          return "yyyy-MM-dd";
        case DateTimeFormat.YearMonthDay:
          return "yyyyMMdd";
        case DateTimeFormat.HHmmssByColon:
          return "HH:mm:ss";
        case DateTimeFormat.HHmmByColon:
          return "HH:mm";
        case DateTimeFormat.YearMonthDayHourMinuteSecond:
          return "yyyyMMddHHmmss";
        case DateTimeFormat.YearMonthDayByDashTHourMinuteSecondByColonZ:
          return "yyyy-MM-ddTHH:mm:ssZ";
        case DateTimeFormat.YearMonthDayByDashTHourMinuteSecondByColon:
          return "yyyy-MM-ddTHH:mm:ss";
        case DateTimeFormat.YearMonthDayTHourMinuteSecondZ:
          return "yyyyMMddTHHmmssZ";
        case DateTimeFormat.YearMonthDayTHourMinuteSecond:
          return "yyyyMMddTHHmmss";
        default:
          return "dd/MM/yyyy HH:mm:ss";
      }
    }

    private static string GetDateTimeUtcOffsetValue(TimeSpan offset, DateTimeUtcOffset utcOffset)
    {
      var checkPatterm = Regex.IsMatch(offset.ToString(), @"^\d{2}:\d{2}:\d{2}$", RegexOptions.IgnoreCase);
      var vSign = string.Empty;
      var vHH = string.Empty;
      var vMM = string.Empty;
      var vSS = string.Empty;
      var offsetText = offset.ToString();

      if (checkPatterm)
      {
        vSign = "+";
        offsetText = offsetText.Substring(0, offsetText.Length);
      }
      else
      {
        vSign = "-";
        offsetText = offsetText.Substring(1, offsetText.Length - 1);
      }

      string[] splitOffset = offsetText.Split(':');
      vHH = splitOffset[0];
      vMM = splitOffset[1];
      vSS = splitOffset[2];

      if (utcOffset == DateTimeUtcOffset.HHMMByColon)
      {
        return vSign + vHH + ":" + vMM;
      }
      else if (utcOffset == DateTimeUtcOffset.HHMM)
      {
        return vSign + vHH + vMM;
      }
      else if (utcOffset == DateTimeUtcOffset.HH)
      {
        return vSign + vHH;
      }
      else
      {
        return "";
      }
    }

    private static string GetCultureValue(LanguageCultureName culture)
    {
      return LanguageCultureNames[Convert.ToInt32(culture.ToString("D"))];
    }

    private static string GetTimeZoneValue(TimeZoneDisplayName timezone)
    {
      return TimeZoneDisplayNames[Convert.ToInt32(timezone.ToString("D"))];
    }
    #endregion



    #region Use For Development Time Only
    /// <summary>
    /// ใช้สำหรับดึงข้อมูลออกมา เพื่อเอาไปทำ Enum TimeZoneDisplayName
    /// </summary>
    /// <returns></returns>
    private static string DEVELOPMENTUSEGetTimeZoneDisplayName()
    {
      var result = string.Empty;
      var i = 0;

      foreach (var item in TimeZoneInfo.GetSystemTimeZones())
      {
        var pattern1 = @"(\(UTC((\+|-)\d{2}\:\d{2}\)|\)) |-| - | |'|\(|\))";
        var replacement1 = "";
        var rgx1 = new Regex(pattern1);
        var afterReplace1 = rgx1.Replace(item.DisplayName, replacement1);

        var pattern2 = @"(Time\+|Time-)";
        var replacement2 = "Time";
        var rgx2 = new Regex(pattern2);
        var afterReplace2 = rgx2.Replace(afterReplace1, replacement2);

        var pattern3 = @"(,|&)";
        var replacement3 = "_";
        var rgx3 = new Regex(pattern3);
        var afterReplace3 = rgx3.Replace(afterReplace2, replacement3);

        var pattern4 = @"Is\.";
        var replacement4 = "Islands";
        var rgx4 = new Regex(pattern4);
        var afterReplace4 = rgx4.Replace(afterReplace3, replacement4);

        var pattern5 = @"St\.";
        var replacement5 = "St";
        var rgx5 = new Regex(pattern5);
        var afterReplace5 = rgx5.Replace(afterReplace4, replacement5);

        result += afterReplace5 + " = " + i + "," + "\r\n";
        i++;
      }

      return result;
    }

    /// <summary>
    /// ใช้สำหรับดึงข้อมูลออกมา เพื่อเอาไปทำ Dictionary TimeZoneDisplayNames
    /// </summary>
    /// <returns></returns>
    private static string DEVELOPMENTUSEGetTimeZoneId()
    {
      var result = string.Empty;
      var i = 0;

      foreach (var item in TimeZoneInfo.GetSystemTimeZones())
      {
        result += "{" + i + ", \"" + item.StandardName.Trim() + "\"}," + "\r\n";
        i++;
      }

      return result;
    }
    #endregion
  }
}