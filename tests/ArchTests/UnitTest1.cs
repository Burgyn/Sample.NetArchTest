using EShop.Domains;
using EShop.Infrastructure;
using FluentAssertions;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using NetArchTest.Rules;

namespace ArchTests;

public class UnitTest1
{
    [Fact]
    public void RepositoriesShouldBeLocatedInInfrastructureNamespace()
    {
        var result = Types.InAssembly(typeof(ProductRepository).Assembly)
            .That()
            .ImplementInterface(typeof(IRepository))
            .Should()
            .ResideInNamespaceEndingWith("Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DtoShouldBeRecordType()
    {
        var result = Types.InAssembly(typeof(ProductDto).Assembly)
            .That()
            .HaveNameEndingWith("Dto")
            .Should()
            .BeRecord()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainShouldNotReferenceInfrastructure()
    {
        var result = Types.InAssembly(typeof(ProductDto).Assembly)
            .That()
            .ResideInNamespace("EShop.Domains")
            .ShouldNot()
            .HaveDependencyOn("EShop.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}

public class IsRecordRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
        => type.GetMethods().Any(m => m.Name == "<Clone>$");
}

public static class CustomRules
{
    public static ConditionList BeRecord(this Conditions conditions)
        => conditions.MeetCustomRule(new IsRecordRule());
}