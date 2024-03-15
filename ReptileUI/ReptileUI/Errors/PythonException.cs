using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReptileUI.Errors
{
    class PythonException : Exception
    {
        public PythonException(): base("python调用过程中产生错误!")
        {

        }

        public PythonException(string message) : base("pythonc调用过程中产生错误!\n" + message)
        {

        }
    }
}
