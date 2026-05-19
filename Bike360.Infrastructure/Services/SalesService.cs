using Bike360.Application.DTOs.Sales;
using Bike360.Application.Exceptions;
using Bike360.Application.Interfaces;
using Bike360.Domain.Entities;
using Bike360.Infrastructure.Data;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Bike360.Infrastructure.Services
{
    public class SalesService(AppDbContext context, IConfiguration config) : ISalesService
    {
        // FEATURE 2: Create sales invoice
        public async Task<SalesInvoiceResponse> CreateInvoiceAsync(CreateSalesInvoiceRequest request)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var customer = await context.Customers
                    .FirstOrDefaultAsync(c => c.Id == request.CustomerId)
                    ?? throw new NotFoundException($"Customer with ID {request.CustomerId} not found.");

                decimal totalAmount = 0;
                var invoiceItems = new List<SalesInvoiceItem>();

                foreach (var item in request.Items)
                {
                    var part = await context.Parts.FindAsync(item.PartId)
                        ?? throw new NotFoundException($"Part with ID {item.PartId} not found.");

                    if (part.StockQuantity < item.Quantity)
                        throw new BadRequestException(
                            $"Not enough stock for: {part.Name}. Available: {part.StockQuantity}");

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

        // FEATURE 3: Send invoice via email
        public async Task SendInvoiceEmailAsync(int invoiceId)
        {
            var invoice = await context.SalesInvoices
                .Include(i => i.Customer)
                .Include(i => i.Items).ThenInclude(i => i.Part)
                .FirstOrDefaultAsync(i => i.Id == invoiceId)
                ?? throw new NotFoundException($"Invoice not found.");

            // Build email HTML
            var itemRows = string.Join("", invoice.Items.Select(item =>
                $"<tr>" +
                $"<td style='padding:8px;border:1px solid #ddd'>{item.Part.Name}</td>" +
                $"<td style='padding:8px;border:1px solid #ddd'>{item.Quantity}</td>" +
                $"<td style='padding:8px;border:1px solid #ddd'>Rs. {item.UnitPrice:N2}</td>" +
                $"<td style='padding:8px;border:1px solid #ddd'>Rs. {item.SubTotal:N2}</td>" +
                $"</tr>"
            ));

            var discountRow = invoice.DiscountAmount > 0
                ? $"<p><b style='color:green'>Loyalty Discount (10%): - Rs. {invoice.DiscountAmount:N2}</b></p>"
                : "";

            var emailBody = $"""
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px'>
                    <h2 style='color:#2b3053'>Invoice: {invoice.InvoiceNumber}</h2>
                    <p>Dear <b>{invoice.Customer.FullName}</b>,</p>
                    <p>Thank you for your purchase!</p>
                    <table style='width:100%;border-collapse:collapse'>
                        <tr style='background:#2b3053;color:white'>
                            <th style='padding:8px'>Part</th>
                            <th style='padding:8px'>Qty</th>
                            <th style='padding:8px'>Unit Price</th>
                            <th style='padding:8px'>Total</th>
                        </tr>
                        {itemRows}
                    </table>
                    <br/>
                    <p><b>Total: Rs. {invoice.TotalAmount:N2}</b></p>
                    {discountRow}
                    <h3 style='color:#ff751f'>Final Amount: Rs. {invoice.FinalAmount:N2}</h3>
                    <p>Date: {invoice.CreatedAt:dd MMM yyyy}</p>
                    <hr/>
                    <p style='color:gray;font-size:12px'>Vehicle Parts Center</p>
                </div>
            """;

            // Send using MailKit
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(
                config["EmailSettings:FromName"],
                config["EmailSettings:FromEmail"]));
            emailMessage.To.Add(new MailboxAddress(
                invoice.Customer.FullName,
                invoice.Customer.Email));
            emailMessage.Subject = $"Invoice {invoice.InvoiceNumber} - Vehicle Parts Center";
            emailMessage.Body = new TextPart("html") { Text = emailBody };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                config["EmailSettings:SmtpHost"],
                int.Parse(config["EmailSettings:SmtpPort"]!),
                false);
            await smtp.AuthenticateAsync(
                config["EmailSettings:Username"],
                config["EmailSettings:Password"]);
            await smtp.SendAsync(emailMessage);
            await smtp.DisconnectAsync(true);

            // Mark as sent
            invoice.IsEmailSent = true;
            await context.SaveChangesAsync();
        }

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