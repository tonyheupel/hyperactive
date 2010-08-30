using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Dynamic;
using TonyHeupel.HyperJS;
using TonyHeupel.HyperCore;

namespace TonyHeupel.HyperActive.JSExtensions
{
    public static class JSExtensionsForImage
    {
        public static dynamic Image(this JS js)
        {
            return Image(js, 0, 0);
        }

        public static dynamic Image(this JS js, int width, int height)
        {
            //dynamic img = new HyperHypo(Object());
            dynamic img = new HyperHypo();
            img.Prototype = JS.cs.Object(img); // Pass the img object in so it knows what "this" (self) is...maybe take this out?
                                               // Should actually create a DOM Element base class with name and id and use that...
            img.width = width;
            img.height = height;

            img.id = "";
            img.name = "";
            img.src = "";
            img.alt = "";           // Alternate text when image can't be displayed
            img.isMap = false;      // Whether to use a server-side image map
            img.longDesc = "";      // Uri of a long image description
            img.useMap = "";        // Specifies a client-side image map for the image


            #region Just playing around
            img.doStuff = new Func<string, string>(someArg => string.Format("[someArg: {0}, width: {1}, height: {2}]", someArg, img.width, img.height));

            //Private variables with Closures and no collision even though in static method?  Awesome...
            var _iamprivate = "private var";

            img.getPrivate = new Func<string>(delegate() { return _iamprivate; });
            img.setPrivate = new Func<string, object>(delegate(string newPrivate) { _iamprivate = newPrivate; return null; });
            #endregion

            return img;
        }
        
    }
}
