using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain_Core;

namespace Application
{
    public interface IUserProfile
    {
        void Add(userProfile UserProfile);
        Domain_Core.userProfile UserDeatils(int Id);

    }
}
