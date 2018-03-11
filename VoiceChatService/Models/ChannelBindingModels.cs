using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VoiceChatService.Models
{

    /// <summary>
    /// @author: AL1ve1T
    /// </summary>
    public class CreateChannelBindingModel
    {
        [Required]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [Required]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

    public class JoinChannelBindingModel
    {
        [Required]
        [Display(Name = "User")]
        public string User { get; set; }

        [Required]
        [Display(Name ="ChannelName")]
        public string ChannelName { get; set; }
    }

    public class LeaveChannelBindingModel
    {
        [Required]
        [Display(Name = "User")]
        public string User { get; set; }

        [Required]
        [Display(Name = "ChannelName")]
        public string ChannelName { get; set; }
    }
}