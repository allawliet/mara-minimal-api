using imas.Finance.ApiService.Application.DTOs;

namespace imas.Finance.ApiService.Application.Interfaces;

public interface IInvoiceService
{
    Task<ApiResponse<IEnumerable<InvoiceDto>>> GetAllInvoicesAsync();
    Task<ApiResponse<InvoiceDto>> GetInvoiceByIdAsync(int id);
    Task<ApiResponse<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto);
    Task<ApiResponse> DeleteInvoiceAsync(int id);
}

public interface IPaymentService
{
    Task<ApiResponse<IEnumerable<PaymentDto>>> GetAllPaymentsAsync();
    Task<ApiResponse<PaymentDto>> GetPaymentByIdAsync(int id);
    Task<ApiResponse<PaymentDto>> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
    Task<ApiResponse> CompletePaymentAsync(int id);
    Task<ApiResponse> DeletePaymentAsync(int id);
}