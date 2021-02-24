using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOG_COMM.Class
{
    class FreindAutoJobMoel
    {
        public int work_id { get; set; }
        public String work_title { get; set; }
        public int start_num { get; set; }
        public int end_num { get; set; }
        public DateTime work_start_dtm { get; set; }
        public DateTime work_end_dtm { get; set; }

        public String reply_content { get; set; }
        public bool  empathy { get; set; }

        public int delaytime { get; set; }
        public DateTime reg_dtm { get; set; }

    }

}

