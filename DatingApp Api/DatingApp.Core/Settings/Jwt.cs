using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Settings
{
    public class Jwt
    {
        public string ? Key { get; set; }
        public string ? Issuer { get; set; }
        public string ? Audience { get; set; }
        public short DurationInMinutes { get; set; }
        public short RefreshTokenDurationInDays { get; set; }
    }
}
