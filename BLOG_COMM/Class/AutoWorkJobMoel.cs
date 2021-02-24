using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOG_COMM.Class
{
    class AutoWorkJobMoel
    {
        public int work_id { get; set; }
        public String work_title { get; set; }
        public String work_desc { get; set; }
        public String work_type { get; set; }
        public DateTime reg_dtm { get; set; }
        public int total_cnt { get; set; }
        public int process_cnt { get; set; }
        public int success_cnt { get; set; }
        public int fail_cnt { get; set; }
        public DateTime work_start_dtm { get; set; }
        public DateTime work_end_dtm { get; set; }

        public int target_cnt { get; set; }

        public String reply_content { get; set; }
        public bool  empathy { get; set; }

        public int delaytime { get; set; }

    }

}

