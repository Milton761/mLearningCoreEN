//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MLearningDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string image_url { get; set; }

        public string social_id { get; set; }

        public bool is_online { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

      

        
    }
}

namespace MLearningDBResult
{
    using System;
    using System.Collections.Generic;

    public partial class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string image_url { get; set; }
        public bool is_online { get; set; }

        public int type { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

    }
}
