using Aetheris.Web.Security;

namespace Aetheris.Tests;

public class PasswordsTests
{
    [Fact]
    public void Hash_then_verify()
    {
        var hash = Passwords.Hash("Clinician#2026");
        Assert.True(Passwords.Verify(hash, "Clinician#2026"));
        Assert.False(Passwords.Verify(hash, "wrong"));
    }
}
