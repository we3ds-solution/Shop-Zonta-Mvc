using market_mvc.Domain.Enums;
using market_mvc.Features.Auth.Totp;
using market_mvc.Features.Orders;

namespace Zonta.Tests;

public class TotpServiceTests
{
    // RFC 6238 Appendix B, SHA-1 vectors (secret ASCII "12345678901234567890").
    // Public test vector — not a production secret.
    private const string RfcSecret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ"; // gitleaks:allow

    [Theory]
    [InlineData(59, "287082")]
    [InlineData(1111111109, "081804")]
    [InlineData(1234567890, "005924")]
    public void ComputeCode_Matches_Rfc6238_Vectors(long unixSeconds, string expected)
    {
        Assert.Equal(expected, TotpService.ComputeCode(RfcSecret, DateTimeOffset.FromUnixTimeSeconds(unixSeconds)));
    }

    [Fact]
    public void VerifyCode_Accepts_Current_And_Rejects_Wrong()
    {
        var now = DateTimeOffset.UtcNow;
        var code = TotpService.ComputeCode(RfcSecret, now);
        Assert.True(TotpService.VerifyCode(RfcSecret, code, now));
        Assert.False(TotpService.VerifyCode(RfcSecret, "000000", now));
        Assert.False(TotpService.VerifyCode(RfcSecret, "abcdef", now));
        Assert.False(TotpService.VerifyCode(RfcSecret, "", now));
    }

    [Fact]
    public void GenerateSecret_Is_Base32_And_Unique()
    {
        var a = TotpService.GenerateSecret();
        var b = TotpService.GenerateSecret();
        Assert.NotEqual(a, b);
        Assert.Matches("^[A-Z2-7]+$", a);
    }

    [Fact]
    public void BuildKeyUri_Contains_Otpauth_Parts()
    {
        var uri = TotpService.BuildKeyUri("a@example.com", "Zonta", "ABCDEF");
        Assert.StartsWith("otpauth://totp/", uri);
        Assert.Contains("secret=ABCDEF", uri);
    }
}

public class OrderStatusTransitionTests
{
    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.Confirmed, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Delivered, false)]
    [InlineData(OrderStatus.Processing, OrderStatus.Shipped, true)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Cancelled, false)]
    [InlineData(OrderStatus.Delivered, OrderStatus.Refunded, false)]
    public void AllowedFrom_Enforces_Legal_Transitions(OrderStatus from, OrderStatus to, bool allowed)
    {
        Assert.Equal(allowed, OrderStatusTransitions.AllowedFrom(from).Contains(to));
    }
}
