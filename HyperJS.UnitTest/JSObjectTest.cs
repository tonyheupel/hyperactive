using TonyHeupel.HyperJS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Dynamic;

namespace TonyHeupel.HyperJS.UnitTest
{
    
    
    /// <summary>
    /// Tests for JSObject, which is the base class for 
    /// the dynamic objects for HyperJS/JS.cs
    ///</summary>
    [TestClass()]
    public class JSObjectTest
    {
        /// <summary>
        ///A test for JSObject Constructor
        ///</summary>
        [TestMethod()]
        public void JSObjectConstructorTest()
        {
            JSObject target = new JSObject();
            Assert.IsInstanceOfType(target, typeof(HyperCore.HyperHypo));
        }

        [TestMethod]
        public void JSObjectMissingMembersAreUndefined()
        {
            dynamic foo = JS.Object();
            Assert.AreSame(JS.undefined, foo.bar);
        }

        [TestMethod]
        public void FeatureDetectionWorks_KindOf()
        {
            dynamic foo = JS.Object();
            if (foo.bar) //Can do this simply because it's undefined, but will not work if it is defined, so not good practice yet...
            {
                Assert.Fail("bar should not be defined yet");
            }

            foo.bar = new Func<string>(delegate() {  return "this is bar"; });
            if (JS.Boolean(foo.bar)) //Had to use the Boolean function explicitely...if only EVERYTHING was a JSObject...someday...
            {
                Assert.AreEqual("this is bar", foo.bar());
            }
            else
            {
                Assert.Fail("bar should be defined now");
            }
        }
    }
}
