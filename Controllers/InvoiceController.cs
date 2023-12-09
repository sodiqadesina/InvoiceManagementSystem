using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _service;

        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index(int customerId, string alphaFilter)
        {
            var customer = _service.GetCustomerById(customerId);
            if (customer == null)
            {
                // Handling the null case, e.g., return a NotFound view
                return NotFound();
            }

            var invoices = _service.GetInvoicesForCustomer(customerId).ToList();
            var invoiceViewModels = new List<InvoiceViewModel>();
            var paymentTermsList = new List<PaymentTermsViewModel>();

            foreach (var invoice in invoices)
            {
                var paymentTerms = _service.GetPaymentTermsByInvoiceId(invoice.InvoiceId);
                var invoiceViewModel = new InvoiceViewModel
                {
                    InvoiceId = invoice.InvoiceId,
                    DueDate = invoice.InvoiceDate?.AddDays(paymentTerms.DueDays) ?? DateTime.MinValue,
                    AmountPaid = (decimal)(invoice.PaymentTotal ?? 0),
                    PaymentTermsDescription = paymentTerms.Description,
                    PaymentTermsId = paymentTerms.PaymentTermsId
                };

                // Adding payment terms to the list if not already included
                if (!paymentTermsList.Any(pt => pt.PaymentTermsId == paymentTerms.PaymentTermsId))
                {
                    paymentTermsList.Add(new PaymentTermsViewModel
                    {
                        PaymentTermsId = paymentTerms.PaymentTermsId,
                        Description = paymentTerms.Description,
                        DueDays = paymentTerms.DueDays
                    });
                }

                invoiceViewModels.Add(invoiceViewModel);
            }

            int? selectedInvoiceId = invoices.Any() ? invoices.First().InvoiceId : (int?)null;
            var selectedInvoiceLineItems = selectedInvoiceId.HasValue
                ? _service.GetLineItemsByInvoiceId(selectedInvoiceId.Value)
                    .Select(li => new InvoiceLineItemViewModel
                    {
                        Description = li.Description,
                        Amount = (decimal)li.Amount
                    }).ToList()
                : new List<InvoiceLineItemViewModel>();

            var viewModel = new CustomerInvoicesViewModel
            {
                CompanyName = customer.Name,
                CompanyAddress = customer.Address1 + ", " + customer.City,
                CompanyCity = customer.City,
                PaymentTerms = paymentTermsList, // This is now a list
                SelectedInvoiceId = selectedInvoiceId,
                Invoices = invoiceViewModels,
                SelectedInvoiceLineItems = selectedInvoiceLineItems
            };

            // Add the current filter to the ViewBag so it can be used in the View
            ViewBag.CurrentFilter = alphaFilter;

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> GetLineItems(int invoiceId)
        {
            var lineItems = _service.GetLineItemsByInvoiceId(invoiceId)
                .Select(li => new InvoiceLineItemViewModel
                {
                    Description = li.Description,
                    Amount = (decimal)li.Amount
                }).ToList();

            var viewModel = new CustomerInvoicesViewModel
            {
                SelectedInvoiceId = invoiceId,
                SelectedInvoiceLineItems = lineItems.Select(li => new InvoiceLineItemViewModel
                {
                    Description = li.Description,
                    Amount = (decimal)li.Amount
                }).ToList()
            };

            return PartialView("_InvoiceLineItems", viewModel);
        }





    }
}
