using ERAEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RoadsideAssistanceBusiness.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadsideAssistanceBusiness.Services
{
    public class EntityFactory : IEntityFactory, IDisposable
    {
        private ERAContext _context;

        public ERAContext CreateDbConext(string connectionString)
        {
            DbContextOptions<ERAContext> options = new DbContextOptionsBuilder<ERAContext>().UseSqlServer(connectionString, x => x.UseNetTopologySuite()).Options;
            _context = new ERAContext(options);
            DBInitializer.Init(_context);

            return _context;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing) 
        { 
            _context?.Dispose();
        }
    }
}
