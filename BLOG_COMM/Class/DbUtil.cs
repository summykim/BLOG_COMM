using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLOG_COMM
{
     class DbUtil
    {
        private  static OleDbConnection conn = null;

        private static OleDbCommand cmd;
        private static SqlDataAdapter da;
        private static DataSet ds;
        private static DataTable dt;

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
                conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + @"\marbling.accdb;Persist Security Info=True";
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
            Console.WriteLine(sql);
            cmd.CommandText = sql;


            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgFriendsList.DataSource = dt.DefaultView;

          

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
                    Console.WriteLine(result.ToString(), "insert conunt");
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message.ToString(), "Error Message");
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
                Console.WriteLine(result.ToString(), "delete  conunt");
            }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message.ToString(), "Error Message");
                }
            finally
            {
                conn.Close();
            }

        }

        // 네이버친구  end  

        // 친한친구  start 



        public static void searchMyFriends(DataGridView dgMyFriendsList, string searchName = "", string gubun = "", string naverid = "")
        {
            DBinit();

            String sql = "SELECT  Id as 아이디  ,nickname as 이름, reg_dtm as 등록일,gubun_type as 구분 FROM  MyFriends  WHERE 1=1";
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
            Console.WriteLine(sql);
            cmd.CommandText = sql;



            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                dgMyFriendsList.DataSource = dt.DefaultView;
            }
            catch
            {
                MessageBox.Show("에러가 발생했어요.");
            }







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
                Console.WriteLine(result.ToString(), "insert conunt");
            }
            catch (OleDbException e)
            {
                Console.WriteLine(e.Message.ToString(), "Error Message");
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
                Console.WriteLine(result.ToString(), "delete  conunt");
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message.ToString(), "Error Message");
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
                Console.WriteLine(result.ToString(), "delete  conunt");
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message.ToString(), "Error Message");
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        // 친한친구  end  

    }
}
