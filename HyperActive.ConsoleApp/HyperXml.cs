using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TonyHeupel.HyperCore;
using System.Xml.Linq;
using System.Xml;
using System.IO;
/*
 require 'net/http'
require 'rubygems'
require 'xmlsimple'

url = 'http://api.search.yahoo.com/WebSearchService/V1/webSearch?appid=YahooDemo&query=madonna&results=2'
xml_data = Net::HTTP.get_response(URI.parse(url)).body

data = XmlSimple.xml_in(xml_data)

data['Result'].each do |item|
   item.sort.each do |k, v|
      if ["Title", "Url"].include? k
         print "#{v[0]}" if k=="Title"
         print " => #{v[0]}\n" if k=="Url"
      end
   end
end
*/
namespace TonyHeupel.HyperXml
{
    public class XmlSimple : HyperHypo
    {
        #region Static members
        public static dynamic XmlIn(string xml)
        {
            var ms = new StringReader(xml);
            var doc = XDocument.Load(ms);
            
            dynamic thing = ParseElement(doc.Root);

            return thing;
        }


        protected static dynamic ParseElement(XElement element)
        {
            dynamic current = new XmlSimple();

            ParseAttributes(element, ref current);
            ParseElements(element, ref current);

            return current;
        }


        protected static void ParseAttributes(XElement element, ref dynamic elementObject)
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                //TODO: Consider following XMLSimple and use @{attribute-name} so it can deserialize
                //      back out properly...may need to add a case for inheriting from HyperHypo and allowing
                //      thing.id find thing["@id"] if it doesn't find thing["id"] directly.
                elementObject[GetAttributeName(attribute)] = attribute.Value;
            }
        }


        protected static void ParseElements(XElement element, ref dynamic elementObject)
        {
            foreach (XElement current in element.Elements())
            {
                string name = GetElementName(current);
                
                //  If there is already a property of this name, then
                //  we have a collection of these items.  
                //  Yes, it stinks that Library->Books->Book, Book, Book
                //  will be accessed via library.Books.Book[0] notation, 
                //  but since we can't assume that the Books element
                //  is ONLY an array object, we need to stick with it
                //  this way for now. (This is how XMLSimple does it).
                if (elementObject.ContainsKey(name))
                {
                    if (!(elementObject[name] is IEnumerable<object>))
                    {
                        List<object> value = new List<object>();
                        value.Add(elementObject[name]);
                        elementObject[name] = value;
                    }

                    elementObject[name].Add(ParseElement(current));
                }
                else
                {
                    elementObject[name] = ParseElement(current);
                }
            }
        }

        #region Member Name Formatting
        protected static readonly string AttributePrefix = "@";
        protected static readonly string ElementPrefix = "";

        protected static string GetAttributeName(XAttribute attribute)
        {
            return GetAttributeName(attribute.Name);
        }


        protected static string GetAttributeName(XName name)
        {
            return String.Format("{0}{1}", AttributePrefix, name.LocalName.ToLower());
        }


        protected static string GetElementName(XElement element)
        {
            return GetElementName(element.Name.LocalName.ToLower());
        }


        protected static string GetElementName(XName name)
        {
            return String.Format("{0}{1}", ElementPrefix, name.LocalName);
        }
        #endregion

        #endregion

        #region Overrides
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (base.TryGetMember(binder, out result)) return true;

            string name = binder.Name.ToLower();
            
            try
            {
                result = this[name];
                return true;
            }
            catch
            {
                try
                {
                    result = this[GetAttributeName(name)];
                    return true;
                }
                catch
                {
                    try
                    {
                        result = this[GetElementName(name)];
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
        #endregion
    }
}
