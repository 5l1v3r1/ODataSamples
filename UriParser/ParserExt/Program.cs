﻿using System;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Metadata;

namespace ParserExt
{
    class Program
    {
        private static readonly Uri ServiceRoot = new Uri("http://demo/odata.svc/");
        private static ParserExtModel extModel = new ParserExtModel();

        static void Main()
        {
            TestCaseInsensitive();
            TestUnqualified();
            TestStringAsEnum();
            TestCombination1();
            TestCombination2();
            TestStringRep();
        }

        #region Test built-in Resolver
        private static void TestCaseInsensitive()
        {
            Console.WriteLine("TestCaseInsensitive");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/People(1)/Pets/TestNS.Fish?$orderby=Color"));

            var path = parser.ParsePath();
            var clause = parser.ParseOrderBy();
            Console.WriteLine(path.ToLogString());
            Console.WriteLine(clause.Expression.ToLogString());

            var parser2 = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/people(1)/pets/testns.fish?$ORDERBY=color"))
            {
                Resolver = new ODataUriResolver { EnableCaseInsensitive = true }
            };

            // Identical to path and clause
            var path2 = parser2.ParsePath();
            var clause2 = parser2.ParseOrderBy();
            Console.WriteLine(path2.ToLogString());
            Console.WriteLine(clause2.Expression.ToLogString());
        }

        private static void TestUnqualified()
        {
            Console.WriteLine("TestUnqualified");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/People(1)/Addr/TestNS.GetZip"));

            var path = parser.ParsePath();
            Console.WriteLine(path.ToLogString());

            var parser2 = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/People(1)/addr/getzip"))
            {
                Resolver = new UnqualifiedODataUriResolver { EnableCaseInsensitive = true }
            };

            // Identical to path
            var path2 = parser2.ParsePath();
            Console.WriteLine(path2.ToLogString());
        }

        private static void TestStringAsEnum()
        {
            Console.WriteLine("TestStringAsEnum");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/GetColorCmykImport(col=TestNS.Color'Blue')"));

            var path = parser.ParsePath();
            Console.WriteLine(path.ToLogString());

            var parser2 = new ODataUriParser(
                    extModel.Model,
                    ServiceRoot,
                    new Uri("http://demo/odata.svc/GetColorCmykImport(col='Blue')"))
            {
                Resolver = new StringAsEnumResolver { EnableCaseInsensitive = true }
            };

            // Identical to path
            var path2 = parser2.ParsePath();
            Console.WriteLine(path2.ToLogString());
        }
        #endregion

        #region Test combination
        private static void TestCombination1()
        {
            Console.WriteLine("TestCombination1");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/PetSet(Id=1, Color=TestNS.Color'Blue')/TestNS.HasColor(col=TestNS.Color'Blue')"));
            var path = parser.ParsePath();
            Console.WriteLine(path.ToLogString());

            var parser2 = new ODataUriParser(
                    extModel.Model,
                    ServiceRoot,
                    new Uri("http://demo/odata.svc/petset(id=1, color='Blue')/hascolor(COL='Blue')"))
            {
                Resolver = new AllInOneResolver { EnableCaseInsensitive = true }
            };

            // Identical to path

            var path2 = parser2.ParsePath();
            Console.WriteLine(path2.ToLogString());
        }

        private static void TestCombination2()
        {
            Console.WriteLine("TestCombination2");
            var parser = new ODataUriParser(
               extModel.Model,
               ServiceRoot,
               new Uri("http://demo/odata.svc/PetSet?$filter=Color eq TestNS.Color'Red'"));
            var path = parser.ParsePath();
            Console.WriteLine(path.ToLogString());
            var clause = parser.ParseFilter();
            Console.WriteLine(clause.Expression.ToLogString());

            var parser2 = new ODataUriParser(
                 extModel.Model,
                 ServiceRoot,
                 new Uri("http://demo/odata.svc/petset?$FILTER=color EQ 'Red'"))
            {
                Resolver = new AllInOneResolver { EnableCaseInsensitive = true }
            };

            var path2 = parser2.ParsePath();
            Console.WriteLine(path2.ToLogString());
            var clause2 = parser2.ParseFilter();
            Console.WriteLine(clause2.Expression.ToLogString());
        }
        #endregion

        private static void TestStringRep()
        {
            Console.WriteLine("TestStringRep");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/People?$filter=Name eq 'Red' mul 3"));
            try
            {
                // Would thrown exception
                var clause = parser.ParseFilter();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var parser2 = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/People?$filter=Name eq 'Red' mul 3"))
            {
                Resolver = new StringRepResolver()
            };

            // Would work
            var clause2 = parser2.ParseFilter();
            Console.WriteLine(clause2.Expression.ToLogString());
        }
    }
}
