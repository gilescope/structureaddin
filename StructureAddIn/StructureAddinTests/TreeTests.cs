using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using StructureAddinAPI;
using StructureInterfaces;
using StructureSource;

namespace StructureAddinTests
{
    [TestFixture]
    public class PreviousTests
    {
        [Test]
        public void KnowYourDependency()
        {
            var api = new API(null);

            //Root only
            List<ITree> forrest = api.ParseForrest(@"
            11:1,
                22:2");
            Assert.That(forrest[0].Id == 11
                && forrest[0].Previous == null);

            // As all parents are summary tasks with implicit depenencies on their kids
            // there is no need to depend explicitly on the final child.
            forrest = api.ParseForrest(@"
            11:1,
                22:2,
            33:1,
                44:2");

            Assert.That(forrest[1].Id == 33);
            Assert.That(forrest[1].Previous, Is.EqualTo(forrest[0]));        
        }
    }

    [TestFixture]
    public class TreeTests
    {
        [Test]
        public void CanParseTree()
        {
            var api = new API(null);

            //Root only
            List<ITree> forrest = api.ParseForrest(@"
            11:0");
            Assert.That(forrest.Count == 1 && forrest[0].Id == 11);

            //Child
            forrest = api.ParseForrest(@"
            11:0,
                22:1");
            Assert.That(forrest.Count == 1 && forrest[0].Id == 11
                && forrest[0].Children.Single().Id == 22);

            //GrandChild
            forrest = api.ParseForrest(@"
            11:0,
                22:1,
                    33:2");
            Assert.That(forrest.Count == 1 && forrest[0].Id == 11
                && forrest[0].Children.Single().Id == 22
                && forrest[0].Children.Single().Children.Single().Id == 33
                );

            //Two children
            forrest = api.ParseForrest(@"
            11:0,
                22:1,
                33:1");
            Assert.That(forrest[0].Children.Count() == 2);
            Assert.That(forrest[0].Children.First().Id == 22);
            Assert.That(forrest[0].Children.Skip(1).Single().Id == 33);

            //Grandchild
            forrest = api.ParseForrest(@"
            11:0,
                22:1,
                    33:2,
                44:1");
            Assert.That(forrest[0].Children.Count() == 2);
            Assert.That(forrest[0].Children.First().Children.Single().Id == 33);
            Assert.That(forrest[0].Children.Skip(1).Single().Id == 44);

            //two levels up
            forrest = api.ParseForrest(
            @"11:0,
                22:1,
                    33:2,
                        44:3,
                55:1");//two levels up
            Assert.That(forrest[0].Children.Count() == 2);
            Assert.That(forrest[0].Children.First().Children.Single().Children.Single().Id == 44);
            Assert.That(forrest[0].Children.Skip(1).Single().Id == 55);

            //Multiple roots:
            forrest = api.ParseForrest(@"
            11:0,
            22:0,
                33:1");
            Assert.That(forrest.Count, Is.EqualTo(2));
            Assert.That(forrest[0].Id, Is.EqualTo(11));
            Assert.That(!forrest[0].Children.Any());
            
            Assert.That(forrest[1].Id, Is.EqualTo(22));
            Assert.That(forrest[1].Children.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CanIncludeCorrectJIRAs()
        {
            var api = new API(null);

            //All selected
            Forrest forrest = MakeForrest(api, @"
            11:0,
                22:1", 
            @"<xml><issues>
                <item><id>11</id></item>
                <item><id>22</id></item>
            </issues></xml>");

            var tree = forrest.Children.Single();
            Assert.That(tree.Included);
            Assert.That(tree.Level == 0);
            Assert.That(tree.Children.Single().Included);

            //Top selected
            forrest = MakeForrest(api, @"
            11:0,
                22:1",
            @"<xml><issues>
                <item><id>11</id></item>
            </issues></xml>");

            tree = forrest.Children.Single();
            Assert.That(tree.Included);
            Assert.That(tree.Level == 0);
            Assert.That(!tree.Children.Single().Included);

            //Bottom selected
            forrest = MakeForrest(api, @"
            11:0,
                22:1",
            @"<xml><issues>
                <item><id>22</id></item>
            </issues></xml>");

            tree = forrest.Children.Single();
            Assert.That(tree.Included);
            Assert.That(tree.Id == 22);
            Assert.That(tree.Level == 0);
            Assert.That(!tree.Children.Any());
            Assert.That(tree.AnyIncluded);

            //None selected
            forrest = MakeForrest(api, @"
            11:0,
                22:1",
            @"<xml><issues>
            </issues></xml>");
           
            Assert.That(!forrest.Any());
            Assert.That(forrest.Level < 0);            
        }

        private static Forrest MakeForrest(API api, string forrestIds, string xml)
        {
            Forrest forrest = api.ParseForrest(forrestIds);

            XElement jiras = XElement.Parse(xml);
            api.BindJIRAsToForrest(forrest, jiras);
            return forrest;
        }
    }
}