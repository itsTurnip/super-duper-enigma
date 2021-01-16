using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    class VBScript
    {
        public static void CallVBScript()
        {
            var sc = new MSScriptControl.ScriptControl();
            sc.Language = "VBScript";
            sc.AllowUI = true;

            try
            {
                sc.AddCode("Function Alert(msg_text)\n" +
                    "message = MsgBox(msg_text, 16, \"Ошибка!\")\n" +
                    "End Function");
                var parameters = new object[] { "Программа уже запущена" };
                sc.Run("Alert", ref parameters);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.ToString());
            }
        }
    }
}
