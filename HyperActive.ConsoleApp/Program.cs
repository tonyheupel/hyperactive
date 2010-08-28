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
        public static string SayHello() { return "Hello"; }

        static void Main(string[] args)
        {
            #region ExpandoObject coolness
            Console.WriteLine("Using Expando Object\n====================");
            dynamic stuff = new ExpandoObject();
            stuff.Something = "something";
            stuff.Fun = "fun";

            foreach (object o in stuff)
            {
                Console.WriteLine(o);
            }

            Pause();
            #endregion

            #region Simple, single-level dynamic HyperDictionary/dictionary, like JavaScript
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
            #endregion

            #region Inheritable HyperDictionary/dictionary
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
            #endregion

            #region Awesome dynamic mappings with inheritance!!
            Console.WriteLine("Using HyperDynamo with HyperDictionary MemberProvider to show\ndynamic mappings and inheritance!\n==================================================");
            
            dynamic thirdDyn = new HyperDynamo("d-third", third);  //Manually use HyperDictionary with HyperDnamo
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
            #endregion

            #region HyperHypo --> Enable JS.CS?
            Console.WriteLine("Using HyperHypo (formalized HyperDynamo with HyperDictionary) to enable JS.cs\n(JavaScript-style programming within C#) called HyperJS\n==================================================");

            Console.WriteLine("First example: prototype inheritance where only one.Whassup is set");
            dynamic one = new HyperHypo();
            one.Whassup = new Func<string>(SayHello);
            Console.WriteLine("one.Whassup: {0}", one.Whassup());

            //two inherits from one (set's it's prototype)
            dynamic two = new HyperHypo(one);
            Console.WriteLine("two.Whassup: {0}", two.Whassup());
            Pause();

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
            #endregion

            #region Store it in RavenDB - Don't want to take this dependency right now...
            //var store = new DocumentStore { Url = "http://localhost:8080" };
            //store.Initialize();

            //using (var session = store.OpenSession())
            //{
            //    session.Store(top);
            //    session.Store(third);
            //    session.Store(thirdDyn);

            //    dynamic onlyMine = new ExpandoObject();
            //    onlyMine.Id = "own-" +
            //        third.Id;
            //    onlyMine.Items = third.OwnTuples;
            //    session.Store(onlyMine);
            //    session.SaveChanges();
            //}
            //Pause();
            #endregion
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press any key...");
            Console.ReadKey();
        }
    }
}
