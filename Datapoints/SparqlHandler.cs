using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace ECA2LD.Datapoints
{
    public static class SparqlExecutor
    {
        static SparqlQueryParser parser = new SparqlQueryParser();

        public static SparqlResultSet PerformQuery(string q, Graph g)
        {
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("ldp", new Uri("http://www.w3.org/ns/ldp#"));
            queryString.Namespaces.AddNamespace("dct", new Uri("http://purl.org/dc/terms/"));
            queryString.CommandText = q;
            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);

            TripleStore store = new TripleStore();
            store.Add(g);

            ISparqlQueryProcessor processor = new LeviathanQueryProcessor(store);
            SparqlResultSet results = processor.ProcessQuery(query) as SparqlResultSet;

            return results;
        }
    }
}
