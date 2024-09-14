using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Application;
using Domain_Core;

namespace Infrastructure
{
    public class PostingRepository : IPosting
    {
        private string connectionString;
        List<string> strings = new List<string>();

        IRepository<Posting> repo;
        IRepository<images> repo1;
        public PostingRepository(string ConnectionString, IRepository<Posting> repo, IRepository<images> repo1)
        {
            this.connectionString = ConnectionString;
            this.repo = repo;
            this.repo1 = repo1;
        }

        public void Add(Posting entity)
        {
            repo.Add(entity);
        }
        public int GetId(string userId)
        {
            return repo.GetId(userId);
        }
        public Posting GetById(int id)
        {
            return repo.GetById(id);
        }
        public IEnumerable<Posting> GetAll()
        {
            return repo.GetAll();
        }
        public void Update(Posting entity)
        {
            repo.Update(entity);
        }
        public void Delete(int id)
        {
            repo.Delete(id);
        }
        public string setPath(string path)
        {
            string relativePath = string.Empty;
            if (!(path == string.Empty))
            {
                if (path != null)
                {
                    string rootDirectory = "E:\\VisualStudio\\WebProject\\WebProject\\wwwroot\\";

                    // Remove the root directory part
                    relativePath = path.Replace(rootDirectory, "");

                    // Replace backslashes with forward slashes
                    relativePath = relativePath.Replace("\\", "/");

                }
            }
            return relativePath;
        }
        public async Task<List<messages>> GetMsg(string userId, string receiverId)
        {
            string selectQuery = "SELECT * FROM messages WHERE (SenderId = @userId AND ReceiverId = @receiverId) OR (SenderId = @receiverId AND ReceiverId = @userId) ORDER BY Timestamp";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); // Open the connection asynchronously
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@userId", userId);
                selectCommand.Parameters.AddWithValue("@receiverId", receiverId);

                List<messages> msgs = new List<messages>();
                using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync()) // Execute the command asynchronously
                {
                    while (await reader.ReadAsync()) // Read the data asynchronously
                    {
                        messages msg = new messages();

                        msg.Id = Convert.ToInt32(reader["Id"]);
                        msg.SenderId = Convert.ToString(reader["SenderId"]);
                        msg.ReceiverId = Convert.ToString(reader["ReceiverId"]);
                        msg.Content = Convert.ToString(reader["Content"]);
                        msg.Timestamp = (DateTime)reader["Timestamp"];

                        msgs.Add(msg);
                    }
                }
                return msgs; // Return the list of messages
            }
        }
        public void checkAndAddUserId(string id, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string selectQuery = "SELECT isNewUser FROM AspNetUsers WHERE Id = @userid";
                string insertUserProfileQuery = "INSERT INTO userProfile (userId,userName) VALUES (@id,@userName)";
                string updateAspNetUserQuery = "UPDATE AspNetUsers SET isNewUser=@value WHERE id=@userid";

                connection.Open();

                // Check if the user already exists in AspNetUsers table
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@userid", id);

                bool isNewUser = true; // Assume user is new by default

                using (SqlDataReader sqlDataReader = selectCommand.ExecuteReader())
                {
                    if (sqlDataReader.Read())
                    {
                        isNewUser = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("isNewUser"));
                    }
                }

                // If the user is new, add them to userProfile and update isNewUser flag
                if (!isNewUser)
                {
                    // Add user to userProfile table
                    SqlCommand insertUserProfileCommand = new SqlCommand(insertUserProfileQuery, connection);
                    insertUserProfileCommand.Parameters.AddWithValue("@id", id);
                    insertUserProfileCommand.Parameters.AddWithValue("@userName", userName);
                    insertUserProfileCommand.ExecuteNonQuery();

                    // Update isNewUser flag in AspNetUsers table
                    SqlCommand updateAspNetUserCommand = new SqlCommand(updateAspNetUserQuery, connection);
                    updateAspNetUserCommand.Parameters.AddWithValue("@userid", id);
                    updateAspNetUserCommand.Parameters.AddWithValue("@value", true);
                    updateAspNetUserCommand.ExecuteNonQuery();
                }
            }
        }
        public void deletePost(int Id, string userId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sql1 = "DELETE FROM comment WHERE postId = @id  and userId=@userId";
            string sql2 = "DELETE FROM images WHERE  postId = @id  and userId=@userId";
            //string sql3 = "DELETE FROM posting WHERE postID = @id  and userId=@userId";
            connection.Open();

            //SqlCommand cmd1 = new SqlCommand(sql1, connection);
            //cmd1.Parameters.AddWithValue("id", Id);
            //cmd1.Parameters.AddWithValue("userId", userId);
            //cmd1.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand(sql2, connection);
            cmd2.Parameters.AddWithValue("id", Id);
            cmd2.Parameters.AddWithValue("userId", userId);
            cmd2.ExecuteNonQuery();

            repo.Delete(Id);
            connection.Close();

        }
        public void updatePost(int id, string publication)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string insertQuery = "update posting set publication=@publication where Id=@id";
            connection.Open();

            SqlCommand cmd = new SqlCommand(insertQuery, connection);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("publication", publication);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public List<string> getPostImages(int Id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string selectQuery2 = "select imagePath from images where postId=@id";
            connection.Open();
            SqlCommand cmd2 = new SqlCommand(selectQuery2, connection);
            cmd2.Parameters.AddWithValue("id", Id);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            List<string> images = new List<string>();
            while (reader2.Read())
            {
                images.Add(setPath(Convert.ToString(reader2["imagePath"])));
            }
            reader2.Close();
            connection.Close();
            return images;
        }
        public void add(Posting post, List<string> imagePath)
        {
            if (post != null)
            {
                repo.Add(post);

                List<images> images1 = new List<images>();
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string query = "SELECT IDENT_CURRENT('posting')";
                SqlCommand cmd2 = new SqlCommand(query, connection);
                SqlDataReader reader = cmd2.ExecuteReader();
                if (reader.Read())
                {
                    int lastInsertedId = Convert.ToInt32(reader[0]);
                    reader.Close();

                    if (imagePath.Count > 0)
                    {

                        foreach (var postImage in imagePath)
                        {
                            images img = new images();
                            img.postId = lastInsertedId;
                            img.userId = post.userId;
                            img.imagePath = postImage;

                            repo1.Add(img);

                        }
                    }
                }
                reader.Close();
                connection.Close();

            }

        }
        public string getUserName(string id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string selectQuery = "select fname,lname from AspNetUsers where id=@userid";
            connection.Open();
            string fname = string.Empty, lname = string.Empty;
            SqlCommand cmd = new SqlCommand(selectQuery, connection);
            cmd.Parameters.AddWithValue("userid", id);
            SqlDataReader readData = cmd.ExecuteReader();
            while (readData.Read())
            {
                fname = readData[0].ToString();
                lname = readData[1].ToString();
            }
            readData.Close();
            connection.Close();
            return (fname + " " + lname);
        }
        public List<postANDimage> getAll()
        {

            List<Posting> posts = repo.GetAll().ToList();

            //SqlConnection connection = new SqlConnection(connectionString);
            //string selectQuery = "select * from posting";
            //connection.Open();

            //SqlCommand cmd = new SqlCommand(selectQuery, connection);
            //SqlDataReader readData=  cmd.ExecuteReader();
            ////1st query
            //while (readData.Read())
            //{
            //    Posting post = new Posting();
            //    post.Id = Convert.ToInt32(readData["postID"]);
            //    post.userId = Convert.ToString(readData["userId"]);
            //    post.publication = readData["publication"].ToString();
            //    post.postDate = (DateTime)readData["postDate"];
            //    posts.Add(post);
            //}
            //readData.Close();
            //2nd query
            List<postANDimage> postData = new List<postANDimage>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            foreach (var post in posts)
            {
                postANDimage p = new postANDimage();
                p.posting = post;
                string selectQuery2 = "select imagePath from images where postId=@id";
                string selectQuery3 = "select userName,profilePath from userProfile where userId=@id";
                SqlCommand cmd2 = new SqlCommand(selectQuery2, connection);
                SqlCommand cmd3 = new SqlCommand(selectQuery3, connection);
                cmd2.Parameters.AddWithValue("id", post.Id);
                cmd3.Parameters.AddWithValue("id", post.userId);
                SqlDataReader reader = cmd2.ExecuteReader();
                while (reader.Read())
                {
                    string path = Convert.ToString(reader["imagePath"]);
                    p.imagePath.Add(setPath(path));
                }
                reader.Close();
                SqlDataReader reader2 = cmd3.ExecuteReader();
                while (reader2.Read())
                {
                    p.userName = Convert.ToString(reader2["userName"]);
                    p.profilePath = setPath(Convert.ToString(reader2["profilePath"]));
                }
                postData.Add(p);
                reader2.Close();
            }
            connection.Close();
            return postData;
        }
        public void addUserProfilePictureInDb(string path, string id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string updateImageQuery = "UPDATE userProfile SET profilePath=@imagePath WHERE userId=@id";
            SqlCommand cmd3 = new SqlCommand(updateImageQuery, connection);
            cmd3.Parameters.AddWithValue("@imagePath", path);
            cmd3.Parameters.AddWithValue("@id", id);
            cmd3.ExecuteNonQuery();
            connection.Close();
        }
        public string getUserProfilePictureFromDb(string id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string insertImageQuery = "select profilePath from  userProfile where userId=@id";
            SqlCommand cmd3 = new SqlCommand(insertImageQuery, connection);
            cmd3.Parameters.AddWithValue("id", id);
            SqlDataReader reader = cmd3.ExecuteReader();
            string relativePath = string.Empty;
            while (reader.Read())
            {
                relativePath = setPath(Convert.ToString(reader["profilePath"]));
            }
            connection.Close();
            return relativePath;
        }
    }
}
