using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManager.Repositories;
using UserManager.Controllers;
using UserManager.Repositories;
using UserManager.Services;

namespace UserManagerTests;

public class Tests
{
    private UserService _userService;
    private Mock<IUserRepository> _userRepositoryMock;
    

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        
        _userService = new UserService(_userRepositoryMock.Object, null);
    }

    
    [Test]
    public async Task ActivateUser_UserExists_ActivatesUser()
    {
     
        var userId = 1;
        _userRepositoryMock.Setup(repo => repo.ActivateUser(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        //_productRepositoryMock.Setup(repo => repo.RestoreProductsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _userService.ActivateUser(userId, CancellationToken.None);

        
        _userRepositoryMock.Verify(repo => repo.ActivateUser(userId, It.IsAny<CancellationToken>()), Times.Once);
        //_productRepositoryMock.Verify(repo => repo.RestoreProductsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void ActivateUser_UserNotExists_ThrowsException()
    {
   
        var userId = 999;
        _userRepositoryMock.Setup(repo => repo.ActivateUser(It.IsAny<int>(), It.IsAny<CancellationToken>())).Throws(new Exception("User not found"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _userService.ActivateUser(userId, CancellationToken.None));
    }
    [Test]
    public async Task DeactivateUser_UserExists_DeactivatesUser()
    {
   
        var userId = 1;
        _userRepositoryMock.Setup(repo => repo.DeactivateUser(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        //_productRepositoryMock.Setup(repo => repo.SoftDeleteProductsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

    
        await _userService.DeactivateUser(userId, CancellationToken.None);

        _userRepositoryMock.Verify(repo => repo.DeactivateUser(userId, It.IsAny<CancellationToken>()), Times.Once);
        //_productRepositoryMock.Verify(repo => repo.SoftDeleteProductsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void DeactivateUser_UserNotExists_ThrowsException()
    {
        var userId = 999;
        _userRepositoryMock.Setup(repo => repo.DeactivateUser(It.IsAny<int>(), It.IsAny<CancellationToken>())).Throws(new Exception("User not found"));

 
        Assert.ThrowsAsync<Exception>(async () => await _userService.DeactivateUser(userId, CancellationToken.None));
    }

   
}