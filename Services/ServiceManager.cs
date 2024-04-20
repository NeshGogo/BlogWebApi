using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Abstractions;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _lazyUserService;

        public ServiceManager(IRepositoryManager repositoryManager, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
           _lazyUserService = new  Lazy<IUserService>(() => new UserService(repositoryManager, userManager, signInManager, configuration)); 
        } 
        public IUserService UserService => _lazyUserService.Value;
    }
}
