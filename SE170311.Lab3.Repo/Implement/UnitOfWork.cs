using SE170311.Lab3.Repo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE170311.Lab3.Repo.Implement
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly Lab3Context context;
        private IGenericRepository<Account> accountRepository;
        private IGenericRepository<Role> roleRepository;
        private IGenericRepository<Product> productRepository;
        private IGenericRepository<Category> categoryRepository;

        public UnitOfWork(Lab3Context context)
        {
            this.context = context;
        }

        public IGenericRepository<Account> AccountRepository
        {
            get
            {
                return accountRepository ??= new GenericRepository<Account>(context);
            }
        }

        public IGenericRepository<Role> RoleRepository
        {
            get
            {
                return roleRepository ??= new GenericRepository<Role>(context);
            }
        }

        public IGenericRepository<Product> ProductRepository
        {
            get
            {
                return productRepository ??= new GenericRepository<Product>(context);
            }
        }

        public IGenericRepository<Category> CategoryRepository
        {
            get
            {
                return categoryRepository ??= new GenericRepository<Category>(context);
            }
        }

        public void Save()
        {
            var validationErrors = context.ChangeTracker.Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(e => e != ValidationResult.Success)
                .ToArray();
            if (validationErrors.Any())
            {
                var exceptionMessage = string.Join(Environment.NewLine,
                    validationErrors.Select(error => $"Properties {error.MemberNames} Error: {error.ErrorMessage}"));
                throw new Exception(exceptionMessage);
            }
            context.SaveChanges();
        }

        async public Task SaveAsync()
        {
            var validationErrors = context.ChangeTracker.Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(e => e != ValidationResult.Success)
                .ToArray();
            if (validationErrors.Any())
            {
                var exceptionMessage = string.Join(Environment.NewLine,
                    validationErrors.Select(error => $"Properties {error.MemberNames} Error: {error.ErrorMessage}"));
                throw new Exception(exceptionMessage);
            }
            await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
