using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Services.Abstractions;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _lazyUserService;

        public ServiceManager(IRepositoryManager repositoryManager, UserManager<User> userManager)
        {
           _lazyUserService = new  Lazy<IUserService>(() => new UserService(repositoryManager, userManager)); 
        } 
        public IUserService UserService => _lazyUserService.Value;
    }
}
