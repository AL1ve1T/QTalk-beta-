using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using System.Linq;
using System.Web;

namespace VoiceChatService.Models
{
    public class VoiceChatServiceContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public VoiceChatServiceContext() : base("name=VoiceChatServiceContext")
        {
        }

        public static VoiceChatServiceContext Create()
        {
            return new VoiceChatServiceContext();
        }

        public System.Data.Entity.DbSet<VoiceChatService.Models.UserDataModel> UserDataModels { get; set; }

        public System.Data.Entity.DbSet<VoiceChatService.Models.ChannelDataModel> ChannelDataModels { get; set; }
    }

    public interface IUserDataRepository
    {
        IEnumerable<UserDataModel> GetAll();
        void Add(UserDataModel userDataModel);
        Task<UserDataModel> FindAsync(string _id);
    }

    public interface IChannelDataRepository
    {
        IEnumerable<ChannelDataModel> GetAll();
        void Add(ChannelDataModel channelDataModel);
        Task<ChannelDataModel> FindAsync(string _id);
    }

    /// <summary>
    /// @author: AL1ve1T
    /// Represents repository for UserDataModels
    /// </summary>
    public class UserDataRepository : IUserDataRepository, IDisposable
    {
        private VoiceChatServiceContext db = new VoiceChatServiceContext();

        public IEnumerable<UserDataModel> GetAll()
        {
            return db.UserDataModels;
        }

        public void Add(UserDataModel userDataModel)
        {
            db.UserDataModels.Add(userDataModel);
            db.SaveChanges();
        }

        public async Task<UserDataModel> FindAsync(string _id)
        {
            return await db.UserDataModels.FindAsync(_id);
        }

        public UserDataModel FindByName(string _name)
        {
            return db.UserDataModels.FirstOrDefault(model => model.UserName == _name);
        }

        public void UpdateVisitedChannels(string _name, string _channelName)
        {
            UserDataModel toUpdate = db.UserDataModels.Where(u => u.UserName == _name).FirstOrDefault();

            if (toUpdate != null)
            {
                toUpdate.RecenltyVisitedRooms += _channelName + ',';
                db.SaveChanges();
            }
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// @author: AL1ve1T
    /// Represents repository for ChannelDataModels
    /// </summary>
    public class ChannelDataRepository : IChannelDataRepository, IDisposable
    {
        VoiceChatServiceContext db = new VoiceChatServiceContext();

        public IEnumerable<ChannelDataModel> GetAll()
        {
            return db.ChannelDataModels;
        }

        public void Add(ChannelDataModel channelDataModel)
        {
            db.ChannelDataModels.Add(channelDataModel);
            db.SaveChanges();
        }

        public async Task<ChannelDataModel> FindAsync(string _name)
        {
            return await db.ChannelDataModels.Where(u => u.Name == _name).FirstOrDefaultAsync();
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
