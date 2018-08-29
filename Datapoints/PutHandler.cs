using LDPDatapoints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace ECA2LD.Datapoints
{
    public static class PutHandler
    {
        private static TurtleParser turtleParser = new TurtleParser();

        public static void handleRequest(HttpEventArgs e, string Route, Action<Graph> resultProcessor)
        {
            string responseString = "";
            if (e.request.ContentType.Equals("text/turtle"))
            {
                try
                {
                    handleTurtle(e.request.InputStream, resultProcessor);
                    e.response.StatusCode = 201;
                    responseString = Route;
                }
                catch (Exception ex)
                {
                    e.response.StatusCode = 500;
                    responseString = "Could not process provided data. Reason: " + ex.Message;
                }
            }
            else
            {
                responseString = "Provided MIME type is not supported. Expected text/turtle as Content-Type.";
                e.response.StatusCode = 415;
            }
            e.response.OutputStream.Write(Encoding.UTF8.GetBytes(responseString), 0, responseString.Length);
            e.response.OutputStream.Close();
        }

        private static void handleTurtle(Stream InputStream, Action<Graph> resultProcessor)
        {
            using (Stream input = InputStream)
            {
                using (StreamReader reader = new StreamReader(input, Encoding.UTF8))
                {
                    Graph receivedGraph = new Graph();
                    turtleParser.Load(receivedGraph, reader);
                    resultProcessor(receivedGraph);
                }
            }
        }
    }
}
