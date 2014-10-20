﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Microsoft.OData.Core;
using ODataSamples.Common;
using ODataSamples.Common.Model;

namespace ODataSamples.Writer
{
    class Program
    {
        private static readonly Uri ServiceRoot = new Uri("http://demo/odata.svc/");
        private static readonly ParserExtModel ExtModel = new ParserExtModel();
        private static readonly ODataFeed Feed;
        private static readonly ODataComplexValue Address1;
        private static readonly ODataEntry PersonEntry;
        private static readonly ODataEntry PetEntry;
        private static readonly ODataEntry FishEntry;
        private static readonly ODataMessageWriterSettings BaseSettings = new ODataMessageWriterSettings()
        {
            DisableMessageStreamDisposal = true,
            Indent = true,
        };

        static Program()
        {
            Feed = new ODataFeed();

            Address1 = new ODataComplexValue()
            {
                InstanceAnnotations = new List<ODataInstanceAnnotation>()
                {
                    new ODataInstanceAnnotation("ns.ann2", new ODataPrimitiveValue("hi"))
                },
                TypeName = "TestNS.Address", // Need this for parsed model.
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = "ZipCode",
                        Value = "200",
                    },
                },
            };

            PersonEntry = new ODataEntry()
            {
                InstanceAnnotations = new List<ODataInstanceAnnotation>()
                {
                    new ODataInstanceAnnotation("ns.ann1", new ODataPrimitiveValue("hi"))
                },
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = "Id",
                        Value = 1,
                    },
                    new ODataProperty()
                    {
                        Name = "Name",
                        Value = "Shang",
                    },
                    new ODataProperty()
                    {
                        Name = "Addr",
                        Value = Address1
                    }
                },
            };

            PetEntry = new ODataEntry()
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = "Id",
                        Value = 1,
                    },
                    new ODataProperty()
                    {
                        Name = "Color",
                        Value = new ODataEnumValue("Red")
                    },
                },
            };

            FishEntry = new ODataEntry()
            {
                TypeName = "TestNS.Fish",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = "Id",
                        Value = 2,
                    },
                    new ODataProperty()
                    {
                        Name = "Color",
                        Value = new ODataEnumValue("Blue"),
                    },
                    new ODataProperty()
                    {
                        Name = "Name",
                        Value = "Qin",
                    },
                },
            };
        }

        static void Main(string[] args)
        {
            WriteTopLevelFeed();
            WriteTopLevelFeed(false);
            WriteTopLevelEntry();
            ContainmentTest.FeedWriteReadNormal();
        }

        private static void WriteTopLevelFeed(bool enableFullValidation = true)
        {
            var msg = ODataSamplesUtil.CreateMessage();

            var settings = new ODataMessageWriterSettings(BaseSettings)
            {
                ODataUri = new ODataUri()
                {
                    ServiceRoot = new Uri("http://demo/odata.svc/PetSet")
                },
                EnableFullValidation = enableFullValidation
            };

            using (var omw = new ODataMessageWriter((IODataResponseMessage)msg, settings, ExtModel.Model))
            {
                var writer = omw.CreateODataFeedWriter(ExtModel.PetSet);
                writer.WriteStart(Feed);
                writer.WriteStart(PetEntry);
                writer.WriteEnd();
                writer.WriteStart(FishEntry);
                writer.WriteEnd();
                writer.WriteEnd();
            }

            Console.WriteLine(ODataSamplesUtil.MessageToString(msg));
        }

        private static void WriteTopLevelEntry()
        {
            var msg = ODataSamplesUtil.CreateMessage();
            msg.PreferenceAppliedHeader().AnnotationFilter = "*";

            var settings = new ODataMessageWriterSettings(BaseSettings)
            {
                ODataUri = new ODataUri()
                {
                    ServiceRoot = new Uri("http://demo/odata.svc/People(2)")
                },
            };

            using (var omw = new ODataMessageWriter((IODataResponseMessage)msg, settings, ExtModel.Model))
            {
                var writer = omw.CreateODataEntryWriter(ExtModel.People);
                writer.WriteStart(PersonEntry);
                writer.WriteEnd();
            }

            Console.WriteLine(ODataSamplesUtil.MessageToString(msg));
        }
    }
}
