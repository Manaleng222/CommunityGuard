using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityGuard.Shared.Dtos
{
   
        // Data sent BY the MAUI client to log in.
        public record LoginRequest(string Email, string Password);

        // Data sent BACK to the MAUI client after logging in.
        public record LoginResponse(string Token, string Email, string Role);
    
}
