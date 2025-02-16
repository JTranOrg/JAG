using JTran.Extensions;
using System.Reflection;

namespace JAG.UnitTests
{
    [TestClass]
    public sealed class StorageTests
    {
        [TestMethod]
        public void Storage_run()
        {
            var storage = LoadTemplate("Storage\\storage.jtran");
            var result  = JTran.TransformerBuilder
                               .FromString(LoadTest("storage.jtran"))
                               .AddInclude("storage.jtran", storage)
                               .AddArguments(new Dictionary<string, object> { {"environment", "dev"} } )
                               .Build<string>()
                               .Transform("{ 'dummy': null}");

            Assert.IsNotNull(result);

            File.WriteAllText("c:\\Documents\\Testing\\JAG\\storage.json", result);
        }

        private string LoadTemplate(string fileName)
        {
            var path = Path.Combine(Assembly.GetExecutingAssembly().Location.SubstringBefore("\\bin"), "..\\Templates\\Resources", fileName);
            
            return File.ReadAllText(path);
        }

        private string LoadTest(string fileName)
        {
            var path = Path.Combine(Assembly.GetExecutingAssembly().Location.SubstringBefore("\\bin"), "..\\Tests", fileName);
            
            return File.ReadAllText(path);
        }
    }
}
