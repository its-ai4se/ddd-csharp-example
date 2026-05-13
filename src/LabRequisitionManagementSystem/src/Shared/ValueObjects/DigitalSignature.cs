using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class DigitalSignature : ValueObject
{
    public byte[] SignatureData { get; }
    public string FileName { get; }
    public string ContentType { get; }

    public DigitalSignature(byte[] signatureData, string fileName, string contentType)
    {
        if (signatureData == null || signatureData.Length == 0)
        {
            throw new ArgumentException("Signature data cannot be null or empty.", nameof(signatureData));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be empty or whitespace.", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException("Content type cannot be empty or whitespace.", nameof(contentType));
        }

        SignatureData = signatureData;
        FileName = fileName.Trim();
        ContentType = contentType.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SignatureData;
        yield return FileName;
        yield return ContentType;
    }
}
