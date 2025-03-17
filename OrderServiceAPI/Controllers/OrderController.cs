using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace OrderServiceAPI.Controllers
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

            _clientFactory = clientFactory;

            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddresses(hostName);
            var _ipaddr = ips.First().MapToIPv4().ToString();

            _logger.LogInformation(1, $"Order Service running on IP: {_ipaddr}");
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            if (order == null)
            {
                var msg = "Order object is null.";
                _logger.LogError(msg);
                return BadRequest(msg);
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
                var msg = "Item quantities must be greater than zero.";
                _logger.LogWarning(msg);
                return BadRequest(msg);
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
                    var msg = "Error sending order to ShippingService.";
                    _logger.LogError(msg);
                    return StatusCode(500, msg);
                }

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                var msg = "Error processing order.";
                _logger.LogError(ex, msg);                
                return StatusCode(500, msg);
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

        [HttpGet("version")]
        public async Task<Dictionary<string, string>> GetVersion()
        {
            var properties = new Dictionary<string, string>
            {
                { "service", "HaaV Order Service" }
            };
            
            var ver = FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).ProductVersion ?? "Unknown";
            properties.Add("version", ver!);

            try
            {
                var hostName = System.Net.Dns.GetHostName();
                var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
                var ipa = ips.First().MapToIPv4().ToString();
                properties.Add("hosted-at-address", ipa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                properties.Add("hosted-at-address", "Could not resolve IP-address");
            }
            return properties;
        }
    }
}
