﻿using Azure.Storage.Blobs;
using Contracts;
using Domain.Entities;
using Domain.Storages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Persistence.Repositories.Cached;

namespace Persistence.Repositories
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IRepository<User>> _LazyUserRepo;
        private readonly Lazy<IPostRepository> _LazyPostRepo;
        private readonly Lazy<ICommentRepository> _LazyCommentRepo;
        private readonly Lazy<IUnitOfWork> _LazyUnitOfWork;
        private readonly Lazy<IEmailRepository> _LazyEmailRepo;
        private readonly Lazy<IFileStorage> _LazyFileStorageAzureRepo;
        private readonly Lazy<IRepository<PostLike>> _LazyPostLikeRepo;
        private readonly Lazy<IRepository<UserFollowing>> _LazyUserFollowingRepo;

        public RepositoryManager(
            IConfiguration config, 
            AppDbContext dbContext, 
            BlobServiceClient blobServiceClient, 
            ILoggerManager loggerManager,
            IMemoryCache memoryCache)
        {
            _LazyUserRepo = new Lazy<IRepository<User>>(() => new Repository<User>(dbContext));
            _LazyPostRepo = new Lazy<IPostRepository>(() => new PostRepository(dbContext));
            _LazyCommentRepo = new Lazy<ICommentRepository>(() => new CommentRepository(dbContext));
            _LazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
            _LazyEmailRepo = new Lazy<IEmailRepository>(() => new EmailRepository(config));
            _LazyFileStorageAzureRepo = new Lazy<IFileStorage>(() => new FileStorageAzureRepository(blobServiceClient, loggerManager));
            _LazyPostLikeRepo = new Lazy<IRepository<PostLike>>(() => new Repository<PostLike>(dbContext));
            _LazyUserFollowingRepo = new Lazy<IRepository<UserFollowing>>(() => new Repository<UserFollowing>(dbContext));
        }

        public IRepository<User> UserRepo => _LazyUserRepo.Value;

        public IPostRepository PostRepo => _LazyPostRepo.Value;

        public ICommentRepository CommentRepo => _LazyCommentRepo.Value;

        public IUnitOfWork UnitOfWork => _LazyUnitOfWork.Value;
        public IEmailRepository EmailRepository => _LazyEmailRepo.Value;

        public IFileStorage FileStorage => _LazyFileStorageAzureRepo.Value;

        public IRepository<PostLike> PostLikeRepo => _LazyPostLikeRepo.Value;

        public IRepository<UserFollowing> UserFollowingRepo => _LazyUserFollowingRepo.Value;
    }
}
