using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using BLOG_COMM.Class;

namespace BLOG_COMM
{
     class DbUtil
    {
        private  static OleDbConnection conn = null;

        private static OleDbCommand cmd;
        private static OleDbDataAdapter da;
        private static DataSet ds;
        private static DataTable dt;
        public static string ConnectionString= @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source = marbling.accdb";

        public   DbUtil()
        {
            //SQL DB와 연결
            DBinit();
        }
        public static void DbClose()
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn = null;

            }
        }
        private static void DBinit()
        {
            if (conn == null)
            {
                conn = new OleDbConnection();
                conn.ConnectionString = ConnectionString;
                Common.log.Debug(conn.ConnectionString);
            }

            if(conn.State==ConnectionState.Closed) conn.Open();
             
             cmd = new OleDbCommand();
             cmd.Connection = conn;

        }

        // 한글 UTF8 변환
        private  static string str2UTF8(String orginString)
        {
            byte[] bytes = Encoding.Default.GetBytes(orginString);


            String resultString = Encoding.UTF8.GetString(bytes);
            return resultString;

        }
        // 네이버친구  start 
        public static void GridviewLoad(DataGridView dgFriendsList)
        {
            DBinit();
            dgFriendsList.SelectionMode = DataGridViewSelectionMode.CellSelect;


        }


        public static void searchFriends(DataGridView dgFriendsList, string searchName="",string searchBlogTitle="", string gubun="" ,string naverid="")
        {
            DBinit();

            String sql = "SELECT seq as 번호 , Id as 아이디  ,nickname as 이름, blogtitle as 블로그명, blogurl as 블로그URL,add_date as 네이버등록일,gubun_type as 구분 FROM  Friends  WHERE 1=1";
            if (searchName.Length > 0)
            {
                sql += " AND  nickname like '%"+ searchName + "%' ";
            }
            if (searchBlogTitle.Length > 0)
            {
                sql += " AND  blogtitle like '%" + searchBlogTitle + "%' ";
            }
            if (gubun.Length > 0 && !gubun.Equals("전체"))
            {
                sql += " AND  gubun_type = '" + gubun + "' ";
            }
            if (naverid.Length > 0 )
            {
                sql += " AND  id = '" + naverid + "' ";
            }

             sql += " AND  owner = '" + Common.currUser.Id + "' ";

            sql += " order by seq asc ";
            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt); 
            dgFriendsList.DataSource = dt.DefaultView;

        }
        public static DataTable searchFriends( string searchName = "", string searchBlogTitle = "", string gubun = "", string naverid = "")
        {
            DBinit();

            String sql = "SELECT seq as 번호 , Id as 아이디  ,nickname as 이름, blogtitle as 블로그명, blogurl as 블로그URL,add_date as 네이버등록일,gubun_type as 구분 FROM  Friends  WHERE 1=1";
            if (searchName.Length > 0)
            {
                sql += " AND  nickname like '%" + searchName + "%' ";
            }
            if (searchBlogTitle.Length > 0)
            {
                sql += " AND  blogtitle like '%" + searchBlogTitle + "%' ";
            }
            if (gubun.Length > 0 && !gubun.Equals("전체"))
            {
                sql += " AND  gubun_type = '" + gubun + "' ";
            }
            if (naverid.Length > 0)
            {
                sql += " AND  id = '" + naverid + "' ";
            }

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            sql += " order by seq asc ";
            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;

        }
        public static DataTable existFriend( string naverid )
        {
            DBinit();

            String sql = "SELECT seq as 번호 , Id as 아이디  ,nickname as 이름, blogtitle as 블로그명, blogurl as 블로그URL,add_date as 네이버등록일,gubun_type as 구분 FROM  Friends  WHERE 1=1";
 
            if (naverid.Length > 0)
            {
                sql += " AND  id = '" + naverid + "' ";
            }

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            sql += " order by seq asc ";
            Common.log.Debug(sql);
            cmd.CommandText = sql;

            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public static void InsertFriend(String friendId, String nickname, String blogtitle, String blogurl, String add_date, String gubun_type)
        {
            DBinit();


                cmd.CommandType = CommandType.Text;
                String sql = " INSERT  INTO Friends(Id,nickname,blogtitle, blogurl,add_date,gubun_type,owner) values( @friendId,@nickname,@blogtitle, @blogurl,@add_date,@gubun_type,@owner) ";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@friendId", friendId);
                cmd.Parameters.AddWithValue("@nickname", nickname);
                cmd.Parameters.AddWithValue("@blogtitle", blogtitle);
                cmd.Parameters.AddWithValue("@blogurl", blogurl);
                cmd.Parameters.AddWithValue("@add_date", add_date);
                cmd.Parameters.AddWithValue("@gubun_type", gubun_type);
                cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
                {
                    int result= cmd.ExecuteNonQuery();
                    Common.log.Debug(result.ToString() + "insert conunt");
                }
                catch (SqlException e)
                {
                    Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }
            Application.DoEvents();
 
        }

        public static void UpdateFriend(String friendId, String nickname, String blogtitle,  String add_date, String gubun_type)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = " UPDATE   Friends  SET " +
                          "nickname=@nickname," +
                          "blogtitle=@blogtitle," +
                          "add_date=@add_date," +
                          "gubun_type=@gubun_type " +
                          " WHERE Id=@friendId AND owner= @owner ";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@friendId", friendId);
            cmd.Parameters.AddWithValue("@nickname", nickname);
            cmd.Parameters.AddWithValue("@blogtitle", blogtitle);
            cmd.Parameters.AddWithValue("@add_date", add_date);
            cmd.Parameters.AddWithValue("@gubun_type", gubun_type);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "update conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }
            Application.DoEvents();

        }

        public static void allDeleteFriends()
        {
            DBinit();

  
                cmd.CommandType = CommandType.Text;
                String sql = " DELETE  FROM  Friends   ";
                sql += " WHERE  owner = '" + Common.currUser.Id + "' ";
            cmd.CommandText = sql;

                try
                {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "delete  conunt");
            }
                catch (SqlException e)
                {
                    Common.log.Debug(e.Message.ToString() + "Error Message");
                }
            finally
            {
                conn.Close();
            }

        }

        // 네이버친구  end  

        // 친한친구  start 



        public static void searchMyFriends(DataGridView dgMyFriendsList, string naverid = "", string searchName = "", string gubun = "")
        {


            DataTable dt = searchMyFriends(naverid, searchName, gubun);
            dgMyFriendsList.DataSource = dt.DefaultView;

        }

        public static DataTable searchMyFriends( string naverid = "", string searchName = "", string gubun = "")
        {
            DBinit();

            String sql = "SELECT  Id as 아이디  ,nickname as 이름, reg_dtm as 등록일 FROM  MyFriends  WHERE 1=1";
            if (searchName.Length > 0)
            {
                sql += " AND  nickname like '%" + searchName + "%' ";
            }

            if (gubun.Length > 0 && !gubun.Equals("전체"))
            {
                sql += " AND  gubun_type = '" + gubun + "' ";
            }
            if (naverid.Length > 0)
            {
                sql += " AND  id = '" + naverid + "' ";
            }

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            sql += " order by reg_dtm desc ";
            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch(Exception e)
            {
               Common.log.Debug("에러가 발생했어요."+ e.Message.ToString());
            }

            return dt;

        }

        public static void InsertMyFriend(String friendId, String nickname, String gubun_type)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = " INSERT  INTO MyFriends(Id,nickname,gubun_type,owner) values( @friendId,@nickname,@gubun_type,@owner) ";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@friendId", friendId);
            cmd.Parameters.AddWithValue("@nickname", nickname);
            cmd.Parameters.AddWithValue("@gubun_type", gubun_type);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "insert conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }


            Application.DoEvents();


        }
        public static void allDeleteMyFriends()
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = " DELETE  FROM  MyFriends ";
            cmd.CommandText = sql;

            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "delete  conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }

        public static int  DeleteMyFriends(string naverid)
        {
            DBinit();

            int result = 0;
            cmd.CommandType = CommandType.Text;
            String sql = " DELETE  FROM  MyFriends WHERE id=@naverid" ;
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@naverid", naverid);

            try
            {
                result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "delete  conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        // 친한친구  end  


        // 자동댓글작업 데이터 관리 start
        public static void searchAutoReplyWorkList(DataGridView AutoReplyWorkGrid, string regdate="", string naverid = "", string searchName = "",  string result_code = "", string work_id = "", string group_type = "BH")
        {


            DataTable dt = searchAutoReplyWorkList(regdate, naverid, searchName,result_code, work_id, group_type);
            AutoReplyWorkGrid.DataSource = dt.DefaultView;

        }

        //블로그홈용
        public static DataTable searchAutoReplyWorkList(string regdate="", string naverid = "", string searchName = "", string result_code="",string work_id="",string group_type= "BH")
        {
            DBinit();

            String sql = "SELECT  naverid as 아이디 , postid as 글번호 ,nickname as 이름, reg_dtm as 등록일시 ,title as 제목" +
                ", content as 내용 , start_dtm as 작업시작 ,end_dtm as 작업종료 ,result_code as 결과 " +
                "FROM  AutoReplyWork  WHERE group_type='"+group_type+"' ";
            if (searchName.Length > 0)
            {
                sql += " AND  nickname like '%" + searchName + "%' ";
            }

            if (naverid.Length > 0)
            {
                sql += " AND  naverid = '" + naverid + "' ";
            }
            if (result_code.Length > 0)
            {
                sql += " AND  result_code = '" + result_code + "' ";
            }
            if (work_id.Length > 0)
            {
                sql += " AND  work_id = " + work_id + "";
            }
            if (regdate.Length > 0)
            {
                 sql += " AND  reg_dtm  >=#" + regdate + " 00:00:00#";//등록일자
                sql += " AND  reg_dtm  <=#" + regdate + " 23:59:59#";//등록일자
            }
           

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            sql += " order by reg_dtm desc ";
            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
                Common.log.Debug("에러가 발생했어요." + e.Message.ToString());
            }

            return dt;

        }

        //블로그홈용
        public static DataTable searchAutoReplyWorkList2(string regdate = "", string naverid = "", string searchName = "", string result_code = "", string work_id = "", string group_type = "BH")
        {
            DBinit();

            String sql = "SELECT  seq,naverid  , postid  ,nickname , reg_dtm  ,title " +
                ", content  , start_dtm  ,end_dtm  ,result_code  " +
                "FROM  AutoReplyWork  WHERE group_type='" + group_type + "' ";
            if (searchName.Length > 0)
            {
                sql += " AND  nickname like '%" + searchName + "%' ";
            }

            if (naverid.Length > 0)
            {
                sql += " AND  naverid = '" + naverid + "' ";
            }
            if (result_code.Length > 0)
            {
                sql += " AND  result_code = '" + result_code + "' ";
            }
            if (work_id.Length > 0)
            {
                sql += " AND  work_id = " + work_id + "";
            }
            if (regdate.Length > 0)
            {
                sql += " AND  regdtm = '" + regdate + "' ";//등록일자
            }


            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            sql += " order by reg_dtm desc ";
            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
                Common.log.Debug("에러가 발생했어요." + e.Message.ToString());
            }

            return dt;

        }

        // 블로그홈용
        public static DataTable getAutoReplyWork( string naverid , string work_id, string group_type = "BH")
        {
            DBinit();

            String sql = "SELECT seq, naverid  , postid  ,nickname , reg_dtm ,title , content  , start_dtm  ,end_dtm  ,result_code  " +
                "FROM  AutoReplyWork  WHERE group_type='" + group_type + "' ";

            if (naverid.Length > 0)
            {
                sql += " AND  naverid = '" + naverid + "' ";
            }

            if (work_id.Length > 0)
            {
                sql += " AND  work_id = " + work_id + "";
            }

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
                Common.log.Error("에러가 발생했어요." + e.Message.ToString());
            }

            return dt;

        }

        public static void InsertAutoReplyWork(String naverid,string postid, String nickname, String title,string content,string  work_id, string group_type = "BH")
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = " INSERT  INTO AutoReplyWork(naverid,postid,nickname,title,content,work_id,owner,group_type)" +
                " values( @naverid,@postid,@nickname,@title,@content,@work_id,@owner,@group_type) ";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@naverid", naverid);
            cmd.Parameters.AddWithValue("@postid", postid);
            cmd.Parameters.AddWithValue("@nickname", nickname);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@content", content);
            cmd.Parameters.AddWithValue("@work_id", work_id);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            cmd.Parameters.AddWithValue("@group_type", @group_type);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "insert conunt");
            }
            catch (SqlException e)
            {
                Common.log.Error(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }

        public static void UpdateAutoReplyWorkStart(String seq)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = "Update AutoReplyWork SET start_dtm=Now() WHERE seq=@seq  AND  owner = @owner";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@seq", seq);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString()+ "update conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString()+ "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }
        public static void UpdateAutoReplyWorkEnd(String seq,String result_code)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = "Update AutoReplyWork SET result_code=@result_code, end_dtm=Now() WHERE seq=@seq  AND  owner = @owner";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@result_code", result_code);
            cmd.Parameters.AddWithValue("@seq", seq);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString()+ "update conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString()+ "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }

        public static void UpdateAutoReplyWorkPost(String seq, String postid,string title,string content)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = "Update AutoReplyWork SET postid=@postid,title=@title  WHERE seq=@seq  AND  owner = @owner";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@postid", postid);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@seq", seq);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString()+ "update conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString()+ "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }
        public static void allDeleteAutoReplyWork()
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = " DELETE  FROM  AutoReplyWork ";
            cmd.CommandText = sql;

            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString()+ "delete  conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString()+ "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }

        public static int DeleteAutoReplyWork(string naverid)
        {
            DBinit();

            int result = 0;
            cmd.CommandType = CommandType.Text;
            String sql = " DELETE  FROM  AutoReplyWork WHERE id=@naverid";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@naverid", naverid);

            try
            {
                result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString()+ "delete  conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString()+ "Error Message");
            }
            finally
            {
                conn.Close();
            }
            return result;
        }


        // 자동댓글작업 데이터 관리 end 

        // 자동 댓글 워크그룹 

        public static DataTable getAutoWorkJobList(string work_title="",int  work_id=0)
        {
            DBinit();

            String sql = "SELECT  work_id,work_title,work_desc,work_type,reg_dtm,total_cnt,process_cnt,success_cnt,fail_cnt,work_start_dtm,work_end_dtm ,target_cnt,delaytime,reply_content,empathy " +
                "FROM  AutoWorkJob  WHERE 1=1";
            if (work_title.Length > 0)
            {
                sql += " AND  work_title like '%" + work_title + "%' ";
            }

            if (work_id>0)
            {
                sql += " AND  work_id = " + work_id ;
            }

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            sql += " order by reg_dtm desc ";
            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
                Common.log.Debug("에러가 발생했어요." + e.Message.ToString());
            }

            return dt;

        }

        public static DataTable getAutoWorkJob(string work_title = "", int work_id = 0)
        {
            DBinit();

            String sql = "SELECT  work_id,work_title,work_desc,work_type,reg_dtm,total_cnt,process_cnt,success_cnt," +
                "fail_cnt,work_start_dtm,work_end_dtm ,target_cnt,delaytime,reply_content,empathy " +
                "FROM  AutoWorkJob  WHERE 1=1";
            if (work_title.Length > 0)
            {
                sql += " AND  work_title ='" + work_title + "' "; ;
            }

            if (work_id > 0)
            {
                sql += " AND  work_id = " + work_id ;
            }

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            Common.log.Debug(sql);
            cmd.CommandText = sql;

            

            OleDbDataAdapter da = new OleDbDataAdapter(cmd);


            DataTable ds = new DataTable();
            try
            {
                da.Fill(ds);
            }
            catch (Exception e)
            {
                Common.log.Debug("에러가 발생했어요." + e.Message.ToString());
            }

            return ds;

        }

        //예약작업 등록
        public static void InsertAutoWorkJobReserve(AutoWorkJobMoel autoWorkJobModel)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = " INSERT  INTO AutoWorkJob( work_title,work_desc,work_type,total_cnt ,target_cnt,delaytime,reply_content,empathy,owner) " +
                "values( @work_title,@work_desc,@work_type,@total_cnt ,@target_cnt,@delaytime,@reply_content,@empathy,@owner) ";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@work_title", autoWorkJobModel.work_title);
            cmd.Parameters.AddWithValue("@work_desc", autoWorkJobModel.work_desc);
            cmd.Parameters.AddWithValue("@work_type", autoWorkJobModel.work_type);
            cmd.Parameters.AddWithValue("@total_cnt", autoWorkJobModel.total_cnt);
            cmd.Parameters.AddWithValue("@target_cnt", autoWorkJobModel.target_cnt);
            cmd.Parameters.AddWithValue("@delaytime", autoWorkJobModel.delaytime);
            cmd.Parameters.AddWithValue("@reply_content", autoWorkJobModel.reply_content);
            cmd.Parameters.AddWithValue("@empathy", autoWorkJobModel.empathy);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString()+ "insert conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString()+ "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }



        //시작시간 업데이트 
        public static void UpdateAutoWorkJobStartTime(String work_id)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = "Update AutoWorkJob SET work_start_dtm=Now() WHERE work_id=@work_id  AND  owner = @owner";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@work_id", work_id);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString()+"update conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString()+ "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }

        //종료시간 업데이트 
        public static void UpdateAutoWorkJobEndTime(String work_id)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = "Update AutoWorkJob SET work_end_dtm=Now() WHERE work_id=@work_id  AND  owner = @owner";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@work_id", work_id);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "update conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }
        // 네이버 이웃 자동 댓글 워크그룹 

        public static DataTable getFreindAutoJobList(string work_title = "", int work_id = 0)
        {
            DBinit();

            String sql = "SELECT  work_id,work_title,start_num,end_num,delaytime,reply_content,empathy,owner,work_start_dtm,work_end_dtm " +
                "FROM  FriendAutoJob  WHERE 1=1";
            if (work_title.Length > 0)
            {
                sql += " AND  work_title like '%" + work_title + "%' ";
            }

            if (work_id > 0)
            {
                sql += " AND  work_id = " + work_id;
            }

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            sql += " order by reg_dtm desc ";
            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
                Common.log.Debug("에러가 발생했어요." + e.Message.ToString());
            }

            return dt;

        }

        public static FreindAutoJobMoel getFreindAutoJob(string work_title = "", int work_id = 0)
        {
            DBinit();

            String sql = "SELECT  work_id,work_title,start_num,end_num,delaytime,reply_content,empathy,owner,work_start_dtm,work_end_dtm " +
                          "FROM  FriendAutoJob  WHERE 1=1";
            if (work_title.Length > 0)
            {
                sql += " AND  work_title ='" + work_title + "' "; ;
            }

            if (work_id > 0)
            {
                sql += " AND  work_id = " + work_id;
            }

            sql += " AND  owner = '" + Common.currUser.Id + "' ";

            Common.log.Debug(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);


            DataTable ds = new DataTable();
            try
            {
                da.Fill(ds);
            }
            catch (Exception e)
            {
                Common.log.Debug("에러가 발생했어요." + e.Message.ToString());
            }

            FreindAutoJobMoel freindAutoJobMoel = null;
            if (ds.Rows.Count > 0)
            {
                freindAutoJobMoel = new FreindAutoJobMoel();
                freindAutoJobMoel.work_id = ds.Rows[0].Field<int>("work_id");
                freindAutoJobMoel.work_title = ds.Rows[0].Field<string>("work_title");
                freindAutoJobMoel.start_num = ds.Rows[0].Field<int>("start_num");
                freindAutoJobMoel.end_num = ds.Rows[0].Field<int>("end_num");
                freindAutoJobMoel.delaytime = ds.Rows[0].Field<int>("delaytime");
                freindAutoJobMoel.reply_content = ds.Rows[0].Field<String>("reply_content");
                freindAutoJobMoel.empathy = ds.Rows[0].Field<bool>("empathy");


            }

            return freindAutoJobMoel;

        }

        //예약작업 등록
        public static void InsertFreindAutoJob(FreindAutoJobMoel freindAutoJobMoel)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = " INSERT  INTO FriendAutoJob( work_title,start_num,end_num,delaytime,reply_content,empathy,owner) " +
                "values( @work_title,@start_num,@end_num,@delaytime,@reply_content,@empathy,@owner) ";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@work_title", freindAutoJobMoel.work_title);
            cmd.Parameters.AddWithValue("@start_num", freindAutoJobMoel.start_num);
            cmd.Parameters.AddWithValue("@end_num", freindAutoJobMoel.end_num);
            cmd.Parameters.AddWithValue("@delaytime", freindAutoJobMoel.delaytime);
            cmd.Parameters.AddWithValue("@reply_content", freindAutoJobMoel.reply_content);
            cmd.Parameters.AddWithValue("@empathy", freindAutoJobMoel.empathy);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "insert conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }



        //시작시간 업데이트 
        public static void UpdateFreindAutoJobStartTime(String work_id)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = "Update FriendAutoJob SET work_start_dtm=Now() WHERE work_id=@work_id  AND  owner = @owner";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@work_id", work_id);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "update conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }

        //종료시간 업데이트 
        public static void UpdateFreindAutoJobEndTime(String work_id)
        {
            DBinit();


            cmd.CommandType = CommandType.Text;
            String sql = "Update FriendAutoJob SET work_end_dtm=Now() WHERE work_id=@work_id  AND  owner = @owner";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@work_id", work_id);
            cmd.Parameters.AddWithValue("@owner", Common.currUser.Id);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Common.log.Debug(result.ToString() + "update conunt");
            }
            catch (SqlException e)
            {
                Common.log.Debug(e.Message.ToString() + "Error Message");
            }
            finally
            {
                conn.Close();
            }

        }
    }
}
