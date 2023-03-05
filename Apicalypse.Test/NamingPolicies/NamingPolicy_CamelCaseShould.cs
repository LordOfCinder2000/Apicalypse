using Apicalypse.Configuration;
using Apicalypse.NamingPolicies;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Apicalypse.Test.NamingPolicies;
public class NamingPolicy_CamelCaseShould
{
    public NamingPolicy NamingPolicy  { get; set; }

    public NamingPolicy_CamelCaseShould()
    {
        NamingPolicy = NamingPolicy.SnakeCase;
       
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ConvertPascalCaseToCamelCase()
    {
        var original = "SomePascalCaseString";
        var expected = "some_pascal_case_string";

        Assert.AreEqual(expected, NamingPolicy.ConvertName(original));
    }
}
