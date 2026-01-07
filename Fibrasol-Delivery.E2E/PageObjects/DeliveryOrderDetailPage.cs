using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.PageObjects;

public class DeliveryOrderDetailPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Main form selectors
    private const string OrderIdInput = "#orderId";
    private const string StatusSelect = "#orderStatus";
    private const string TotalInput = "#orderTotal";
    private const string RidersSelect = "#orderRiders";
    private const string AddBackorderButton = "button:has-text('Agregar Comanda')";
    private const string SaveButton = ".btn-primary-custom";
    private const string CancelButton = ".btn-secondary";

    // Backorder selectors
    private const string BackorderCards = ".backorder-card";
    private const string ClientSearch = ".backorder-client";
    private const string ClientDropdown = ".client-dropdown";
    private const string AddClientButton = ".input-group .btn-outline-primary";
    private const string BackorderNumber = ".backorder-command";
    private const string BackorderWeight = ".backorder-weight";
    private const string AddInvoiceButton = ".factura-section .btn-outline-primary";  // CSS selector for QuerySelectorAsync
    private const string DeleteBackorderButton = ".btn-sm.btn-outline-danger";  // btn-sm distinguishes from invoice delete

    // Invoice selectors
    private const string InvoiceCards = ".factura-card";
    private const string InvoiceAddress = ".factura-direccion";
    private const string InvoiceReference = ".factura-reference";
    private const string InvoiceValue = ".factura-value";
    private const string InvoiceSalesPerson = ".factura-salesperson";
    private const string InvoiceCurrency = ".factura-currency";
    private const string InvoiceFile = "input[type='file']";
    private const string DeleteInvoiceButton = ".btn-sm-custom.btn-outline-danger";  // btn-sm-custom is unique to invoice

    // Modal selectors
    private const string ClientModal = "#createClientModal";
    private const string ClientModalNameInput = "#newClientName";
    private const string ClientModalSaveButton = "#createClientModal .btn-primary-custom";

    public DeliveryOrderDetailPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateAsync(int orderId = 0)
    {
        await _page.GotoAsync($"{_baseUrl}/constancias/{orderId}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task WaitForPageLoadAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await _page.WaitForTimeoutAsync(1000); // Wait for dynamic content
    }

    // Main form methods
    public async Task SelectStatusAsync(string statusText)
    {
        await _page.SelectOptionAsync(StatusSelect, new SelectOptionValue { Label = statusText });
    }

    public async Task<string?> GetTotalAsync()
    {
        return await _page.InputValueAsync(TotalInput);
    }

    public async Task SelectRidersAsync(params string[] riderNames)
    {
        foreach (var name in riderNames)
        {
            await _page.SelectOptionAsync(RidersSelect, new SelectOptionValue { Label = name });
        }
    }

    public async Task ClickSaveAsync()
    {
        await _page.ClickAsync(SaveButton);
    }

    public async Task ClickCancelAsync()
    {
        await _page.ClickAsync(CancelButton);
    }

    // Backorder methods
    public async Task ClickAddBackorderAsync()
    {
        await _page.ClickAsync(AddBackorderButton);
        await _page.WaitForTimeoutAsync(500);
    }

    public async Task<int> GetBackorderCountAsync()
    {
        var cards = await _page.QuerySelectorAllAsync(BackorderCards);
        return cards.Count;
    }

    public async Task FillBackorderAsync(int backorderIndex, string clientName, string number, string weight)
    {
        var backorders = await _page.QuerySelectorAllAsync(BackorderCards);
        if (backorders.Count > backorderIndex)
        {
            var backorder = backorders[backorderIndex];

            // Search and select client
            var clientInput = await backorder.QuerySelectorAsync(ClientSearch);
            if (clientInput != null)
            {
                await clientInput.FillAsync(clientName);
                await _page.WaitForTimeoutAsync(500);
                // Click on dropdown item
                await _page.ClickAsync($".dropdown-item:has-text('{clientName}')");
            }

            // Fill number
            var numberInput = await backorder.QuerySelectorAsync(BackorderNumber);
            if (numberInput != null)
            {
                await numberInput.FillAsync(number);
            }

            // Fill weight
            var weightInput = await backorder.QuerySelectorAsync(BackorderWeight);
            if (weightInput != null)
            {
                await weightInput.FillAsync(weight);
            }
        }
    }

    public async Task ClickAddInvoiceAsync(int backorderIndex = 0)
    {
        var backorders = await _page.QuerySelectorAllAsync(BackorderCards);
        if (backorders.Count > backorderIndex)
        {
            var addInvoiceBtn = await backorders[backorderIndex].QuerySelectorAsync(AddInvoiceButton);
            if (addInvoiceBtn != null)
            {
                await addInvoiceBtn.ClickAsync();
                await _page.WaitForTimeoutAsync(500);
            }
        }
    }

    public async Task DeleteBackorderAsync(int backorderIndex = 0)
    {
        var backorders = await _page.QuerySelectorAllAsync(BackorderCards);
        if (backorders.Count > backorderIndex)
        {
            var deleteBtn = await backorders[backorderIndex].QuerySelectorAsync(DeleteBackorderButton);
            if (deleteBtn != null)
            {
                await deleteBtn.ClickAsync();
            }
        }
    }

    // Invoice methods
    public async Task<int> GetInvoiceCountAsync()
    {
        var cards = await _page.QuerySelectorAllAsync(InvoiceCards);
        return cards.Count;
    }

    public async Task FillInvoiceAsync(int invoiceIndex, string address, string reference, string value, string? salesPersonName = null)
    {
        var invoices = await _page.QuerySelectorAllAsync(InvoiceCards);
        if (invoices.Count > invoiceIndex)
        {
            var invoice = invoices[invoiceIndex];

            var addressInput = await invoice.QuerySelectorAsync(InvoiceAddress);
            if (addressInput != null) await addressInput.FillAsync(address);

            var refInput = await invoice.QuerySelectorAsync(InvoiceReference);
            if (refInput != null) await refInput.FillAsync(reference);

            var valueInput = await invoice.QuerySelectorAsync(InvoiceValue);
            if (valueInput != null) await valueInput.FillAsync(value);

            if (salesPersonName != null)
            {
                var spSelect = await invoice.QuerySelectorAsync(InvoiceSalesPerson);
                if (spSelect != null)
                {
                    await spSelect.SelectOptionAsync(new SelectOptionValue { Label = salesPersonName });
                }
            }
        }
    }

    public async Task SelectInvoiceCurrencyAsync(int invoiceIndex, string currency)
    {
        var invoices = await _page.QuerySelectorAllAsync(InvoiceCards);
        if (invoices.Count > invoiceIndex)
        {
            var currencySelect = await invoices[invoiceIndex].QuerySelectorAsync(InvoiceCurrency);
            if (currencySelect != null)
            {
                await currencySelect.SelectOptionAsync(new SelectOptionValue { Label = currency });
            }
        }
    }

    public async Task DeleteInvoiceAsync(int invoiceIndex = 0)
    {
        var invoices = await _page.QuerySelectorAllAsync(InvoiceCards);
        if (invoices.Count > invoiceIndex)
        {
            var deleteBtn = await invoices[invoiceIndex].QuerySelectorAsync(DeleteInvoiceButton);
            if (deleteBtn != null)
            {
                await deleteBtn.ClickAsync();
            }
        }
    }

    // Client modal methods
    public async Task ClickAddClientButtonAsync()
    {
        await _page.ClickAsync(AddClientButton);
        await _page.WaitForSelectorAsync($"{ClientModal}.show");
    }

    public async Task CreateClientInModalAsync(string name)
    {
        await _page.FillAsync(ClientModalNameInput, name);
        await _page.ClickAsync(ClientModalSaveButton);
        await _page.WaitForSelectorAsync($"{ClientModal}.show", new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden });
    }

    // Helper methods
    public async Task<bool> IsPageLoadedAsync()
    {
        var statusSelect = await _page.QuerySelectorAsync(StatusSelect);
        return statusSelect != null;
    }

    public async Task<string?> GetSelectedStatusAsync()
    {
        return await _page.InputValueAsync(StatusSelect);
    }

    public async Task<int> GetSelectedRidersCountAsync()
    {
        var options = await _page.QuerySelectorAllAsync($"{RidersSelect} option:checked");
        return options.Count;
    }
}
