namespace Library.BusinessLogic.Exceptions
{
    /// <summary>
    /// נזרקת כאשר בקשה תקינה מבחינת מבנה, אך מפרה חוק עסקי
    /// (לדוגמה: ISBN כפול, אין עותקים זמינים להשאלה). ה-Middleware ימיר אותה ל-400.
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
}
