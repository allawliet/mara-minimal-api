using Microsoft.AspNetCore.Mvc;
using imas.Finance.ApiService.Application.DTOs;
using imas.Finance.ApiService.Application.Interfaces;

namespace imas.Finance.ApiService.Presentation.Endpoints;

public static class FinanceEndpoints
{
    public static void MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoices")
            .WithTags("Invoices")
            .WithOpenApi();

        group.MapGet("/", GetAllInvoices)
             .WithName("GetAllInvoices")
             .WithSummary("Get all invoices");

        group.MapGet("/{id:int}", GetInvoiceById)
             .WithName("GetInvoiceById")
             .WithSummary("Get invoice by ID");

        group.MapPost("/", CreateInvoice)
             .WithName("CreateInvoice")
             .WithSummary("Create a new invoice");

        group.MapDelete("/{id:int}", DeleteInvoice)
             .WithName("DeleteInvoice")
             .WithSummary("Delete an invoice");
    }

    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments")
            .WithTags("Payments")
            .WithOpenApi();

        group.MapGet("/", GetAllPayments)
             .WithName("GetAllPayments")
             .WithSummary("Get all payments");

        group.MapGet("/{id:int}", GetPaymentById)
             .WithName("GetPaymentById")
             .WithSummary("Get payment by ID");

        group.MapPost("/", CreatePayment)
             .WithName("CreatePayment")
             .WithSummary("Create a new payment");

        group.MapPost("/{id:int}/complete", CompletePayment)
             .WithName("CompletePayment")
             .WithSummary("Complete a payment");

        group.MapDelete("/{id:int}", DeletePayment)
             .WithName("DeletePayment")
             .WithSummary("Delete a payment");
    }

    private static async Task<IResult> GetAllInvoices(IInvoiceService invoiceService)
    {
        var result = await invoiceService.GetAllInvoicesAsync();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> GetInvoiceById(int id, IInvoiceService invoiceService)
    {
        var result = await invoiceService.GetInvoiceByIdAsync(id);
        return result.Success ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> CreateInvoice([FromBody] CreateInvoiceDto createInvoiceDto, IInvoiceService invoiceService)
    {
        var result = await invoiceService.CreateInvoiceAsync(createInvoiceDto);
        return result.Success ? Results.Created($"/api/invoices/{result.Data?.Id}", result) : Results.BadRequest(result);
    }

    private static async Task<IResult> DeleteInvoice(int id, IInvoiceService invoiceService)
    {
        var result = await invoiceService.DeleteInvoiceAsync(id);
        return result.Success ? Results.NoContent() : Results.BadRequest(result);
    }

    private static async Task<IResult> GetAllPayments(IPaymentService paymentService)
    {
        var result = await paymentService.GetAllPaymentsAsync();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> GetPaymentById(int id, IPaymentService paymentService)
    {
        var result = await paymentService.GetPaymentByIdAsync(id);
        return result.Success ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> CreatePayment([FromBody] CreatePaymentDto createPaymentDto, IPaymentService paymentService)
    {
        var result = await paymentService.CreatePaymentAsync(createPaymentDto);
        return result.Success ? Results.Created($"/api/payments/{result.Data?.Id}", result) : Results.BadRequest(result);
    }

    private static async Task<IResult> CompletePayment(int id, IPaymentService paymentService)
    {
        var result = await paymentService.CompletePaymentAsync(id);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> DeletePayment(int id, IPaymentService paymentService)
    {
        var result = await paymentService.DeletePaymentAsync(id);
        return result.Success ? Results.NoContent() : Results.BadRequest(result);
    }
}