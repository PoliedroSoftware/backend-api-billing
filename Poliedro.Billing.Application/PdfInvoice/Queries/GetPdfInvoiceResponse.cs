using System;
public record GetPdfInvoiceResponse(Task<byte[]> FileContent, string FileName);