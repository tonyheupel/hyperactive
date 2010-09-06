using TonyHeupel.HyperJS;

namespace TonyHeupel.HyperActive.JSExtensions
{
    public static class Image
    {
        public static dynamic NewImage(this JS js)
        {
            return NewImage(js, 0, 0);
        }

        public static dynamic NewImage(this JS js, int width, int height)
        {
            dynamic img = new JSObject();
            img.JSTypeName = "Image";

            // Set up the prototype
            dynamic p = new JSObject();
            img.Prototype = img.GetPrototype(p);  // Should actually create a DOM Element base class with name and id and use that...
            
            // Set up instance items
            img.width = width;
            img.height = height;

            img.id = "";
            img.name = "";
            img.src = "";
            img.alt = "";           // Alternate text when image can't be displayed
            img.isMap = false;      // Whether to use a server-side image map
            img.longDesc = "";      // Uri of a long image description
            img.useMap = "";        // Specifies a client-side image map for the image


            return img;
        }
        
    }
}
