using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadsideAssistanceBusiness.Interfaces
{
    public interface IEntityFactory: IDisposable        
    {

        public ERAEntities.ERAContext CreateDbConext(string connectionString);
    }
}
