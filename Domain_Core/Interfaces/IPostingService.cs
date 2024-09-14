using Domain_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IPostingService
    {
        string setPath(string path);
        Task<List<Domain_Core.messages>> GetMsg(string userId, string receiverId);
        void checkAndAddUserId(string id, string userName);
        void deletePost(int Id, string userId);
        void updatePost(int id, string publication);
        List<string> getPostImages(int Id);
        void add(Posting post, List<string> imagePath);
        List<Domain_Core.postANDimage> getAll();
        void addUserProfilePictureInDb(string path, string id);
        string getUserProfilePictureFromDb(string id);
        string getUserName(string id);
    }
}
