using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Program
{

    class Program
    {  

        static void Main(string[] args)
        {
            //excecution time start
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            string path = @"C:\Users\Dat\Desktop\SAT\__working\family\box.rfa";
            //Extract AtomXml from Rfa
            string atomXml = ExtractAtomXmlFromRfa(path);
            //Console.WriteLine(atomXml);
            
            //initialize XML document class from string
            XmlDocument xmlDoc = new XmlDocument();
            //load XML
            xmlDoc.LoadXml(atomXml);
            //rfa <entry> element
            XmlNode root = xmlDoc.DocumentElement;

            //Display the contents of the child nodes.
            PrintXmlNodes(xmlDoc);

            //dynamic jsonText = JsonConvert.SerializeXmlNode(xmlDoc);
            //JObject json = JsonConvert.DeserializeObject<JObject>(jsonText);

            //foreach (JProperty property in json.Properties())
            //{
            //    property.Children();
            //    Console.WriteLine(property.Name + " - " + property.Value);
            //}

            //excecution time stop
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

            Console.ReadKey();
        }


        /// <summary>
        /// Reads line by line and Extracts AtomXML from revit family file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>string</returns>
        static string ExtractAtomXmlFromRfa(string path)
        {        
            //define variables
            string startString = "<?xml";
            string endString = "</entry>";
            string chunks = "";
            string xml = "";
            bool startStringFound = false;
            bool endStringFound = false;

            StreamReader streamReader = File.OpenText(path);

            while ((!startStringFound || !endStringFound) && !streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                Byte[] lineByte = Encoding.ASCII.GetBytes(line);
                string lineEncoded = Encoding.ASCII.GetString(lineByte);
                //Console.WriteLine(lineEncoded);
                if (lineEncoded.Contains(startString))
                {
                    //Console.WriteLine("start string {0} found in the following line:\n{1}", startString, lineEncoded);
                    startStringFound = true;
                }
                if (startStringFound)
                {
                    chunks += lineEncoded;
                    if (lineEncoded.Contains(endString))
                    {
                        //Console.WriteLine("end string {0} found in the following line:\n{1}", endString, lineEncoded);
                        int startIndex = chunks.IndexOf(startString);
                        int endIndex = chunks.IndexOf(endString);
                        xml = chunks.Substring(startIndex, endIndex + endString.Length - startIndex);
                        //Console.WriteLine(xml);
                        endStringFound = true;
                    }
                }
            }
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(xml);
            return xml;
        }
                
        /// <summary>
        /// Prints in console Xml elements and Inner text
        /// </summary>
        /// <param name="node"></param>
        static void PrintXmlNodes(XmlNode node)
        {            
            if (node.HasChildNodes)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    XmlNode inode = node.ChildNodes[i];
                    if (node.ChildNodes[i].FirstChild != null)
                    {
                        if (node.ChildNodes[i].NodeType == XmlNodeType.Element && node.ChildNodes[i].FirstChild.NodeType == XmlNodeType.Element)
                        {                            
                            int parentCount = NodeParentCount(node.ChildNodes[i], 0);
                            Console.WriteLine(new String('=',parentCount) + " " + node.ChildNodes[i].LocalName);
                            PrintXmlNodes(node.ChildNodes[i]);
                        }
                        if (node.ChildNodes[i].NodeType == XmlNodeType.Element && node.ChildNodes[i].FirstChild.NodeType == XmlNodeType.Text)
                        {
                            int parentCount = NodeParentCount(node.ChildNodes[i], 0);                           
                            Console.WriteLine(new String('=', parentCount) + " " + node.ChildNodes[i].LocalName + " : " + node.ChildNodes[i].InnerText);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("ELSE");
            }
        }

        /// <summary>
        /// Counts the number of parents that a XML node is nested in
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="count">int initial counter. Usually, 0</param>
        /// <returns>int</returns>
        static int NodeParentCount(XmlNode node, int count)
        {            
            if (node.ParentNode != null)
            {
                count++;                
                return NodeParentCount(node.ParentNode, count);                
            }
            else
            {
                return count;
            }
        }
    }
}
