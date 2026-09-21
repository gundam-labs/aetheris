using Aetheris.Web.Domain;

namespace Aetheris.Tests;

public class MrnTests
{
    [Fact]
    public void Next_pads_sequence()
    {
        Assert.Equal("HV-100001", Mrn.Next("HV", 100001));
    }

    [Fact]
    public void LooksValid_requires_hyphen()
    {
        Assert.True(Mrn.LooksValid("HV-100001"));
        Assert.False(Mrn.LooksValid(""));
        Assert.False(Mrn.LooksValid("norn"));
    }
}
