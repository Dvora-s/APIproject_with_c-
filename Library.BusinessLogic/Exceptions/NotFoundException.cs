namespace Library.BusinessLogic.Exceptions
{
    /// <summary>
    /// נזרקת כאשר ישות מבוקשת לא נמצאה במסד הנתונים. ה-Middleware הגלובלי ימיר אותה ל-404.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string entityName, object key)
            : base($"{entityName} עם מזהה '{key}' לא נמצא.")
        {
        }
    }
}
