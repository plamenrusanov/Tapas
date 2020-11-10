namespace Tapas.Common
{
    using System;

    public static class GlobalConstants
    {
        public const string SystemName = "Tapas";

        public const string AdministratorName = "Administrator";

        public const string OperatorName = "Operator";

        public const string UserNameAsString = "User";

        public const string EmailAdministrator = "plamen.rusanov@abv.bg";

        public const string EmailOperator = "operator@tapas.bg";

        public const string AreaAdmin = "Administration";

        public const string DefaultProductImage = "utvaxhqgzon5lczgkoak.jpg";

        public const string LoginPageRoute = "/Identity/Account/Login";

        public const string IndexRoute = "/";

        public const string RefererHeader = "Referer";

        public const int DescriptionMaxLength = 150;

        public const decimal OrderPriceMin = 10m;

        // Max order price to charge delivery fee
        public const decimal MOPTCDF = 15m;

        public const decimal OrderPriceForBonus = 25m;

        public const string TapasEmail = "plamenrusanov76@gmail.com";

        public const string TapasEmailSender = "Тапас";

        public const string DefaultLogPattern = "User: {0}\n\tMessage: {1}\n\tStackTrace: {2}";

        public const string TakeAway = "TakeAway";

        public static TimeSpan OpenTime => new TimeSpan(10, 0, 0);

        public static TimeSpan CloseTime => new TimeSpan(22, 45, 0);

        public static TimeSpan LastOrderTime => new TimeSpan(22, 30, 0);

        public static TimeSpan TimeNow => DateTime.Now.TimeOfDay;
    }
}
