using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain_Core;

namespace Application
{
    //Implement Bussiness Rule / USE CASES
    public class PostingService : IPostingService
    {
        private readonly IPosting posting;
        public PostingService(IPosting posting)
        {
            this.posting = posting;
        }
        public string setPath(string path)
        {
            return posting.setPath(path);
        }
        public async Task<List<Domain_Core.messages>> GetMsg(string userId, string receiverId)
        {
            return await posting.GetMsg(userId, receiverId);
        }
        public void checkAndAddUserId(string id, string userName)
        {
            posting.checkAndAddUserId(id, userName);
        }
        public void deletePost(int Id, string userId)
        {
            posting.deletePost(Id, userId);
        }
        public void updatePost(int id, string publication)
        {
            posting.updatePost(id, publication);
        }
        public List<string> getPostImages(int Id)
        {
            return posting.getPostImages(Id);
        }
        public void add(Posting post, List<string> imagePath)
        {
            posting.add(post, imagePath);
        }
        public List<Domain_Core.postANDimage> getAll()
        {
            return posting.getAll();
        }
        public void addUserProfilePictureInDb(string path, string id)
        {
            posting.addUserProfilePictureInDb(path, id);
        }
        public string getUserProfilePictureFromDb(string id)
        {
            return posting.getUserProfilePictureFromDb(id);
        }
        public string getUserName(string id)
        {
            return posting.getUserName(id);
        }
    }
}
