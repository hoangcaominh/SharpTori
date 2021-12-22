using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTori
{
    public class TH09 : THBase
    {
        public TH09(IntPtr handle) : base(handle)
        {

        }

        public override void Reset()
        {

        }

        public override bool IsInGame()
        {
            return false;
        }
    }
}
