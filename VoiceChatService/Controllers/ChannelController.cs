using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using VoiceChatService.Models;
using VoiceChatService.Models.VoiceServer;
using System.Threading;

namespace VoiceChatService.Controllers
{
    [Authorize]
    [RoutePrefix("api/Channel")]
    public class ChannelController : ApiController
    {
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create(CreateChannelBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ChannelDataRepository repository = new ChannelDataRepository();

            if (await repository.FindAsync(model.Name) == null)
            {
                ChannelDataModel channelDataModel = new ChannelDataModel();

                channelDataModel.Id = model.Id;
                channelDataModel.Name = model.Name;

                repository.Add(channelDataModel);

                using (UserDataRepository db = new UserDataRepository())
                {
                    db.UpdateVisitedChannels(model.Owner, model.Name);
                }

                return Ok();
            }

            return GetErrorResult(new IdentityResult("Choose another ID"));
        }

        [HttpPost]
        [Route("Join")]
        public async Task<IHttpActionResult> Join(JoinChannelBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ChannelDataRepository repository = new ChannelDataRepository();

            var channelModel = await repository.FindAsync(model.ChannelName);

            if (channelModel == null)
            {
                return GetErrorResult(new IdentityResult("Channel not exists"));
            }

            var channel = ChannelsInfo.GetChannelInfo(channelModel.Name);
            
            if (channel != null)
            {
                channel.Members.Add(model.User);
                return Ok(Convert.ToInt32(channel.Port));
            }

            int initPort = 5000;
            VoiceServer newVoiceServer = new VoiceServer();
            while (initPort < 65000)
            {
                try
                {
                    newVoiceServer.Start(initPort);

                    ChannelsInfo.ListOfSockets.Add(initPort.ToString(), newVoiceServer);
                    ChannelsInfo.AddChannel(channelModel, initPort.ToString());
                    ChannelsInfo.AddMember(channelModel.Name, model.User);
                    break;
                }
                catch (Exception e)
                {
                    ChannelsInfo.ListOfSockets.Remove(initPort.ToString());
                    initPort++;
                }
            }

            using (UserDataRepository db = new UserDataRepository())
            {
                var user = db.FindByName(model.User);
                if (!user.RecenltyVisitedRooms.Split(',').Contains(model.ChannelName))
                {
                    db.UpdateVisitedChannels(model.User, model.ChannelName);
                }
            }

            return Ok(initPort);
        }

        public async Task<IHttpActionResult> Leave(LeaveChannelBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ChannelsInfo.RemoveMember(model.ChannelName, model.User);

            return Ok();
        }
    }
}