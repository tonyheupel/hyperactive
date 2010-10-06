using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Dynamic;
using TonyHeupel.HyperCore;
using TonyHeupel.HyperJS;

using TonyHeupel.HyperActive.JSExtensions;  // To get the Image class

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
            TryXmlSimple();

            dynamic third = InhertiableHyperDictionary();

            ExpandoObjectLimited();

            HyperDynamoGiveJSLikeSettersAndGettersAndForEach();

            CombiningHyperDictionaryWithHyperDynamo(third);

            HelloHyperHypo();

            CreateAClassWithAccessorsInJSStyle();

            HelloHyperJS();
        }
       
        private static dynamic InhertiableHyperDictionary()
        {
            Console.WriteLine("Using HyperDictionary to show dictionary inheritance\n==================================================");
            var top = new HyperDictionary("top");
            top["eyes"] = "brown";
            top["hair"] = "pointy";

            var second = new HyperDictionary("second");
            second.InheritsFrom(top);
            second["hair"] = "straight";

            Console.WriteLine("top[\"eyes\"]:\t{0}", top["eyes"]);
            Console.WriteLine("top[\"hair\"]:\t{0}", top["hair"]);
            Console.WriteLine("second[\"eyes\"]:\t{0}", second["eyes"]);
            Console.WriteLine("second[\"hair\"]:\t{0}", second["hair"]);

            //Extends and removes using an IEnumerable<object> value
            top["things"] = new string[] { "first thing", "second thing" };

            var third = new HyperDictionary("third");
            third.InheritsFrom(second);
            third.RemoveProperty("hair");
            third.ExtendProperty("things", new object[] { 3, 4, 5 });

            //Output members of third - note the absence of "hair" member
            Console.Write("third members:\n");
            foreach (object o in third)
            {
                Console.WriteLine(o);
            }
            Console.WriteLine();

            // Output the extended list of items in "things", 
            // some from top and some from third.
            // And notice: DIFFERENT DATA TYPES!
            Console.Write("third things:\t");
            var things = third["things"] as IEnumerable<object>;
            foreach (object thing in things)
            {
                Console.Write(" | " + thing.ToString());
            }
            Console.Write(" | ");
            Console.WriteLine();

            Console.WriteLine("Making sure TryGetProperty works...should have 'not set' as the next value...");
            object stomach;
            if (third.TryGetProperty("stomach", out stomach)) Console.WriteLine("third\tstomach:\t{0}", stomach);
            else Console.WriteLine("not set - third.stomach");

            
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
                Console.WriteLine("EXPECTED EXCEPTION - NO INDEXER/NAME ALLOWED ON EXPANDOOBJECT (thing[\"For\"] = \"everyone\"");
                Console.WriteLine(ex.ToString());
            }

            DumpEnumerable(stuff);
            Pause();
        }

        /// <summary>
        /// Show how even with a basic Dictionary member provider, 
        /// HyperDynamo is cooler than ExpandoObject
        /// </summary>
        private static void HyperDynamoGiveJSLikeSettersAndGettersAndForEach()
        {
            Console.WriteLine("Using HyperDynamo with Plain Dictionary MemberProvider\n==================================================");

            dynamic person = new HyperDynamo();
            person.FirstName = "Tony";
            person.LastName = "Heupel";
            person["MiddleInitial"] = "C";

            Console.WriteLine(person["FirstName"] + " " + person.MiddleInitial + ". " + person.LastName);

            DumpEnumerable(person);
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

            dynamic dynoThird = new HyperDynamo(third);  //Manually use HyperDictionary with HyperDynamo
            dynoThird.toes = "third toes set through dynamic property";
            Console.WriteLine("eyes:\t{0}", dynoThird["eyes"]);
            Console.WriteLine("eyes:\t{0}", dynoThird.eyes);
            Console.WriteLine("toes:\t{0}", dynoThird["toes"]);
            Console.WriteLine("toes:\t{0}", dynoThird.toes);

            Console.WriteLine();
            try
            {
                //Should throw an exception since it got removed at this level
                Console.WriteLine("hair:\t{0}", dynoThird.hair);
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException rbe)
            {
                Console.WriteLine("EXPECTED EXCEPTION SINCE PROPERTY WAS REMOVED:\n{0}", rbe);
            }
            Pause();

            Console.WriteLine("Properties in the HyperDynamo object built off of 3 levels of HyperDictionary\ninheritance and a couple dynamic property setters\n========================================================================");
            DumpEnumerable(dynoThird);
            Pause();
        }

        /// <summary>
        /// Formalize a HyperDynamo using a HyperDictionary into a concrete class
        /// called HyperHypo (Hyper - more than C#; Hypo - less than JavaScript)
        /// </summary>
        private static void HelloHyperHypo()
        {
            Console.WriteLine("Using HyperDynamo with HyperDictionary membership provider\n==========================================================");

            Console.WriteLine("First example: prototype inheritance where only one.Whassup is set");
            dynamic one = new HyperHypo();
            one.Whassup = new Func<string>(SayHello);
            Console.WriteLine("one.Whassup(): {0}", one.Whassup());

            //two inherits from one (set's it's prototype)
            dynamic two = new HyperHypo(one);
            two.HowsItGoing = new Func<string, string>(name => String.Format("How's it going, {0}?", name));
            Console.WriteLine("two.Whassup(): {0}", two.Whassup());
            Console.WriteLine("two.HowsItGoing(\"buddy\"): {0}", two.HowsItGoing("buddy"));
            Pause();
        }

        private static void CreateAClassWithAccessorsInJSStyle()
        {
            Console.WriteLine("Using HyperHypo (HyperDynamo with HyperDictionary)\nand closures to create JavaScript-style object declarations\n==========================================================================");
            // Define the class as a function constructor and 
            // private variables using closures.
            // NOTE: No overrides on constructor -- 
            //       to do this, move the definition out into
            //       static methods and just call them directly
            //       (see the Image and the global JS functions
            //       like Boolean in this project).
            dynamic Person = new Func<string, string, dynamic>(delegate(string firstName, string lastName)
            {
                var _firstName = firstName;
                var _lastName = lastName;


                dynamic p = new HyperHypo();
                p.getFirstName = new Func<dynamic>(delegate() { return _firstName; });
                p.getLastName = new Func<dynamic>(delegate() { return _lastName; });

                p.setFirstName = new Func<string, object>(value => _firstName = value);
                p.setLastName = new Func<string, object>(value => _lastName = value);
                
                p.toString = new Func<string>(delegate() { return String.Format("{0} {1}", _firstName, _lastName); });
                return p;
            });

            dynamic me = Person("Tony", "Heupel");
            dynamic singer = Person(null, null);

            singer.setFirstName("Paul");
            singer.setLastName("Hewson");

            OutputPeople(me, singer);
        }

        /// <summary>
        /// Output people from the CreateACl
        /// </summary>
        /// <param name="me"></param>
        /// <param name="singer"></param>
        private static void OutputPeople(dynamic me, dynamic singer)
        {
            Console.WriteLine("me.getFirstName():\t{0}", me.getFirstName());
            Console.WriteLine("me.getLastName():\t{0}", me.getLastName());
            Console.WriteLine("singer.getFirstName():\t{0}", singer.getFirstName());
            Console.WriteLine("singer.getLastName():\t{0}", singer.getLastName());
            Console.WriteLine("me:\t{0}", me.toString());
            Console.WriteLine("singer:\t{0}", singer.toString());
            Console.WriteLine();

            // Notice that with the closure at the time the constructor was called,
            // each Person has it's own variable scope (closure) that does not
            // interfere -- even if the constructor is a staic method!
            DumpEnumerable(me);
            DumpEnumerable(singer);
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
            // NOTE: Image functions are extension methods added inside this assembly
            //       and made available with 'using TonyHeupel.HyperActive.JSExtensions;' 
            dynamic img = JS.cs.NewImage(20, 30);
            dynamic img2 = JS.cs.NewImage();

            Console.WriteLine();
            Console.WriteLine("img.width: {0}\nimg.height: {1}", img.width, img.height);
            Console.WriteLine();
            Console.WriteLine("img.toString(): {0}\n", img.toString());
            Console.WriteLine();
            Console.WriteLine("img2.toString(): {0}\n", img2.toString());

            Pause();
        }


        /// <summary>
        /// Dump the members of an enumerable to the console
        /// </summary>
        /// <param name="thing"></param>
        private static void DumpEnumerable(dynamic thing)
        {
            foreach (object o in thing)
            {
                Console.WriteLine(o);
            }
        }

        /// <summary>
        /// Try using an XMLSimple style xml parser for C# 4
        /// </summary>
        private static void TryXmlSimple()
        {
            string xml = @"<Library Name=""Covington""><Books><Book Title=""Soon Will Come the Light"" Author=""Tom McKean""></Book><Book Title=""Fall of Giants"" Author=""Ken Follett"" /></Books></Library>";

            dynamic library = TonyHeupel.HyperXml.XmlSimple.XmlIn(xml);

            DumpEnumerable(library);
            DumpEnumerable(library.Books);
            DumpEnumerable(library.Books.Book[0]);
            DumpEnumerable(library.Books.Book[1]);
            Console.WriteLine(library.Books.Book[1].Title);
        }
    }
}
