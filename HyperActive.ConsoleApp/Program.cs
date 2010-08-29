using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Dynamic;
using TonyHeupel.HyperCore;
using TonyHeupel.HyperJS;

namespace HyperActive.ConsoleApp
{
    class Program
    {
        /// <summary>
        /// Simple function that is bound dynamically in Main
        /// </summary>
        /// <returns></returns>
        public static string SayHello() { return "Hello"; }

        /// <summary>
        /// Add a blank line and wait for a keypress in a way that allows the next 
        /// Console.WriteLine to clear the Press any key... prompt.
        /// </summary>
        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press any key...");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            dynamic third = InhertiableHyperDictionary();

            ExpandoObjectLimited();

            HyperDynamoGiveJSLikeSettersAndGettersAndForEach();

            CombiningHyperDictionaryWithHyperDynamo(third);

            HelloHyperHypo();

            HelloHyperJS();
        }
       
        private static dynamic InhertiableHyperDictionary()
        {
            Console.WriteLine("Using HyperDictionary to show dictionary inheritance\n==================================================");
            var top = new HyperDictionary("top");
            top["eyes"] = "brown";
            top["hair"] = "pointy";
            top["body_head"] = "big";
            top["body_stomach"] = "fat";

            var second = new HyperDictionary("second");
            second["body_stomach"] = "skinny";
            second["body_feet"] = new string[] { "very", "stinky" };
            top.AddChild(second); //Adding Child since we may want to stry to store the WHOLE object graph in document DB

            var third = new HyperDictionary("third");
            third.RemoveProperty("hair");
            third.ExtendProperty("body_feet", new string[] { "NOT!" });
            third.RemoveProperty("body_stomach");
            third.InheritsFrom(second);  //Using InheritsFrom since we don't want this part of the object graph that could be stored in RavenDB later

            Console.WriteLine("top\tbody_head:\t{0}", top["body_head"]);
            Console.WriteLine("top\tbody_stomach:\t{0}", top["body_stomach"]);

            Console.WriteLine("second\tbody_head:\t{0}", second["body_head"]);
            Console.WriteLine("second\tbody_stomach:\t{0}", second["body_stomach"]);
            Console.Write("second\tbody_feet values:\t");
            var feet = second["body_feet"] as IEnumerable<object>;
            foreach (string foot in feet)
            {
                Console.Write(foot + " ");
            }
            Console.WriteLine();
            Console.WriteLine("third body_head: {0}", third["body_head"]);

            Console.WriteLine("Making sure TryGetProperty works...should have 'not set' as the next value...");
            object stomach;
            if (third.TryGetProperty("body_stomach", out stomach)) Console.WriteLine("third\tbody_stomach:\t{0}", stomach);
            else Console.WriteLine("not set - third.body_stomach");

            Console.WriteLine("Making sure Extending of IEnumerables in a subclass works...should have 'NOT!' at the end...");
            Console.Write("third\tbody_feet values:\t");
            feet = third["body_feet"] as IEnumerable<object>;
            foreach (string foot in feet)
            {
                Console.Write(foot + " ");
            }
            Console.WriteLine();
            Pause();

            return third;
        }

        /// <summary>
        /// Demonstate that ExpandoObject is cool, but not cool enough.
        /// </summary>
        private static void ExpandoObjectLimited()
        {
            Console.WriteLine("Using Expando Object\n====================");
            dynamic stuff = new ExpandoObject();
            stuff.Something = "something";
            stuff.Fun = "fun";
            try
            {
                stuff["For"] = "everyone"; //Generates a runtime error (not as cool as JavaScript)
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXPECTED EXCEPTION - NO INDEXER/NAME ALLOWED ON EXPANDOOBJECT (stuff[\"For\"] = \"everyone\"");
                Console.WriteLine(ex.ToString());
            }

            foreach (object o in stuff)
            {
                Console.WriteLine(o);
            }

            Pause();
        }

        /// <summary>
        /// Show how even with a basic Dictionary member provider, 
        /// HyperDynamo is cooler than ExpandoObject
        /// </summary>
        private static void HyperDynamoGiveJSLikeSettersAndGettersAndForEach()
        {
            Console.WriteLine("Using HyperDynamo with Plain Dictionary MemberProvider\n==================================================");
            // Creating a dynamic dictionary.
            dynamic person = new HyperDynamo();

            // Adding new dynamic properties. 
            // The TrySetMember method is called.
            person.FirstName = "Tony";
            person.LastName = "Heupel";
            person["MiddleInitial"] = "C.";

            // Getting values of the dynamic properties.
            // The TryGetMember method is called.
            // Note that property names are case-insensitive.
            Console.WriteLine(person.FirstName + " " + person.MiddleInitial + " " + person["LastName"]);

            //Define the enumerator on the inner HyperDictionary and then expose it for GetEnumerator
            foreach (object o in person)
            {
                Console.WriteLine(o);
            }
            Pause();
        }

        /// <summary>
        /// For real fun, combine HyperDynamo with a 
        /// HyperDictionary member provider for some
        /// JavaScript-like Prototype inhertance!
        /// </summary>
        public static void CombiningHyperDictionaryWithHyperDynamo(dynamic third)
        {
            Console.WriteLine("Using HyperDynamo with HyperDictionary MemberProvider to show\ndynamic mappings and inheritance!\n==================================================");

            dynamic thirdDyn = new HyperDynamo(third);  //Manually use HyperDictionary with HyperDnamo
            thirdDyn.toes = "third toes set through dynamic property";
            thirdDyn.body_head = "third body_head set through dynamic property";
            Console.WriteLine("eyes:\t{0}", thirdDyn["eyes"]);
            Console.WriteLine("eyes:\t{0}", thirdDyn.eyes);
            Console.WriteLine("body_head:\t{0}", thirdDyn["eyes"]);
            Console.WriteLine("body_head:\t{0}", thirdDyn.body_head);

            Console.WriteLine();
            try
            {
                //Should throw an exception since it got removed at this level
                Console.WriteLine("hair:\t{0}", thirdDyn.hair);
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException rbe)
            {
                Console.WriteLine("EXPECTED EXCEPTION SINCE PROPERTY WAS REMOVED:\n{0}", rbe);
            }
            Pause();

            Console.WriteLine("Properties in the HyperDynamo object built off of 3 levels of HyperDictionary\ninheritance and a couple dynamic property setters\n========================================================================");
            foreach (object o in thirdDyn)
            {
                Console.WriteLine(o);
            }
            Pause();
        }

        /// <summary>
        /// Formalize a HyperDynamo using a HyperDictionary into a concrete class
        /// called HyperHypo (Hyper - more than C#; Hypo - less than JavaScript)
        /// </summary>
        private static void HelloHyperHypo()
        {
            Console.WriteLine("Using HyperHypo (formalized HyperDynamo with HyperDictionary) to enable JS.cs\n(JavaScript-style programming within C#) called HyperJS\n==================================================");

            Console.WriteLine("First example: prototype inheritance where only one.Whassup is set");
            dynamic one = new HyperHypo();
            one.Whassup = new Func<string>(SayHello);
            Console.WriteLine("one.Whassup: {0}", one.Whassup());

            //two inherits from one (set's it's prototype)
            dynamic two = new HyperHypo(one);
            Console.WriteLine("two.Whassup: {0}", two.Whassup());
            Pause();
        }

        /// <summary>
        /// Build HyperJS on top of HyperHypo, adding "undefined" as the return value
        /// when a property is not defined on an object.  Also create the JavaScript
        /// "Global object".  Made Image as an add-on not part of the JS core.
        /// </summary>
        private static void HelloHyperJS()
        {
            Console.WriteLine("Second example: I created a HyperJS (JavaScript using HyperDynamo) Image class\nusing extension methods so that it looks like it's baked into HyperJS but isn't\n(see code Image.cs in the ConsoleApp project for details)\n==============================");
            dynamic img = JS.Image(20, 30);
            dynamic img2 = JS.Image();

            Console.WriteLine();
            Console.WriteLine("img.width: {0}\nimg.height: {1}", img.width, img.height);
            Console.WriteLine();
            Console.WriteLine("img.toString(): {0}\n", img.toString());
            Console.WriteLine();
            Console.WriteLine("img2.toString(): {0}\n", img2.toString());
            Console.WriteLine();
            Console.WriteLine("img.doStuff(Tony): {0}", img.doStuff("Tony"));
            Console.WriteLine();
            Console.WriteLine("img.getPrivate (should return 'private var'): {0}", img.getPrivate());
            img.setPrivate("set from outside");
            img2.setPrivate("set on img2 from outside");
            Console.WriteLine("img.getPrivate() (should return 'set from outside'): {0}", img.getPrivate());
            Console.WriteLine("img2.getPrivate() (should return 'set on img2 from outside'): {0}", img2.getPrivate());

            Pause();
        }
    }
}
