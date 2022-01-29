using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating_System
{
    public enum GameTypes
    {
        [Description("8 Ball")]
        EightBall = 0,

        [Description("9 Ball")]
        NineBall = 1,

        [Description("10 Ball")]
        TenBall = 2
    }
}
