using Mysqlx.Expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

using MySql.Data.MySqlClient;
using System.Threading;     //connect to database


namespace caculator  
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        string first = "";
        string second = "";
        
        
        string userInput = "";

        char function;
        int result = 0;

        string oderInput = "";     //for postorder and preoder

        MySqlConnection con = new MySqlConnection(        //connection to database
            "server=localhost;userid=root;password= ;database=caculator;"          //connect to database
            );
        private string sql;
        MySqlCommand cmd;

        public Window1 Window1 { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }




        private void insert_Click(object sender, RoutedEventArgs e)
        {

            string connectionString = "server=localhost;userid=root;password=;database=caculator;";
            using(MySqlConnection con=new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string expression = inputDisplay.Content.ToString();
                    //check same expression or not
                    MySqlCommand checkCmd = new MySqlCommand("select count(*) from data_new where expression=@exp", con);
                    checkCmd.Parameters.AddWithValue("@exp", expression);
                    int count1 = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count1 > 0)
                    {
                        MessageBox.Show("this expression is already exists in the database.");

                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO data_new(expression, postoder, preoder, `decimal`, `binary`) VALUES (@exp, @post, @pre, @dec, @bin)", con);
                        cmd.Parameters.AddWithValue("@exp", inputDisplay.Content.ToString());   //database is varchar
                        cmd.Parameters.AddWithValue("@post", postoderDisplay.Content);
                        cmd.Parameters.AddWithValue("@pre", preoderDisplay.Content);
                        cmd.Parameters.AddWithValue("@dec", Evaluate(count(inputDisplay.Content.ToString())));    //database is int
                        cmd.Parameters.AddWithValue("@bin", binaryDisplay.Content);

                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("insert successfully!!!");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    con.Close() ;
                }               
            }
         
        }

        private void query_Click(object sender, RoutedEventArgs e)
        {
            Window1 = new Window1();  //連接到window1
            Window1.ShowDialog();
            // MessageBox.Show("1");
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            //inputDisplay.Content = "";  //清空
            userInput += "1";
            inputDisplay.Content += "1";
            

        }
        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            userInput += "2";
            inputDisplay.Content += "2";
            
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            userInput += "3";
            inputDisplay.Content += "3";
            
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            userInput += "4";
            inputDisplay.Content += "4";
            
        }
        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            userInput += "5";
            inputDisplay.Content += "5";
            
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            userInput += "6";
            inputDisplay.Content += "6";
            
        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            userInput += "7";
            inputDisplay.Content += "7";
            
        }
        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            userInput += "8";
            inputDisplay.Content += "8";
            
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            userInput += "9";
            inputDisplay.Content += "9";
            
        }

        private void btn0_Click(object sender, RoutedEventArgs e)
        {
            userInput += "0";
            inputDisplay.Content += "0";
           
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {

            inputDisplay.Content += "+";
            function = '+';
            first = userInput;
            userInput = "";

        }

        private void minus_Click(object sender, RoutedEventArgs e)
        {

            inputDisplay.Content += "-";
            function = '-';
            first = userInput;
            userInput = "";
        }

        private void multiple_Click(object sender, RoutedEventArgs e)
        {

            inputDisplay.Content += "*";
            function = '*';
            first = userInput;
            userInput = "";
        }

        private void division_Click(object sender, RoutedEventArgs e)
        {

            inputDisplay.Content += "/";
            function = '/';
            first = userInput;
            userInput = "";
        }

        private void equal_Click(object sender, RoutedEventArgs e)     // four function need to be placed
        {
           
            string expression = inputDisplay.Content.ToString();
            List<string> postfix1 =count(expression);
            int result=Evaluate(postfix1);

            decimalDisplay.Content = result.ToString();
            binaryDisplay.Content = binary_convert(result);
            postoderDisplay.Content = postoder(expression);
            preoderDisplay.Content = preoder(expression);
        }
        //---------------count area-----------------------
        //處理先乘除後加減的問題
        private static List<string> count(string expression)
        {
            Stack<char> stack=new Stack<char>();
            List<string> output = new List<string>();
            int i = 0;

            while (i < expression.Length)
            {
                char c=expression[i];
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }
                if (char.IsDigit(c))
                {
                    string number = "";
                    while(i<expression.Length && char.IsDigit(expression[i]))
                    {
                        number += expression[i];
                        i++;
                    }
                    output.Add(number);
                    i--;
                }
                else if (c == '+' || c=='-' || c=='*' || c == '/')
                {
                    while(stack.Count>0 && GetPrecedence(stack.Peek()) >= GetPrecedence(c))
                    {
                        output.Add(stack.Pop().ToString());

                    }
                    stack.Push(c);
                }
                i++;
            }
            while (stack.Count > 0)
            {
                output.Add(stack.Pop().ToString());
            }
            return output;
        }
        private static int GetPrecedence(char op)
        {
            if(op =='+' || op == '-')
            {
                return 1;
            }
            if(op =='*' || op == '/')
            {
                return 2;
            }
            return 0;
        }
        private static int Evaluate(List<string> postfix1)
        {
            Stack<int> stack = new Stack<int>();
            foreach(var token in postfix1)
            {
                if(int.TryParse(token,out int number))
                {
                    stack.Push(number);
                }
                else
                {
                    int b=stack.Pop();
                    int a=stack.Pop();
                    switch (token)
                    {
                        case "+":
                            stack.Push(a + b);
                            break;
                        case "-":
                            stack.Push(a - b);
                            break;
                        case "*":
                            stack.Push(a * b);
                            break;
                        case "/":
                            stack.Push(a / b);
                            break;

                    }
                }
            }
            return stack.Pop();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            //reset
            first = "";
            second = "";
            userInput = "";
            result = 0;
            inputDisplay.Content = "";
            decimalDisplay.Content = "";
            binaryDisplay.Content = "";
            preoderDisplay.Content = "";
            postoderDisplay.Content = "";
        }

        //----------binary area---------------------------
        private static string binary_convert(int num)      
        {
            int[] binaryNum = new int[32];
            string output = "";
            int i = 0;
            while (num > 0)
            {
                binaryNum[i] = num % 2;
                num = num / 2;
                i++;
            }
            for (int j = i - 1; j >= 0; j--)
            {
                output += binaryNum[j].ToString();
            }
            return output;
        }
        //---------preoder area-----------------------------
        private static string preoder(string input)
        {
            
            Stack<string> operators = new Stack<string>();
            Stack<Char> prefix = new Stack<Char>();

            for(int i = 0; i < input.Length; i++)
            {
                char c= input[i];
                if (char.IsDigit(c))
                {
                    operators.Push(c.ToString());
                }
                else if (prefix.Count == 0)
                {
                    prefix.Push(c);
                }
                
                else
                {
                    if(c=='+' || c == '-')
                    {
                        while (prefix.Count>0)
                        {
                            if (operators.Count < 2) break;
                            string v2 = operators.Pop();
                            string v1 = operators.Pop();
                            char o = prefix.Pop();
                            string t = o + v1 + v2;
                            operators.Push(t);
                        }
                        
                        prefix.Push(c);
                    }
                    else if(c == '*' || c == '/')
                    {
                        
                        while(prefix.Count>0 && (prefix.Peek()=='*' || prefix.Peek() == '/'))
                        {
                            if(operators.Count < 2) break;
                            string v2 = operators.Pop();
                            string v1 = operators.Pop();
                            char o = prefix.Pop();
                            string t = o + v1 + v2;
                            operators.Push(t);
                        }
                        prefix.Push(c);
                    }
                }
            }
            while(prefix.Count > 0)
            {
                if (operators.Count < 2) break;
                string v2 = operators.Pop();
                string v1 = operators.Pop();
                char o = prefix.Pop();
                string t = o + v1 + v2;
                operators.Push(t);
          
            }

            //string prefixAns = operators.Pop().ToString();
             return operators.Count > 0 ? operators.Pop() : string.Empty;
        }


        //---------postoder area-----------------------------
        private static string postoder(string input)      
        {
            Stack<string> operators = new Stack<string>();
            List<String> postfix = new List<String>();

            foreach (String current in input.ToCharArray().Select(c => c.ToString())){
                if (IsOperator(current))
                {
                    while (operators.Count > 0 && HasLowerPriority(current, operators.Peek()))
                    {
                        postfix.Add(operators.Pop());
                    }
                    operators.Push(current);
                }
                else
                {
                    postfix.Add(current);
                }
            }
            while (operators.Count > 0)
            {
                postfix.Add((String)operators.Pop());
            }
            return string.Join("", postfix);
        }
        //check operator or not
        public static bool IsOperator(string op)
        {
            return new List<string> { "+", "-", "*", "/" }.Contains(op);
        } 
        //check operator priority
        public static bool HasLowerPriority(string op1,string op2)
        {
            return Priority(op1) < Priority(op2);
        }
        public static int Priority(string op)
        {
            if(op=="+" || op=="-")
            {
                return 1;
            }
            else if (op=="*" || op=="/")
            {
                return 2;
            }
            else return 3;
        }
    }

}
