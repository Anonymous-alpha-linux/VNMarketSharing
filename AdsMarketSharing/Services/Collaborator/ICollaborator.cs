using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdsMarketSharing.Services.Collaborator
{
    public interface ICollaborator
    {
         void RemoveCollaborator();
         void AddCollaborator(Collaborator collaborator);
         void UpdateCollaborator(Collaborator collaborator);
         Task<List<Collaborator>> GetAllCollaborators();
    }
}
