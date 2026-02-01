namespace esyasoft.mobility.CHRGUP.service.core.Helpers
{
    public static class DbTime
    {
        public static DateTime From(DateTime dt) =>
            DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);

        public static DateTime Now =>
            DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
    }
}
