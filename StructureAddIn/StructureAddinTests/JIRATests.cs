﻿using System.Linq;
using NUnit.Framework;
using StructureAddinAPI;
using StructureInterfaces;

namespace StructureAddinTests
{
    [TestFixture]
    public class JIRATests
    {
        [Test]
        public void Can_get_list_of_structures_and_one_is_setup_called_TestStructure()
        {
            var map = new API(Test.Settings).GetAvailableStructures();
            Assert.That(map.Count, Is.GreaterThan(1));//Global structure is one.

            Assert.That(map.Any(structure => structure.Name == "TestStructure"));
        }

        [Test, Category("integration")]
        public void CanRetrieveStructure()
        {
            var api = new API(Test.Settings);

            var result = api.GetStructure(new Structure(102, "TestStructure"));
            Assert.That(result != null);
            Assert.That(result[0].LineItem.Summary, Is.EqualTo("Top Task 1"));
        }
    }
}