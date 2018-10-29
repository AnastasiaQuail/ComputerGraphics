using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;

namespace GameFramework
{
    class LuaCompiller
    {
        Lua state;
        public LuaCompiller()
        {
            state = new Lua();
        }
        public string DoCode(string codeText)
        {
            var res = state.DoString(codeText)[0];
            return res.ToString();
        }
    }
}
