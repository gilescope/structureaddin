using System.Linq;
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

        [Test, Category("integration")]
        public void IwantToBeAbleToRetrieveAFilteredStructureBasedOnArbitraryJQL()
        {
            var api = new API(Test.Settings);

            var result = api.GetStructure(new Structure(102, "TestStructure"), "issue in structure(TestStructure) and resolution = Unresolved");
            Assert.That(result != null);
            Assert.That(result[0].LineItem.Summary, Is.EqualTo("Top Task 1"));
            var firstTree = result.First();
            var middleTask = firstTree.Children.Single();
            Assert.That(middleTask.Children.First().Included);
            Assert.That(!middleTask.Children.Skip(1).First().Included);
        }
        [Test, Category("integration")]
        public void IwantToBeAbleToRetrieveAFilteredStructureBasedOnArbitraryJQL2()
        {
            var api = new API(Test.Settings);

            var result = api.GetStructure(new Structure(102, "TestStructure"), "issue in structure(TestStructure) and resolution != Unresolved");
            Assert.That(result != null);
            Assert.That(result[0].LineItem, Is.Not.Null);
            var onlyResolvedTask = result.First();
            Assert.That(!onlyResolvedTask.Children.Any());
            Assert.That(onlyResolvedTask.Level == 0);

        }
    }
}
