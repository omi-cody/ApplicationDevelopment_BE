using Bike360.Application.DTOs.Sales;
using Bike360.Application.Exceptions;
using Bike360.Application.Interfaces;
using Bike360.Domain.Entities;
using Bike360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Bike360.Infrastructure.Services
{
    public class SalesService(AppDbContext context, IConfiguration config) : ISalesService
    {
        // FEATURE 2: Create sales invoice
        public async Task<SalesInvoiceResponse> CreateInvoiceAsync(CreateSalesInvoiceRequest request)
        {
            // Transaction - if anything fails, undo everything
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Check customer exists
                var customer = await context.Customers
                    .FirstOrDefaultAsync(c => c.Id == request.CustomerId)
                    ?? throw new NotFoundException($"Customer with ID {request.CustomerId} not found.");

                decimal totalAmount = 0;
                var invoiceItems = new List<SalesInvoiceItem>();

                foreach (var item in request.Items)
                {
                    // Get part
                    var part = await context.Parts.FindAsync(item.PartId)
                        ?? throw new NotFoundException($"Part with ID {item.PartId} not found.");

                    // Check stock
                    if (part.StockQuantity < item.Quantity)
                        throw new BadRequestException(
                            $"Not enough stock for: {part.Name}. Available: {part.StockQuantity}");

                    // Reduce stock
                    part.StockQuantity -= item.Quantity;

                    var subTotal = part.Price * item.Quantity;
                    totalAmount += subTotal;

                    invoiceItems.Add(new SalesInvoiceItem
                    {
                        PartId = item.PartId,
                        Quantity = item.Quantity,
                        UnitPrice = part.Price,
                        SubTotal = subTotal
                    });
                }

                // Loyalty discount: 10% if total > Rs. 5000
                decimal discountAmount = totalAmount > 5000 ? totalAmount * 0.10m : 0;

                // Create invoice
                var invoice = new SalesInvoice
                {
                    InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    CustomerId = request.CustomerId,
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    FinalAmount = totalAmount - discountAmount,
                    Items = invoiceItems,
                    CreatedAt = DateTime.UtcNow
                };

                context.SalesInvoices.Add(invoice);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await BuildInvoiceResponse(invoice.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // FEATURE 3: Send invoice email (will implement fully in Feature 3)
        public async Task SendInvoiceEmailAsync(int invoiceId)
        {
            throw new NotImplementedException("Will be implemented in Feature 3.");
        }

        // Helper to build response
        private async Task<SalesInvoiceResponse> BuildInvoiceResponse(int invoiceId)
        {
            var invoice = await context.SalesInvoices
                .Include(i => i.Customer)
                .Include(i => i.Items).ThenInclude(i => i.Part)
                .FirstAsync(i => i.Id == invoiceId);

            return new SalesInvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = invoice.Customer.FullName,
                CustomerEmail = invoice.Customer.Email,
                TotalAmount = invoice.TotalAmount,
                DiscountAmount = invoice.DiscountAmount,
                FinalAmount = invoice.FinalAmount,
                IsEmailSent = invoice.IsEmailSent,
                CreatedAt = invoice.CreatedAt,
                Items = invoice.Items.Select(i => new InvoiceItemResponse
                {
                    PartName = i.Part.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SubTotal = i.SubTotal
                }).ToList()
            };
        }
    }
}