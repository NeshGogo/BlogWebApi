using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Abstractions;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _lazyUserService;
        private readonly Lazy<IPostService> _lazyPostService;

        public ServiceManager(
            IRepositoryManager repositoryManager, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
           _lazyUserService = new  Lazy<IUserService>(() => new UserService(httpContextAccessor, repositoryManager,
               userManager, signInManager, configuration)); 
           _lazyPostService = new Lazy<IPostService>(() => new PostService(httpContextAccessor, repositoryManager));
        } 

        public IUserService UserService => _lazyUserService.Value;
        public IPostService PostService => _lazyPostService.Value;
    }
}
