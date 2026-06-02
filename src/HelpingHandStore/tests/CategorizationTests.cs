using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;
using Xunit;

namespace HelpingHandStore.Domain.Tests;
public class CategorizationTests
{
    [Fact]
    public void CT001_CategoryFromStandardList_CategoryAssignedSuccessfully()
    {
        var category = new ItemCategory("Baby clothing");
        Assert.Equal("Baby clothing", category.Name);
    }

    [Fact]
    public void CT002_CategoryOutsideStandardList_CategoryRejected()
    {
        Assert.Throws<DomainException>(() => new ItemCategory("barang acak"));
    }
}
