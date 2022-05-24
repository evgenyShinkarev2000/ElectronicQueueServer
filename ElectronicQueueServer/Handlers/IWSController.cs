using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers
{
    public interface IWSController
    {
        public Task Get();
        public Task Post();
        public Task Update();
        public Task Delete();
        public Task Handle(IEnumerable<string> instructions, object data);
    }
}
