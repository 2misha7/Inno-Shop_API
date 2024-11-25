using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Controllers;
using ProductManager.DTO.Requests;
using ProductManager.DTO.Responses;
using ProductManager.Services;
using Moq;

namespace ProductManagerTests;

public class Tests
{
     public class ProductsControllerTests
    {
        private Mock<IProductService> _mockProductService;
        private ProductsController _controller;

        [SetUp]
        public void SetUp()
        {
            // Mock IProductService
            _mockProductService = new Mock<IProductService>();
            // Create the controller with the mocked service
            _controller = new ProductsController(_mockProductService.Object);
        }

        

        [Test]
        public async Task GetProduct_ReturnsBadRequest_WhenProductNotFound()
        {
            // Arrange
            _mockProductService
                .Setup(service => service.GetProductAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("no"));

            // Act
            var result = await _controller.GetProduct(1, CancellationToken.None);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public async Task GetProduct_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var productDto = new ProductDTO
            {
                IdProduct = 1,
                Name = "Test Product",
                Price = 10.5m
            };
            _mockProductService
                .Setup(service => service.GetProductAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            // Act
            var result = await _controller.GetProduct(1, CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as ProductDTO;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(1, returnValue.IdProduct);
        }
        
        
    }
    }
