using ECommerce.Api.Search.Interfaces;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService _ordersService;
        private readonly IProductsService _productsService;
        private readonly ICustomersService _customerService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomersService customerService)
        {
            _ordersService = ordersService;
            _productsService = productsService;
            _customerService = customerService;
        }

        public async Task<(bool IsSuccess, dynamic? SearchResults)> SearchAsync(int customerId)
        {
            var ordersResult = await _ordersService.GetOrdersAsync(customerId);
            var productsResult = await _productsService.GetProductsAsync();
            var customersResult = await _customerService.GetCustomerAsync(customerId);

            if (ordersResult.IsSuccess)
            {
                foreach (var order in ordersResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ? productsResult.Products?.FirstOrDefault(product => product.Id == item.ProductId)?.Name : "Product information is not available";
                    }
                }

                var result = new
                {
                    Customer = customersResult.IsSuccess ?
                            customersResult.Customer : new { Name = "Customer information not available" },
                    Orders = ordersResult.Orders
                };

                return (true, result);
            }

            return (false, null);
        }
    }
}
