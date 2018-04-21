using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Interfaces, like Abstract Classes, do NOT contain any kind of implantation. However, unlike Abtract Classes, interfaces
 * support multiple inheritance in the .NET Framework. Everything is public by default in an interface.
 */

namespace Casino.Interfaces
{
    interface IWalkAway
    {
        void WalkAway(Player player);
    }
}
