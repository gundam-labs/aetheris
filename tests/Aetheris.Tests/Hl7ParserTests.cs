using Aetheris.Web.Interop.Hl7;

namespace Aetheris.Tests;

public class Hl7ParserTests
{
    [Fact]
    public void Parses_MSH_and_PID_from_blood_gas_ORU()
    {
        var msg = Hl7Parser.Parse(Hl7Samples.BloodGasOru());
        Assert.Equal("ORU^R01", msg.MessageType);
        Assert.Equal("ABL90", msg.SendingApplication);
        Assert.Equal("HV-100004", msg.First("PID")!.Field(3));
    }

    [Fact]
    public void Reads_OBX_analytes()
    {
        var obx = Hl7Parser.Parse(Hl7Samples.BloodGasOru()).Of("OBX").ToList();
        Assert.Equal(5, obx.Count);
        Assert.Equal("2744-1", obx[0].Component(3, 1));
        Assert.Equal("7.11", obx[0].Field(5));
    }

    [Fact]
    public void Unwraps_MLLP_envelope()
    {
        var msg = Hl7Parser.Parse(Hl7Samples.WrappedBloodGas());
        Assert.Equal("ABL90", msg.SendingApplication);
    }

    [Fact]
    public void Rejects_payload_without_MSH()
    {
        Assert.Throws<FormatException>(() => Hl7Parser.Parse("PID|||HV-1"));
    }
}
