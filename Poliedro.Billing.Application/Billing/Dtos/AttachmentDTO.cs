namespace Poliedro.Billing.Application.Billing.Dtos;

public record AttachmentDTO
    (
        string FileName,
        string B64Data
    );