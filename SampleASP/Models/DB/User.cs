//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SampleASP.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public System.Guid Userid { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public System.Guid Token { get; set; }
        public bool IsLoggedin { get; set; }
    }
}
