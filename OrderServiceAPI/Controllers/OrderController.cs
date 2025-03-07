using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private static List<Order> _orders = new List<Order>();

        public OrderController(ILogger<OrderController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _logger.LogInformation("OrderController initialized.");

            _clientFactory = clientFactory;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            if (order == null)
            {
                _logger.LogError("Order object is null.");
                return BadRequest("Order object is null");
            }

            var validationContext = new ValidationContext(order, null, null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(order, validationContext, validationResults, true);

            if (!isValid)
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage);
                foreach (var error in errors)
                {
                    _logger.LogError($"Validation Error: {error}");
                }
                return BadRequest(errors);
            }

            if (order.Items.Any(item => item.Quantity <= 0))
            {
                _logger.LogError("Item quantities must be greater than zero.");
                return BadRequest("Item quantities must be greater than zero.");
            }

            try
            {
                order.OrderStatus = OrderStatus.processing;
                _logger.LogInformation($"Order {order.OrderId} received and processing.");

                // Simulate saving to a database or similar
                _orders.Add(order);

                // POST to ShippingService
                var client = _clientFactory.CreateClient("ShippingService");
                var response = client.PostAsJsonAsync("api/shipping", order).Result;

                if(response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Order sent to ShippingService successfully.");
                }
                else
                {
                    _logger.LogError("Error sending order to ShippingService.");
                    return StatusCode(500, "Error sending order to ShippingService.");
                }

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(Guid id)
        {
            _logger.LogInformation($"Attempting to retrieve order with ID: {id}");

            // Simulate retrieving an order.
            var foundOrder = _orders.FirstOrDefault(order => order.OrderId == id);
            
            if (foundOrder == null)
            {
                _logger.LogWarning($"Order with ID: {id} not found.");
                return NotFound();
            }

            _logger.LogInformation($"Order with ID: {id} retrieved successfully.");
            return Ok(foundOrder);
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            _logger.LogInformation("Retrieving all orders.");
            return Ok(_orders);
        }
    }
}
