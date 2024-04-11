using Domain.Repositories;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _lazyUserService;

        public ServiceManager(IRepositoryManager repositoryManager)
        {
           _lazyUserService = new  Lazy<IUserService>(() => new UserService(repositoryManager)); 
        } 
        public IUserService UserService => _lazyUserService.Value;
    }
}
