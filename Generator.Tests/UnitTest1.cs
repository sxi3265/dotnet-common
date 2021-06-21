using System;
using System.IO;
using Fluid;
using Fluid.Parser;
using Xunit;
using Xunit.Abstractions;

namespace Generator.Tests
{
    public class UnitTest1
    {
        private ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test1()
        {
            var parser = new FluidParser();
            var model = new { Firstname = "Bill", Lastname = "Gates" };
            if (parser.TryParse(File.ReadAllText("template.liquid"), out var template,out var error))
            {
                var context = new TemplateContext(model);

                _output.WriteLine(template.Render(context));
            }
            else
            {
                _output.WriteLine($"Error: {error}");
            }
        }
    }
}
