using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Infrastructure.Services
{
    public class JwtSetting
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public int ExpireInMinutes { get; set; } = 60;


    }
}
