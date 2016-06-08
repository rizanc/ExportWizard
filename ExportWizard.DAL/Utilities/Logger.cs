using System;
using System.Collections.Generic;
using System.Linq;
using Gurock.SmartInspect;

namespace ExportWizard.DAL.Utilities
{
    public static class Logger
    {

        // Static initializer
        static Logger()
        {
            //SiAuto.Si.Connections = "tcp()";

            SiAuto.Si.Connections = "file(filename=logging.sil,rotate=daily),pipe(reconnect=true, reconnect.interval=10s, pipename=\"smartinspect\")";
            SiAuto.Si.Enabled = true;
        }

        public static void Error(string message)
        {
            try
            {
                SiAuto.Main.LogError(message);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogException(ex);
            }
        }

        public static void Error(Exception ex)
        {
            try
            {
                SiAuto.Main.LogException(ex);
            }
            catch (Exception ex1)
            {
                SiAuto.Main.LogException(ex1);
            }
        }

        public static void Warning(string message)
        {
            SiAuto.Main.LogWarning(message);
        }

        public static void Debug(Exception ex)
        {
            SiAuto.Main.LogException(ex);
        }

        public static void Debug(String message)
        {
            SiAuto.Main.LogDebug(message);
        }

        public static void Info(string message)
        {
            SiAuto.Main.LogMessage(message);
        }

        public static void EnterMethod(String method)
        {
            SiAuto.Main.EnterMethod(method);
        }

        public static void ExitMethod(String method)
        {
            SiAuto.Main.LeaveMethod(method);
        }

    }
}

