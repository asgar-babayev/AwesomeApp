﻿using System.ComponentModel.DataAnnotations;

namespace AwesomeAppBack.Areas.Manage.ViewModels
{
    public class RegisterVm
    {
        [Required]
        public string UserName { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
