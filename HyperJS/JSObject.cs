using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TonyHeupel.HyperCore;

namespace TonyHeupel.HyperJS
{
    public class JSObject : HyperHypo
    {
        public static implicit operator bool(JSObject o)
        {
            return ConvertToHyperJSBoolean(o);
        }

        protected static bool ConvertToHyperJSBoolean(object o)
        {
            return JS.Boolean(o);
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            //Try using undefined for properties that aren't defined yet and always returning true...
            if (!base.TryGetMember(binder, out result))
            {
                result = JS.undefined;
            }

            return true;
        }
    }
}
