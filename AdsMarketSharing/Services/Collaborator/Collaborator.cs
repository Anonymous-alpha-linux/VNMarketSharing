using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdsMarketSharing.Services.Collaborator
{
    public class Collaborator : ICollaborator
    {
        public void AddCollaborator(Collaborator collaborator)
        {
            
        }

        public Task<List<Collaborator>> GetAllCollaborators()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveCollaborator()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateCollaborator(Collaborator collaborator)
        {
            throw new System.NotImplementedException();
        }
    }
}
