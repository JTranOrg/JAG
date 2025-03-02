using JTran;
using JTran.Extensions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace JAG.UnitTests
{
    [TestClass]
    public sealed class MicroServiceTests
    {
        [TestMethod]
        [DataRow("rota-inventory", "dev")]
        public void MicroService_run(string name, string environment)
        {
            // First create the JAG file. JAG is a Json schema with a manifest of Azure resources to deploy
            var jag = CreateJAGFile(name, environment);

            Assert.IsNotNull(jag);

            var flat = CreateFlatManifest(jag);

            Assert.IsNotNull(flat);

            var arm = CreateARMTemplate(flat);

            Assert.IsNotNull(arm);

            File.WriteAllText("c:\\Documents\\Testing\\JAG\\" + name + ".jag", jag);
            File.WriteAllText("c:\\Documents\\Testing\\JAG\\" + name + ".flat", flat);
            File.WriteAllText("c:\\Documents\\Testing\\JAG\\" + name + ".arm", arm);
        }

        private string CreateJAGFile(string name, string environment)
        {
            var template = LoadTemplate(name + ".jtran");
            var include = LoadTemplate("microservice.jtran");
            var result  = JTran.TransformerBuilder
                               .FromString(template)
                               .AddInclude("microservice.jtran", include)
                               .AddArguments(new Dictionary<string, object> { {"environment", environment} } )
                               .Build<string>()
                               .Transform("{ 'dummy': null}");

            return result;
        }

        private string CreateFlatManifest(string jagFile)
        {
            var template = LoadTemplate("jagtoflat.jtran");
            var result   = JTran.TransformerBuilder
                                .FromString(template)
                                .AddInclude("resourcename",  LoadInclude("resourcename.jtran"))
                                .AddInclude("helpers",       LoadInclude("helpers.jtran"))
                                .AddResources()
                                .Build<string>()
                                .Transform(jagFile);

            return result;
        }

        private string CreateARMTemplate(string jagFile)
        {
            var template = LoadTemplate("jagtoarm.jtran");
            var result   = JTran.TransformerBuilder
                                .FromString(template)
                                .AddInclude("resourcename",  LoadInclude("resourcename.jtran"))
                                .AddInclude("helpers",       LoadInclude("helpers.jtran"))
                                .AddResources()
                                .Build<string>()
                                .Transform(jagFile);

            return result;
        }

        private string LoadInclude(string fileName)
        {
            var path = Path.Combine(Assembly.GetExecutingAssembly().Location.SubstringBefore("\\bin"), "..\\Templates\\" + fileName);
            
            return File.ReadAllText(path);
        }

        public static string LoadResourceInclude(string fileName)
        {
            var path = Path.Combine(Assembly.GetExecutingAssembly().Location.SubstringBefore("\\bin"), "..\\Templates\\Resources\\" + fileName);
            
            return File.ReadAllText(path);
        }

        private static string LoadTemplate(string fileName)
        {
            var path = Path.Combine(Assembly.GetExecutingAssembly().Location.SubstringBefore("\\bin"), "..\\Tests\\" + fileName);
            
            return File.ReadAllText(path);
        }
    }

    public static class Extensions
    {
        private static List<string> _includes = new List<string>
        {
            "storageaccount",
            "applicationinsights",
            "keyvault",
            "appserviceplan",
            "functionapp",
            "servicebus",
            "servicebusqueue",
            "loganalyticsworkspace",
            "roleassignment_apptoresource"
        };

        public static TransformerBuilder AddResources(this TransformerBuilder builder)
        {
            foreach(var res in _includes)
                builder.AddInclude(res, MicroServiceTests.LoadResourceInclude($"{res}.jtran"));

            return builder;
        }
    }
}
