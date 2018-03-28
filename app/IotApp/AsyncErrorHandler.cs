using System;
using System.Diagnostics;
using IotApp.Helpers;

namespace IotApp
{
    public class AsyncErrorHandler
    {
        public static void HandleException(Exception ex)
        {
            AppCenterHelper.Error("AsyncErrorHandler HandleException", ex);
#if DEBUG
            Debug.WriteLine(ex);
#endif
        }
    }
}
